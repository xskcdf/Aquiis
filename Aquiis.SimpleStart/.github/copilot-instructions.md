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

## Code Patterns & Conventions

### Blazor Component Structure

```csharp
@page "/propertymanagement/entities/create"
@using Aquiis.WebUI.Components.PropertyManagement.Entities
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

1. **Manual SQL Scripts**: Preferred approach - create numbered scripts in `Data/Scripts/` (e.g., `40_AddNewTable.sql`)
2. Update `ApplicationDbContext.cs` with DbSet and entity configuration
3. No EF migrations - database is managed via SQL scripts
4. Connection string in `appsettings.json`: `"DefaultConnection": "Data Source=./Data/app.db"`

### Running the Application

- **F5** in VS Code to debug (configured in `.vscode/launch.json`)
- Or: `dotnet run` in `Aquiis.WebUI/` directory
- Default URLs: `https://localhost:7244` (HTTPS), `http://localhost:5244` (HTTP)
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
