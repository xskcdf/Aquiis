# Aquiis Property Management System - AI Agent Instructions

## Project Overview

Aquiis is a multi-tenant property management system built with **ASP.NET Core 9.0 + Blazor Server**. It manages properties, tenants, leases, invoices, payments, documents, inspections, and maintenance requests with role-based access control.

## Architecture Fundamentals

### Multi-Tenant Design

- **Critical**: Every entity has an `OrganizationId` for tenant isolation
- **UserContextService** (`Services/UserContextService.cs`) provides cached access to current user's `OrganizationId`
- All service methods MUST filter by `OrganizationId` - see `PropertyManagementService` for the pattern
- Never hard-code organization filtering - always use `await _userContext.GetOrganizationIdAsync()`

### Service Layer Pattern

- **PropertyManagementService** is the central data access service (not repository pattern)
- Handles authorization, multi-tenant filtering, and business logic in one place
- All CRUD operations go through this service - components never access `ApplicationDbContext` directly
- Service is injected into Blazor components: `@inject PropertyManagementService PropertyService`

### Soft Delete Pattern

- Entities inherit from `BaseModel` which provides audit fields (`CreatedOn`, `CreatedBy`, `LastModifiedOn`, `LastModifiedBy`, `IsDeleted`)
- **Never hard delete** - always set `IsDeleted = true` when deleting
- Controlled by `ApplicationSettings.SoftDeleteEnabled` configuration
- All queries must filter `.Where(x => !x.IsDeleted)` - this is non-negotiable

### Authentication & Authorization

- ASP.NET Core Identity with custom `ApplicationUser` (adds `OrganizationId`, `FirstName`, `LastName`)
- Three primary roles: `Administrator`, `PropertyManager`, `Tenant` (defined in `ApplicationConstants`)
- Use `@attribute [Authorize(Roles = "Administrator,PropertyManager")]` on Blazor pages
- User context pattern: Inject `UserContextService` instead of repeatedly querying `AuthenticationStateProvider`

## Property & Tenant Lifecycle Workflows

### Property Status Management

Properties follow a status-driven lifecycle (string values from `ApplicationConstants.PropertyStatuses`):

- **Available** - Ready to market and show to prospects
- **ApplicationPending** - One or more applications submitted and under review
- **LeasePending** - Application approved, lease offered, awaiting tenant signature
- **Occupied** - Active lease in place
- **UnderRenovation** - Not marketable, undergoing repairs/upgrades
- **OffMarket** - Temporarily unavailable

**Important:** `Property.Status` is a `string` field (max 50 chars), NOT an enum. Always use `ApplicationConstants.PropertyStatuses.*` constants.

**Status transitions are automatic** based on application/lease workflow events.

### Prospect-to-Tenant Journey

1. **Lead/Inquiry** → ProspectiveTenant created with Status: `Inquiry`
2. **Tour Scheduled** → Tour record created, Status: `TourScheduled`
3. **Tour Completed** → Status: `Toured`, interest level captured
4. **Application Submitted** → RentalApplication created, **Property.Status → ApplicationPending**, Status: `ApplicationSubmitted`
   - **Page:** `/propertymanagement/prospects/{id}/submit-application`
   - Application fee collected (per-application, non-refundable)
   - Application valid for 30 days, auto-expires if not processed
   - Property status automatically changes from Available to ApplicationPending
   - All required fields: current address, landlord info, employment, references
   - Income-to-rent ratio calculated and displayed
5. **Screening** → ApplicationScreening created (background + credit checks), Status: `UnderReview`
   - **Page:** `/propertymanagement/applications/{id}/review` (Initiate Screening button)
   - Background check requested with status tracking
   - Credit check requested with credit score capture
   - Overall screening result: Pending, Passed, Failed, ConditionalPass
6. **Application Approved** → Lease created with Status: `Offered`, **Property.Status → LeasePending**, Status: `ApplicationApproved`
   - **Page:** `/propertymanagement/applications/{id}/review` (Approve button after screening passes)
   - All other pending applications for this property auto-denied
   - Lease offer expires in 30 days if not signed
   - `Lease.OfferedOn` and `Lease.ExpiresOn` (30 days) are set
7. **Lease Signed** → **Tenant created from ProspectiveTenant**, SecurityDeposit collected, **Property.Status → Occupied**, Status: `ConvertedToTenant`
   - **Page:** `/propertymanagement/leases/{id}/accept`
   - `TenantConversionService` handles conversion with validation
   - `Tenant.ProspectiveTenantId` links back to prospect for audit trail
   - `Lease.SignedOn` timestamp recorded for compliance
   - SecurityDeposit must be paid in full upfront
   - Move-in inspection auto-scheduled
8. **Lease Declined** → **Property.Status → Available or ApplicationPending** (if other apps exist), Status: `LeaseDeclined`
   - `Lease.DeclinedOn` timestamp recorded
9. **Application Denied** → Status: `ApplicationDenied`, Property returns to Available if no other pending apps

**Key Services:**

- `TenantConversionService` - Handles ProspectiveTenant → Tenant conversion
  - `ConvertProspectToTenantAsync(prospectId, userId)` - Creates tenant with audit trail
  - Returns existing Tenant if already converted (idempotent operation)
  - `IsProspectAlreadyConvertedAsync()` - Prevents duplicate conversions
  - `GetProspectHistoryForTenantAsync()` - Retrieves full prospect history for compliance

**Key Pages:**

- `GenerateLeaseOffer.razor` - `/propertymanagement/applications/{id}/generate-lease-offer`

  - Generates lease offer from approved application
  - Sets `Lease.OfferedOn` and `Lease.ExpiresOn` (30 days)
  - Updates Property.Status to LeasePending
  - Auto-denies all competing applications for the property
  - Accessible to PropertyManager and Administrator roles only

- `AcceptLease.razor` - `/propertymanagement/leases/{id}/accept`
  - Accepts lease offer with full signature audit trail
  - Captures: timestamp, IP address, user ID, payment method
  - Calls TenantConversionService to create Tenant record
  - Sets `Lease.SignedOn`, updates status to Active
  - Updates Property.Status to Occupied
  - Prevents acceptance of expired offers (checks `Lease.ExpiresOn`)
  - Includes decline workflow (sets `Lease.DeclinedOn`)

**Lease Lifecycle Fields:**

- `OfferedOn` (DateTime?) - When lease offer was generated
- `SignedOn` (DateTime?) - When tenant accepted/signed the lease
- `DeclinedOn` (DateTime?) - When tenant declined the offer
- `ExpiresOn` (DateTime?) - Offer expiration date (30 days from OfferedOn)

**Status Constants:**

- ProspectiveStatuses: `LeaseOffered`, `LeaseDeclined`, `ConvertedToTenant`
- ApplicationStatuses: `LeaseOffered`, `LeaseAccepted`, `LeaseDeclined`
- LeaseStatuses: `Offered`, `Active`, `Declined`, `Terminated`, `Expired`

### Multi-Lease Support

- Tenants can have **multiple active leases simultaneously**
- Same tenant can lease multiple units in same or different buildings
- Each lease has independent security deposit, dividend tracking, and payment schedule

### Security Deposit Investment Model

**Investment Pool Approach:**

- All security deposits pooled into investment account
- Annual earnings distributed as dividends
- Organization takes configurable percentage (default 20%), remainder distributed to tenants
- Dividend = (TenantShare / ActiveLeaseCount) per lease
- **Losses absorbed by organization** - no negative dividends

**Dividend Distribution Rules:**

- **Pro-rated** for tenants who moved in mid-year (e.g., 6 months = 50% dividend)
- Distributed at year-end even if tenant has moved out (sent to forwarding address)
- Tenant chooses: apply as lease credit OR receive as check
- Each active lease gets separate dividend (tenant with 2 leases gets 2 dividends)

**Tracking:**

- `SecurityDepositInvestmentPool` - annual pool performance
- `SecurityDepositDividend` - per-lease dividend with payment method choice
- Full audit trail of investment performance visible in tenant portal

### E-Signature & Audit Trail

- Lease offers require acceptance (checkbox "I Accept" for dev/demo)
- Full signature audit: IP address, timestamp, document version, user agent
- Lease offer expires after 30 days if not signed
- Unsigned leases roll to month-to-month at higher rate

## Code Patterns & Conventions

### Enums & Constants Location

- **Status and type values** stored as string constants in `ApplicationConstants.cs` static classes
- Example: `ApplicationConstants.PropertyStatuses.Available`, `ApplicationConstants.LeaseStatuses.Active`
- **Enums** (PropertyStatus, ProspectStatus, etc.) defined in `ApplicationSettings.cs` for type safety but NOT used in database
- Database fields use `string` type with validation against ApplicationConstants values
- Never hard-code status/type values - always reference ApplicationConstants classes

### Blazor Component Structure

```csharp
@page "/propertymanagement/entities/create"
@using Aquiis.SimpleStart.Components.PropertyManagement.Entities
@attribute [Authorize(Roles = "Administrator,PropertyManager")]
@inject PropertyManagementService PropertyService
@inject UserContextService UserContext
@inject NavigationManager Navigation
@rendermode InteractiveServer

// Component code follows...
```

### Creating Entities

```csharp
private async Task CreateEntity()
{
    entity.OrganizationId = await UserContext.GetOrganizationIdAsync();
    entity.CreatedBy = await UserContext.GetUserIdAsync();
    entity.CreatedOn = DateTime.UtcNow;

    await PropertyService.AddEntityAsync(entity);
    Navigation.NavigateTo("/propertymanagement/entities");
}
```

### Service Method Pattern

```csharp
public async Task<List<Entity>> GetEntitiesAsync()
{
    var organizationId = await _userContext.GetOrganizationIdAsync();

    return await _dbContext.Entities
        .Include(e => e.RelatedEntity)
        .Where(e => !e.IsDeleted && e.OrganizationId == organizationId)
        .ToListAsync();
}
```

### Constants Usage

- All dropdown values come from `ApplicationConstants` (never hard-code)
- Examples: `ApplicationConstants.LeaseStatuses.Active`, `ApplicationConstants.PropertyTypes.Apartment`
- Status/type classes are nested: `ApplicationConstants.PaymentMethods.AllPaymentMethods`

### Entity Relationships

- Properties have many Leases, Documents, Inspections
- Leases belong to Property and Tenant
- Invoices belong to Lease (get Property/Tenant through navigation)
- Always use `.Include()` to eager-load related entities in services

## Development Workflows

### Database Changes

1. **EF Core Migrations**: Primary approach for schema changes
   - Migrations stored in `Data/Migrations/`
   - Run `dotnet ef migrations add MigrationName --project Aquiis.SimpleStart`
   - Apply with `dotnet ef database update --project Aquiis.SimpleStart`
   - Generate SQL script: `dotnet ef migrations script --output schema.sql`
2. **SQL Scripts**: Reference scripts in `Data/Scripts/` (not executed, for documentation)
3. Update `ApplicationDbContext.cs` with DbSet and entity configuration
4. Connection string in `appsettings.json`: `"DefaultConnection": "DataSource=Infrastructure/Data/app.db;Cache=Shared"`
5. **Database**: SQLite (not SQL Server) - scripts will be SQLite syntax

### Development Workflows

**Running the Application:**

- **Ctrl+Shift+B** to run `dotnet watch` (hot reload, default build task)
- **F5** in VS Code to debug (configured in `.vscode/launch.json`)
- Or: `dotnet run` in `Aquiis.SimpleStart/` directory
- Default URLs: Check terminal output for ports
- Default admin: `superadmin@example.local` / `SuperAdmin@123!`

### Build Tasks (VS Code)

- `build` - Debug build (Ctrl+Shift+B)
- `watch` - Hot reload development mode
- `build-release` - Production build
- `publish` - Create deployment package

### Background Services

- **ScheduledTaskService** runs daily/hourly automated tasks
- Registered as hosted service in `Program.cs`
- Add new scheduled tasks to `ScheduledTaskService.cs` with proper scoping

## PDF Generation

- Uses **QuestPDF 2025.7.4** with Community License (configured in Program.cs)
- PDF generators in `Components/PropertyManagement/Documents/` (e.g., `LeasePdfGenerator.cs`)
- Always save generated PDFs to `Documents` table with proper associations
- Pattern: Generate → Save to DB → Return Document object → Navigate to view

## File Naming & Organization

### Component Structure

```
Components/PropertyManagement/[Entity]/
  ├── [Entity].cs (Model - inherits BaseModel)
  ├── Pages/
  │   ├── [Entities].razor (List view)
  │   ├── Create[Entity].razor
  │   ├── View[Entity].razor
  │   └── Edit[Entity].razor
  └── [Entity]PdfGenerator.cs (if applicable)
```

### Route Patterns

- List: `/propertymanagement/entities`
- Create: `/propertymanagement/entities/create`
- View: `/propertymanagement/entities/view/{id:int}`
- Edit: `/propertymanagement/entities/edit/{id:int}`

## Common Pitfalls to Avoid

1. **DO NOT** access `ApplicationDbContext` directly in components - always use `PropertyManagementService`
2. **DO NOT** forget `OrganizationId` filtering - security breach waiting to happen
3. **DO NOT** hard-code status values - use `ApplicationConstants` classes
4. **DO NOT** hard delete entities - always soft delete (check `SoftDeleteEnabled` setting)
5. **DO NOT** forget to set audit fields (`CreatedBy`, `CreatedOn`) when creating entities
6. **DO NOT** query without `.Include()` for navigation properties you'll need
7. **DO NOT** use `!` null-forgiving operator without null checks - validate properly

## Toast Notifications

- Use `ToastService` (singleton) for user feedback instead of JavaScript alerts
- Pattern: `await JSRuntime.InvokeVoidAsync("toastService.showSuccess", "Message")`
- Types: Success, Error, Warning, Info
- Auto-dismiss after 5 seconds (configurable)

## Document Management

- Binary storage in `Documents.FileData` (VARBINARY(MAX))
- View in browser: Use Blob URLs via `wwwroot/js/fileDownload.js`
- Download: Base64 encode and trigger download
- 10MB upload limit configured in components

## Financial Features

- Late fees auto-applied by `ScheduledTaskService` (daily at 2 AM)
- Payment tracking updates invoice status automatically
- Financial reports use `FinancialReportService` with PDF export
- Decimal precision: 18,2 for all monetary values

## Inspection Tracking

- 26-item checklist organized in 5 categories (Exterior, Interior, Kitchen, Bathroom, Systems)
- Routine inspections update `Property.NextRoutineInspectionDueDate`
- Generate PDFs with `InspectionPdfGenerator` and save to Documents

## Key Files to Reference

- `Program.cs` - Service registration, Identity config, startup logic
- `ApplicationConstants.cs` - All dropdown values, roles, statuses
- `PropertyManagementService.cs` - Service layer patterns
- `UserContextService.cs` - Multi-tenant context access
- `BaseModel.cs` - Audit field structure
- `ApplicationDbContext.cs` - Entity relationships

## When Adding New Features

1. Create model inheriting `BaseModel` with `OrganizationId` property
2. Add DbSet to `ApplicationDbContext` with proper relationships
3. Create SQL migration script in `Data/Scripts/`
4. Add CRUD methods to `PropertyManagementService` with org filtering
5. Create Blazor components following naming/routing conventions
6. Add constants to `ApplicationConstants` if needed
7. Update navigation in `NavMenu.razor` if top-level feature

## Code Style Notes

- Use async/await consistently (no `.Result` or `.Wait()`)
- Prefer explicit typing over `var` for service/entity types
- Use string interpolation for logging: `$"Processing {entityId}"`
- Handle errors with try-catch and user-friendly messages
- Include XML comments on service methods describing purpose
