# Routine Inspection Tracking System

## Overview

The inspection tracking system automatically monitors and manages routine property inspections on a 12-month cycle. It tracks completion dates, calculates next due dates, and alerts users to overdue or upcoming inspections.

## Database Migration

Before using the inspection tracking features, apply the database migration:

```bash
cd /home/cisguru/Source/Aquiis/Aquiis.SimpleStart
sqlite3 Data/app.db < Data/Scripts/33_UpdateTable-Properties-InspectionTracking.sql
```

This adds three new columns to the Properties table:

- `LastRoutineInspectionDate` - Date of last routine inspection
- `NextRoutineInspectionDueDate` - Date next routine inspection is due (set to 30 days from creation for new properties)
- `RoutineInspectionIntervalMonths` - Inspection interval (default: 12 months)

**Note:** Existing properties will have their `NextRoutineInspectionDueDate` set to 30 days from the migration date.

## Features

### 1. Automatic Tracking

**New Properties:**

- When a property is created, the system automatically sets `NextRoutineInspectionDueDate` to 30 days from the creation date
- This ensures every property has an initial inspection scheduled

**When a Routine inspection is created:**

- `LastRoutineInspectionDate` is set to the inspection date
- `NextRoutineInspectionDueDate` is automatically calculated (inspection date + 12 months)
- Property inspection status is updated

### 2. Property-Level Computed Properties

Each property has computed properties for inspection status:

- **IsInspectionOverdue** - True if NextRoutineInspectionDueDate has passed
- **DaysUntilInspectionDue** - Days until inspection is due (negative if overdue)
- **InspectionStatus** - One of:
  - "Overdue" - Past due date
  - "Due Soon" - Within 30 days
  - "Scheduled" - Future inspection scheduled
  - "Not Scheduled" - No inspection date set

### 3. Inspection Schedule Dashboard

View all properties and their inspection status at:
**`/propertymanagement/inspections/schedule`**

Features:

- Summary cards showing counts by status (Overdue, Due Soon, Scheduled, Not Scheduled)
- Dedicated section for overdue inspections with days overdue
- Dedicated section for inspections due within 30 days
- Complete list of all properties with inspection status
- Quick links to view property or create inspection

### 4. Property View Page Integration

The View Property page (`/propertymanagement/properties/view/{id}`) displays:

- Routine Inspection card in sidebar
- Last inspection date
- Next inspection due date
- Status badge (color-coded)
- Alert for overdue inspections (red)
- Warning for due soon inspections (yellow)
- Quick "Schedule Inspection" button

Status Badge Colors:

- **Red (Overdue)** - Inspection is past due
- **Yellow (Due Soon)** - Due within 30 days
- **Green (Scheduled)** - Future inspection scheduled
- **Gray (Not Scheduled)** - No inspection scheduled

### 5. Background Service Monitoring

The ScheduledTaskService runs daily at midnight and logs:

- Number of properties with overdue inspections
- Details of first 5 overdue properties (address, days overdue, due date)
- Number of properties with inspections due within 30 days

Example log output:

```
[Warning] 3 propert(ies) have overdue routine inspections
[Warning] Property 123 Main St - Inspection overdue by 45 days (Due: 2024-09-28)
[Information] 5 propert(ies) have routine inspections due within 30 days
```

## PropertyManagementService Methods

### UpdatePropertyInspectionTrackingAsync

```csharp
await PropertyManagementService.UpdatePropertyInspectionTrackingAsync(
    propertyId: 1,
    inspectionDate: DateTime.Today,
    intervalMonths: 12);
```

Manually updates property inspection tracking after an inspection is completed.

### GetPropertiesWithOverdueInspectionsAsync

```csharp
var overdueProperties = await PropertyManagementService.GetPropertiesWithOverdueInspectionsAsync();
```

Returns list of properties with overdue routine inspections, ordered by due date (oldest first).

### GetPropertiesWithInspectionsDueSoonAsync

```csharp
var dueSoonProperties = await PropertyManagementService.GetPropertiesWithInspectionsDueSoonAsync(daysAhead: 30);
```

Returns list of properties with inspections due within specified days.

### GetOverdueInspectionCountAsync

```csharp
int overdueCount = await PropertyManagementService.GetOverdueInspectionCountAsync();
```

Returns count of properties with overdue inspections (for dashboard widgets).

### InitializePropertyInspectionTrackingAsync

```csharp
await PropertyManagementService.InitializePropertyInspectionTrackingAsync(
    propertyId: 1,
    intervalMonths: 12);
```

Sets up inspection tracking for a property that doesn't have a schedule. Sets NextRoutineInspectionDueDate to current date + interval months.

## Workflow Example

1. **Property Manager creates property**

   - System automatically sets NextRoutineInspectionDueDate = 30 days from creation
   - InspectionStatus = "Scheduled"
   - Property appears on dashboard with green badge

2. **First routine inspection is performed (within 30 days)**

   - Navigate to property → Click "Create Inspection"
   - Select InspectionType = "Routine"
   - Complete inspection and save
   - System automatically sets:
     - LastRoutineInspectionDate = inspection date
     - NextRoutineInspectionDueDate = inspection date + 12 months

3. **11 months later**

   - InspectionStatus = "Scheduled" (still more than 30 days away)
   - Property card shows green "Scheduled" badge

4. **11.5 months later (within 30 days)**

   - InspectionStatus = "Due Soon"
   - Property card shows yellow warning badge
   - Dashboard shows property in "Due Soon" section

5. **13 months later (past due)**

   - InspectionStatus = "Overdue"
   - Property card shows red alert with days overdue
   - Dashboard shows property in "Overdue" section
   - Daily scheduled task logs warning

6. **Inspection completed**
   - Create new routine inspection
   - System automatically updates:
     - LastRoutineInspectionDate = new inspection date
     - NextRoutineInspectionDueDate = new inspection date + 12 months
   - Status resets to "Scheduled"

## Customization

### Changing Inspection Interval

To change from 12 months to a different interval:

1. **For new properties**: Modify default in Property.cs

   ```csharp
   public int RoutineInspectionIntervalMonths { get; set; } = 6; // 6-month inspections
   ```

2. **For existing property**: Update directly

   ```csharp
   property.RoutineInspectionIntervalMonths = 6;
   await PropertyManagementService.UpdatePropertyAsync(property);
   ```

3. **When creating inspection**: Pass custom interval
   ```csharp
   await PropertyManagementService.UpdatePropertyInspectionTrackingAsync(
       propertyId: 1,
       inspectionDate: DateTime.Today,
       intervalMonths: 6);
   ```

### Email Notifications (Future Enhancement)

Add to ScheduledTaskService.ExecuteDailyTasks():

```csharp
if (overdueProperties.Any())
{
    // Send email to property managers
    await EmailService.SendOverdueInspectionNotificationAsync(overdueProperties);
}

if (dueSoonProperties.Any())
{
    // Send reminder email
    await EmailService.SendInspectionReminderAsync(dueSoonProperties);
}
```

## Navigation

- **All Properties**: `/propertymanagement/properties`
- **Inspection Schedule**: `/propertymanagement/inspections/schedule`
- **View Property**: `/propertymanagement/properties/view/{id}`
- **Create Inspection**: `/propertymanagement/inspections/create/{propertyId}`

## Notes

- Only **Routine** inspections update the tracking dates
- Move-In, Move-Out, and Maintenance inspections do not affect the routine schedule
- Inspection tracking is scoped by organization (multi-tenant support)
- Soft-deleted properties are excluded from inspection tracking
- The system uses DateTime.Today for all date comparisons (no time component)
