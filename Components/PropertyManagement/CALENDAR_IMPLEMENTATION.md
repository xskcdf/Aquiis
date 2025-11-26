# Event Calendar Implementation

## Overview

Scalable unified calendar system that displays all schedulable events (Tours, Inspections, Maintenance Requests, and future entity types) in a single view with automatic synchronization.

## Architecture

### Core Pattern: Event Table with ISchedulableEntity Interface

- **CalendarEvent**: Central event model stored in database
- **ISchedulableEntity**: Interface implemented by any schedulable entity
- **CalendarEventService**: Centralized service handling all event CRUD operations
- **CalendarEventRouter**: Utility for navigating from events to source entity detail pages

### Scalability

Adding new schedulable entities requires only:

1. Implement `ISchedulableEntity` interface on the new entity model
2. Add `CalendarEventId` property (nullable int)
3. Call `CalendarEventService.CreateOrUpdateEventAsync()` in entity's service methods
4. Add route mapping to `CalendarEventRouter` (one line)

**Calendar.razor automatically displays new event types without code changes.**

## Components

### Models

#### ISchedulableEntity Interface (`Models/ISchedulableEntity.cs`)

```csharp
public interface ISchedulableEntity
{
    int? CalendarEventId { get; set; }
    string GetEventTitle();
    DateTime GetEventStart();
    int GetEventDuration();
    string GetEventType();
    int? GetPropertyId();
    string GetEventDescription();
    string? GetEventStatus();
}
```

#### CalendarEvent Model (`Models/CalendarEvent.cs`)

Primary event table with properties:

- `Title`, `Description`, `Location`
- `StartOn`, `EndOn`, `DurationMinutes`
- `EventType` (Tour, Inspection, Maintenance, etc.)
- `Status`, `Color`, `Icon`
- `PropertyId` (FK to Property, nullable)
- `SourceEntityId`, `SourceEntityType` (tracks source domain entity)
- `OrganizationId` (multi-tenancy)
- `IsCustomEvent` (computed: true when SourceEntityType is null)

#### CalendarEventTypes Constants (`Models/CalendarEventTypes.cs`)

Event type constants and configuration:

- `Tour`: #007bff, bi-house-door
- `Inspection`: #28a745, bi-clipboard-check
- `Maintenance`: #dc3545, bi-tools
- `LeaseExpiry`: #ffc107, bi-calendar-x
- `RentDue`: #17a2b8, bi-currency-dollar
- `Custom`: #6c757d, bi-calendar-event

Methods: `GetColor()`, `GetIcon()`, `GetDisplayName()`, `GetAllTypes()`

### Services

#### CalendarEventService (`Services/CalendarEventService.cs`)

Centralized calendar event management:

**Key Methods:**

- `CreateOrUpdateEventAsync<T>(T entity)`: Generic sync for any ISchedulableEntity
- `DeleteEventAsync(int? calendarEventId)`: Remove calendar event
- `GetEventsAsync(orgId, startDate, endDate, eventTypes?)`: Query with filtering
- `GetEventByIdAsync(eventId, orgId)`: Single event retrieval
- `CreateCustomEventAsync(CalendarEvent)`: User-created events
- `UpdateCustomEventAsync(CalendarEvent)`: Update custom events
- `GetUpcomingEventsAsync(orgId, days, eventTypes?)`: Future events

**Private Helpers:**

- `CreateEventFromEntity<T>()`: Maps ISchedulableEntity to CalendarEvent
- `UpdateEventFromEntity<T>()`: Updates existing event from entity

### Utilities

#### CalendarEventRouter (`Utilities/CalendarEventRouter.cs`)

Navigation routing for calendar events:

**Methods:**

- `GetRouteForEvent(CalendarEvent)`: Returns URL or null
- `IsRoutable(CalendarEvent)`: Check if event has route
- `GetSourceLabel(CalendarEvent)`: User-friendly type label

**Routes:**

- Tour → `/PropertyManagement/Tours/Details/{id}`
- Inspection → `/PropertyManagement/Inspections/View/{id}`
- MaintenanceRequest → `/PropertyManagement/Maintenance/View/{id}`

### Domain Services Integration

#### RentalApplicationService

Tour calendar event sync:

- `CreateTourAsync`: Creates calendar event after tour creation
- `UpdateTourAsync`: Updates calendar event after tour update
- `DeleteTourAsync`: Removes calendar event after tour deletion
- `CancelTourAsync`: Updates calendar event status when tour cancelled

#### PropertyManagementService

Inspection and Maintenance calendar event sync:

- `AddInspectionAsync`: Creates calendar event
- `UpdateInspectionAsync`: Updates calendar event
- `DeleteInspectionAsync`: Removes calendar event
- `AddMaintenanceRequestAsync`: Creates calendar event
- `UpdateMaintenanceRequestAsync`: Updates calendar event
- `DeleteMaintenanceRequestAsync`: Removes calendar event

### Database

#### Schema Changes

**CalendarEvents Table:**

- Id (PK), OrganizationId, Title, Description, Location
- StartOn, EndOn, DurationMinutes
- EventType, Status, Color, Icon
- PropertyId (FK, nullable, ON DELETE SET NULL)
- SourceEntityId, SourceEntityType
- CreatedOn, UpdatedOn, CreatedBy, UpdatedBy

**Indices:**

- OrganizationId
- StartOn
- EventType
- SourceEntityId
- Composite: (SourceEntityType, SourceEntityId)

**Updated Tables:**

- Tours: Added `CalendarEventId INT NULL`
- Inspections: Added `CalendarEventId INT NULL`
- MaintenanceRequests: Added `CalendarEventId INT NULL`

**Migration:** `20251126143634_AddCalendarEventsSystem`

#### ApplicationDbContext

- Added `DbSet<CalendarEvent> CalendarEvents`
- Configured indices and Foreign Key relationships

### Frontend

#### Calendar.razor (`Components/PropertyManagement/Calendar.razor`)

Unified calendar displaying all event types:

**Features:**

- **Three View Modes:** Day, Week, Month
- **Event Type Filtering:** Toggle visibility by event type (Tours, Inspections, Maintenance, etc.)
- **Color-Coded Events:** Each event type has distinct color and icon
- **Click-to-Navigate:** Events route to source entity detail pages
- **Automatic Refresh:** Loads events based on current date range and filters

**State Variables:**

- `allEvents`: List<CalendarEvent> - All loaded events
- `selectedEventTypes`: List<string> - Active event type filters
- `viewMode`: string - Current view (day/week/month)
- `currentDate`: DateTime - Currently displayed date
- `showFilters`: bool - Filter panel visibility

**Key Methods:**

- `LoadEvents()`: Queries CalendarEventService with date range and filters
- `ShowEventDetail(CalendarEvent)`: Navigates using CalendarEventRouter
- `ToggleEventType(string)`: Filter event types
- `RenderDayView()`, `RenderWeekView()`, `RenderMonthView()`: Render fragments

## Synchronization Strategy

### Automatic Sync Flow

1. User creates/updates/deletes domain entity (Tour, Inspection, MaintenanceRequest)
2. Domain service performs database operation on entity
3. Domain service calls `CalendarEventService.CreateOrUpdateEventAsync(entity)` or `DeleteEventAsync()`
4. CalendarEventService creates/updates/deletes corresponding CalendarEvent
5. Calendar view displays updated events on next load

### No Foreign Key Constraints

- `CalendarEventId` on entities is nullable
- Loose coupling prevents cascading delete issues
- Orphaned events cleaned up by service logic

### Sync Points

All entity CRUD operations trigger calendar sync:

- **Tours:** Create, Update, Delete, Cancel
- **Inspections:** Create, Update, Delete
- **Maintenance Requests:** Create, Update, Delete

## Usage Examples

### Creating a New Tour (Automatic Event Creation)

```csharp
var tour = new Tour { /* properties */ };
await ApplicationService.CreateTourAsync(tour, orgId);
// CalendarEvent automatically created
```

### Querying Calendar Events

```csharp
var events = await CalendarEventService.GetEventsAsync(
    organizationId: orgId,
    startDate: DateTime.Today,
    endDate: DateTime.Today.AddDays(30),
    eventTypes: new List<string> { CalendarEventTypes.Tour, CalendarEventTypes.Inspection }
);
```

### Creating Custom Event

```csharp
var customEvent = new CalendarEvent
{
    Title = "Team Meeting",
    Description = "Monthly planning meeting",
    StartOn = DateTime.Today.AddDays(7).AddHours(10),
    DurationMinutes = 60,
    EventType = CalendarEventTypes.Custom,
    OrganizationId = orgId
};
await CalendarEventService.CreateCustomEventAsync(customEvent);
```

### Navigating to Source Entity

```csharp
private void ShowEventDetail(CalendarEvent evt)
{
    if (CalendarEventRouter.IsRoutable(evt))
    {
        var route = CalendarEventRouter.GetRouteForEvent(evt);
        Navigation.NavigateTo(route);
    }
}
```

## Adding New Schedulable Entity Type

Example: Adding "Lease Signing" events

### 1. Create/Update LeaseSigning Model

```csharp
public class LeaseSigning : BaseModel, ISchedulableEntity
{
    public int? CalendarEventId { get; set; }
    public DateTime SigningScheduledOn { get; set; }
    public int LeaseId { get; set; }
    public int PropertyId { get; set; }
    // ... other properties

    // ISchedulableEntity Implementation
    public string GetEventTitle() => $"Lease Signing: {Property?.Address}";
    public DateTime GetEventStart() => SigningScheduledOn;
    public int GetEventDuration() => 60;
    public string GetEventType() => "LeaseSigning";
    public int? GetPropertyId() => PropertyId;
    public string GetEventDescription() => $"Lease signing for {Tenant?.FullName}";
    public string? GetEventStatus() => Status;
}
```

### 2. Add Event Type to CalendarEventTypes

```csharp
public const string LeaseSigning = "LeaseSigning";

public static readonly Dictionary<string, EventTypeConfig> Config = new()
{
    // ... existing types
    { LeaseSigning, new EventTypeConfig("#9c27b0", "bi-pen", "Lease Signing") }
};
```

### 3. Update LeaseSigningService

```csharp
public class LeaseSigningService
{
    private readonly CalendarEventService _calendarEventService;

    public async Task<LeaseSigning> CreateLeaseSigningAsync(LeaseSigning signing)
    {
        _context.LeaseSignings.Add(signing);
        await _context.SaveChangesAsync();

        // Sync to calendar
        await _calendarEventService.CreateOrUpdateEventAsync(signing);

        return signing;
    }

    // Similar for Update and Delete...
}
```

### 4. Add Route to CalendarEventRouter

```csharp
public static string? GetRouteForEvent(CalendarEvent calendarEvent)
{
    return calendarEvent.EventType switch
    {
        // ... existing routes
        "LeaseSigning" => $"/PropertyManagement/LeaseSignings/Details/{calendarEvent.SourceEntityId}",
        _ => null
    };
}
```

**That's it!** Calendar.razor automatically displays LeaseSigning events.

## Testing Checklist

### Event Creation

- [ ] Create Tour → Verify calendar event created
- [ ] Create Inspection → Verify calendar event created
- [ ] Create Maintenance Request → Verify calendar event created

### Event Updates

- [ ] Update Tour time → Verify calendar event updated
- [ ] Update Inspection date → Verify calendar event updated
- [ ] Update Maintenance Request schedule → Verify calendar event updated

### Event Deletion

- [ ] Delete Tour → Verify calendar event deleted
- [ ] Delete Inspection → Verify calendar event deleted
- [ ] Delete Maintenance Request → Verify calendar event deleted

### Status Changes

- [ ] Cancel Tour → Verify calendar event status updated

### Calendar Display

- [ ] Day view shows correct events
- [ ] Week view shows correct events
- [ ] Month view shows correct events
- [ ] Date navigation (Previous/Next/Today) works
- [ ] View mode switching works

### Filtering

- [ ] Toggle Tour filter → Events shown/hidden
- [ ] Toggle Inspection filter → Events shown/hidden
- [ ] Toggle Maintenance filter → Events shown/hidden
- [ ] Multiple filters can be active simultaneously
- [ ] Filter persists across view mode changes

### Navigation

- [ ] Click Tour event → Navigate to Tour Details
- [ ] Click Inspection event → Navigate to Inspection View
- [ ] Click Maintenance event → Navigate to Maintenance View

### Custom Events

- [ ] Create custom event → Appears on calendar
- [ ] Update custom event → Changes reflected
- [ ] Delete custom event → Removed from calendar
- [ ] Custom event click shows info (no navigation)

## Benefits

### For Users

- **Unified View**: All schedulable events in one place
- **Visual Clarity**: Color-coded event types with icons
- **Flexible Filtering**: Show/hide event types as needed
- **Easy Navigation**: Click events to view details

### For Developers

- **Scalable Architecture**: Add new event types without modifying calendar
- **Centralized Logic**: All sync in CalendarEventService
- **Type Safety**: ISchedulableEntity interface enforces contract
- **Loose Coupling**: No FK constraints, flexible relationships

### For System

- **Performance**: Indexed queries, efficient date filtering
- **Multi-tenancy**: OrganizationId on all events
- **Audit Trail**: CreatedOn/UpdatedOn tracking
- **Extensibility**: Custom events support future features

## Future Enhancements

### Potential Features

- [ ] Recurring events (weekly inspections, monthly rent due)
- [ ] Event reminders/notifications
- [ ] Drag-and-drop rescheduling
- [ ] Calendar export (iCal format)
- [ ] Event conflicts detection
- [ ] Bulk event operations
- [ ] Calendar sharing/permissions
- [ ] Mobile-responsive improvements
- [ ] Time zone support
- [ ] Event attachments/documents

## Files Modified/Created

### Created Files

- `Models/ISchedulableEntity.cs`
- `Models/CalendarEvent.cs`
- `Models/CalendarEventTypes.cs`
- `Services/CalendarEventService.cs`
- `Utilities/CalendarEventRouter.cs`
- `Data/Migrations/20251126143634_AddCalendarEventsSystem.cs`
- `Components/PropertyManagement/CALENDAR_IMPLEMENTATION.md` (this file)

### Modified Files

- `Models/Tour.cs` - Added ISchedulableEntity implementation
- `Components/PropertyManagement/Inspections/Inspection.cs` - Added ISchedulableEntity
- `Components/PropertyManagement/MaintenanceRequests/MaintenanceRequest.cs` - Added ISchedulableEntity
- `Data/ApplicationDbContext.cs` - Added CalendarEvents DbSet and configuration
- `Services/RentalApplicationService.cs` - Added calendar event sync
- `Components/PropertyManagement/PropertyManagementService.cs` - Added calendar event sync
- `Program.cs` - Registered CalendarEventService in DI
- `Components/PropertyManagement/Calendar.razor` - Complete refactor to use CalendarEventService

## Backup

Original Calendar.razor backed up to: `Calendar.razor.backup`

---

**Implementation Date:** November 26, 2024  
**Database Migration:** 20251126143634_AddCalendarEventsSystem  
**Status:** ✅ Complete - Ready for Testing
