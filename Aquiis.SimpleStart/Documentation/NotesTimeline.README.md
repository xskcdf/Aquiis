# NotesTimeline Component

A reusable timeline-based notes component that can be attached to any entity in the application.

## Features

- **Timeline View**: Notes display in reverse chronological order with timestamps
- **Add Notes**: Simple textarea with character counter (5000 char limit)
- **User Attribution**: Shows who created each note with formatted timestamps
- **Delete Notes**: Users can delete their own notes (soft delete)
- **Responsive Design**: Clean, card-based UI that works on all screen sizes
- **Real-time Updates**: Notes update immediately after add/delete operations

## Usage

### Basic Implementation

```razor
@using Aquiis.SimpleStart.Components.Shared

<NotesTimeline EntityType="CalendarEvent" EntityId="@eventId" />
```

### With Optional Parameters

```razor
<NotesTimeline
    EntityType="MaintenanceRequest"
    EntityId="@maintenanceId"
    CanDelete="true"
    OnNoteAdded="HandleNoteAdded" />
```

### Parameters

| Parameter     | Type          | Required | Default | Description                                                   |
| ------------- | ------------- | -------- | ------- | ------------------------------------------------------------- |
| `EntityType`  | string        | Yes      | -       | The entity type (e.g., "CalendarEvent", "MaintenanceRequest") |
| `EntityId`    | int           | Yes      | -       | The ID of the entity                                          |
| `CanDelete`   | bool          | No       | true    | Whether users can delete their own notes                      |
| `OnNoteAdded` | EventCallback | No       | -       | Callback triggered when a note is added                       |

### Example Integration Points

#### Custom Calendar Events

```razor
@if (selectedEvent != null && selectedEvent.IsCustomEvent)
{
    <NotesTimeline EntityType="CalendarEvent" EntityId="@selectedEvent.Id" />
}
```

#### Maintenance Requests

```razor
<div class="card">
    <div class="card-header">
        <h5>Maintenance Timeline</h5>
    </div>
    <div class="card-body">
        <NotesTimeline EntityType="MaintenanceRequest" EntityId="@maintenanceRequest.Id" />
    </div>
</div>
```

#### Lease Management

```razor
<NotesTimeline
    EntityType="Lease"
    EntityId="@leaseId"
    OnNoteAdded="RefreshLeaseData" />

@code {
    private async Task RefreshLeaseData()
    {
        // Refresh lease information or update UI
        await LoadLeaseDetails();
    }
}
```

#### Tenant Profile

```razor
<NotesTimeline EntityType="Tenant" EntityId="@tenantId" CanDelete="false" />
```

#### Property Details

```razor
<NotesTimeline EntityType="Property" EntityId="@propertyId" />
```

## Timestamp Formatting

Notes display relative timestamps for recent activity:

- "Just now" - Less than 1 minute ago
- "X minutes ago" - Less than 1 hour ago
- "X hours ago" - Less than 24 hours ago
- "X days ago" - Less than 7 days ago
- "MMM dd, yyyy at h:mm tt" - Older than 7 days

## Styling

The component includes built-in CSS with:

- Timeline connector lines
- Timeline dots
- Card-based note display
- Scrollable container (max 500px height)

## Database Schema

Notes are stored in the `Notes` table with:

- Polymorphic linking via `EntityType` and `EntityId`
- Multi-tenant support via `OrganizationId`
- Full audit trail (CreatedBy, CreatedOn, LastModifiedBy, LastModifiedOn)
- Soft delete support

## Service Layer

Uses `NoteService` for all operations:

- `AddNoteAsync()` - Create new note
- `GetNotesAsync()` - Retrieve notes for entity
- `DeleteNoteAsync()` - Soft delete note
- `GetNoteCountAsync()` - Count notes for entity

## Security

- Notes are scoped to the user's organization
- Users can only delete their own notes
- All operations require valid user and organization context
- Soft deletes maintain audit trail
