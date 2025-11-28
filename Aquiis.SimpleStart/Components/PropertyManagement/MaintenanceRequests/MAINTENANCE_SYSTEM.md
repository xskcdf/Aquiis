# Maintenance Request Management System

## Overview

The Maintenance Request Management System allows property managers to track and manage maintenance requests for properties. Requests can be linked to properties and optionally to active leases.

## Database Migration

To create the MaintenanceRequests table, run:

```bash
cd /home/cisguru/Source/Aquiis/Aquiis.SimpleStart
sqlite3 Data/app.db < Data/Scripts/34_CreateTable-MaintenanceRequests.sql
```

## Features

### 1. Maintenance Request Model

Each maintenance request includes:

- **Property** (required) - The property requiring maintenance
- **Lease** (optional) - Associated lease if applicable
- **Title** - Brief description
- **Description** - Detailed description of the issue
- **Request Type** - Plumbing, Electrical, Heating/Cooling, Appliance, Structural, Landscaping, Pest Control, Other
- **Priority** - Urgent, High, Medium, Low
- **Status** - Submitted, In Progress, Completed, Cancelled
- **Contact Information** - Requested by name, email, phone
- **Timeline** - Requested date, scheduled date, completed date
- **Cost Tracking** - Estimated cost and actual cost
- **Assignment** - Assigned to (contractor/maintenance person)
- **Resolution Notes** - Notes about how the issue was resolved

### 2. Computed Properties

- **DaysOpen** - Number of days since request was created (or until completion)
- **IsOverdue** - True if scheduled date has passed and status is not Completed/Cancelled
- **PriorityBadgeClass** - CSS class for priority badge (bg-danger, bg-warning, bg-info, bg-secondary)
- **StatusBadgeClass** - CSS class for status badge (bg-primary, bg-warning, bg-success, bg-secondary)

### 3. User Interface

#### Main List Page (`/propertymanagement/maintenance`)

- Summary cards showing:
  - Urgent requests (High priority, open)
  - In Progress requests
  - Submitted requests
  - Completed requests (current month)
- Filters by Status, Priority, and Request Type
- Overdue requests section (highlighted in red)
- Complete list of all requests with quick actions

#### Create Page (`/propertymanagement/maintenance/create/{PropertyId?}`)

- Form to create new maintenance request
- Property selection dropdown
- Lease selection (filtered by selected property)
- All request details
- Information sidebar with priority levels and request types

#### View Page (`/propertymanagement/maintenance/view/{Id}`)

- Complete request details
- Quick action buttons:
  - Start Work (Submitted → In Progress)
  - Mark Complete (In Progress → Completed)
  - Cancel Request
- Property information card
- Overdue alert if applicable

#### Edit Page (`/propertymanagement/maintenance/edit/{Id}`)

- Edit all request details
- Update status, priority, costs
- Add resolution notes
- Delete request option

### 4. Service Methods

**PropertyManagementService** provides:

```csharp
// Get all requests
GetMaintenanceRequestsAsync()

// Get by property
GetMaintenanceRequestsByPropertyAsync(propertyId)

// Get by lease
GetMaintenanceRequestsByLeaseAsync(leaseId)

// Get by status
GetMaintenanceRequestsByStatusAsync(status)

// Get by priority
GetMaintenanceRequestsByPriorityAsync(priority)

// Get overdue requests
GetOverdueMaintenanceRequestsAsync()

// Get counts
GetOpenMaintenanceRequestCountAsync()
GetUrgentMaintenanceRequestCountAsync()

// Get single request
GetMaintenanceRequestByIdAsync(id)

// CRUD operations
AddMaintenanceRequestAsync(request)
UpdateMaintenanceRequestAsync(request)
DeleteMaintenanceRequestAsync(id)

// Status update
UpdateMaintenanceRequestStatusAsync(id, status)
```

All methods are organization-scoped via UserContextService.

### 5. Navigation

The Maintenance link appears in the navigation menu for users with PropertyManager or Administrator roles.

**Routes:**

- `/propertymanagement/maintenance` - Main list
- `/propertymanagement/maintenance/create` - Create new request
- `/propertymanagement/maintenance/create/{PropertyId}` - Create for specific property
- `/propertymanagement/maintenance/view/{Id}` - View request details
- `/propertymanagement/maintenance/edit/{Id}` - Edit request

## Workflow Example

1. **Tenant reports issue**

   - Property manager creates maintenance request
   - Selects property and lease (if applicable)
   - Enters description, type, and priority
   - Request status: "Submitted"

2. **Assignment**

   - Manager assigns contractor
   - Sets scheduled date
   - Sets estimated cost
   - Updates status to "In Progress"

3. **Work completion**

   - Contractor completes work
   - Manager enters actual cost
   - Adds resolution notes
   - Updates status to "Completed"
   - Completed date automatically set

4. **Tracking & Reporting**
   - Dashboard shows all open requests
   - Overdue requests highlighted
   - Filter by status, priority, or type
   - Track costs (estimated vs actual)
   - Monitor days open

## Priority Levels

- **Urgent** (Red badge) - Immediate attention required (emergency repairs)
- **High** (Yellow badge) - Should be addressed soon (significant issues)
- **Medium** (Blue badge) - Normal priority (routine maintenance)
- **Low** (Gray badge) - Can wait (minor issues, improvements)

## Request Types

Based on `ApplicationConstants.MaintenanceRequestTypes`:

- **Plumbing** - Leaks, clogs, water issues
- **Electrical** - Wiring, outlets, lighting
- **Heating/Cooling** - HVAC, thermostat issues
- **Appliance** - Refrigerator, stove, dishwasher, etc.
- **Structural** - Walls, floors, roof, foundation
- **Landscaping** - Yard work, irrigation
- **Pest Control** - Insects, rodents
- **Other** - Miscellaneous requests

## Integration Points

### With Properties

- All requests linked to a property
- View maintenance history from property page (future enhancement)

### With Leases

- Optional lease association
- Track maintenance during tenant occupancy
- Cost allocation to tenant or owner (future enhancement)

### With Documents

- Attach photos, invoices, receipts (future enhancement)
- Generate maintenance reports (future enhancement)

## Future Enhancements

1. **Email Notifications**

   - Notify property managers of new urgent requests
   - Send reminders for scheduled maintenance
   - Alert when requests become overdue

2. **Mobile Support**

   - Tenants can submit requests via mobile app
   - Contractors can update status from field

3. **Recurring Maintenance**

   - Schedule routine maintenance (HVAC filter changes, etc.)
   - Auto-generate requests on schedule

4. **Vendor Management**

   - Maintain contractor database
   - Track vendor performance
   - Cost analysis by vendor

5. **Photo Attachments**

   - Before/after photos
   - Issue documentation

6. **Cost Analysis**
   - Track maintenance costs per property
   - Budget vs actual reporting
   - Trend analysis

## Notes

- All maintenance requests are scoped by organization (multi-tenant support)
- Soft-delete pattern used (IsDeleted flag)
- Audit trail maintained (CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
- Date calculations use .Date property for midnight-to-midnight comparisons
- Status "Completed" automatically sets CompletedDate to today
