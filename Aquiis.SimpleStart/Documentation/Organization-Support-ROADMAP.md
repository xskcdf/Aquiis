# Multi-Organization Support - Architecture Roadmap

**Date:** November 28, 2025  
**Status:** Planning / Design Phase  
**Objective:** Enable single business owner to manage multiple organizations with different property types

---

## Business Context

### Use Case

Joe Smith (business owner) operates multiple LLCs:

- **Residential LLC**: Single-family homes, condos, townhouses
- **Multi-Unit LLC**: Apartment complexes with 100+ units
- Each LLC has different:
  - Property management workflows
  - Tenant journeys
  - State regulations
  - Operational complexity

### Key Requirements

1. **Organization-level separation** - Each LLC is a separate organization
2. **Property type designation at org level** - Not per-property
3. **Completely different UIs** - Residential vs multi-unit workflows don't overlap
4. **State-specific compliance** - Different rules per state
5. **Shared financial infrastructure** - Invoices, payments, documents
6. **User can switch between organizations** - Single login, multiple contexts

---

## Proposed Architecture

### Core Principle

**Separate Domain Models, Shared Database, Divergent UIs**

Multi-unit properties are fundamentally different businesses with different workflows, regulations, and management needs. They should be treated as **separate business domains** that happen to share the same platform.

---

## 1. Organization-Level Property Type

### Model Enhancement

```csharp
// Models/Organization.cs
public class Organization : BaseModel
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Name { get; set; }

    // NEW: Define what type of property management this org does
    [Required]
    public OrganizationType Type { get; set; } = OrganizationType.Residential;

    // For multi-unit only
    public int? TotalUnits { get; set; }
    public int? TotalBuildings { get; set; }

    // For state-specific rules
    public string? PrimaryState { get; set; }

    // Existing: Address, phone, email, etc...
}

// Models/OrganizationType.cs
public enum OrganizationType
{
    [Display(Name = "Single-Family Residential")]
    Residential = 1,

    [Display(Name = "Multi-Unit Complex")]
    MultiUnit = 2,

    [Display(Name = "Commercial Properties")]
    Commercial = 3,

    [Display(Name = "Mixed Portfolio")]
    Mixed = 4
}
```

### Database Changes

- Add `Type` column to `Organizations` table
- Add `TotalUnits` (nullable int)
- Add `TotalBuildings` (nullable int)
- Add `PrimaryState` (nullable string, 2 chars)

---

## 2. User-Organization Relationship

### Current State

- `ApplicationUser.OrganizationId` (string) - Single organization per user

### Proposed Enhancement

Create many-to-many relationship for multi-org users:

```csharp
// Models/UserOrganization.cs
public class UserOrganization
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string OrganizationId { get; set; }
    public string Role { get; set; } // "Owner", "Manager", "Viewer"
    public bool IsPrimary { get; set; } // Default org on login

    public ApplicationUser User { get; set; }
    public Organization Organization { get; set; }
}

// Update ApplicationUser.cs
public class ApplicationUser : IdentityUser
{
    [Obsolete("Use ActiveOrganizationId instead")]
    public string OrganizationId { get; set; } // Keep for backward compatibility

    public string? ActiveOrganizationId { get; set; } // Currently selected org
    public List<UserOrganization> Organizations { get; set; }
}
```

### Benefits

- User can belong to multiple organizations
- Easy to switch context between Joe's residential and multi-unit LLCs
- Role-based permissions per organization

---

## 3. Separate Domain Models

### Folder Structure

```
/Models/
  /Residential/         ← Single-family homes
    - ResidentialProperty.cs
    - ResidentialLease.cs
    - ResidentialTenant.cs
    - ResidentialMaintenance.cs

  /MultiUnit/          ← Apartment complexes
    - ApartmentComplex.cs
    - ApartmentBuilding.cs (if multiple buildings per complex)
    - ApartmentUnit.cs
    - ApartmentLease.cs
    - ApartmentTenant.cs
    - ApartmentMaintenance.cs

  /Shared/             ← Common models
    - BaseProperty.cs (abstract)
    - BaseTenant.cs (abstract)
    - BaseLease.cs (abstract)
    - Document.cs
    - Invoice.cs
    - Payment.cs
```

### Abstract Base Classes

```csharp
// Models/Shared/BaseProperty.cs
public abstract class BaseProperty : BaseModel
{
    public string OrganizationId { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
    public bool IsActive { get; set; }

    // Polymorphic - returns "Residential" or "MultiUnit"
    public abstract string PropertyCategory { get; }
}
```

### Residential Models

```csharp
// Models/Residential/ResidentialProperty.cs
public class ResidentialProperty : BaseProperty
{
    public override string PropertyCategory => "Residential";

    public string PropertyType { get; set; } // House, Condo, Townhouse
    public int Bedrooms { get; set; }
    public decimal Bathrooms { get; set; }
    public int SquareFeet { get; set; }
    public decimal MonthlyRent { get; set; }
    public bool HasGarage { get; set; }
    public bool HasYard { get; set; }

    // Single tenant
    public int? CurrentLeaseId { get; set; }
    public ResidentialLease? CurrentLease { get; set; }
}
```

### Multi-Unit Models

```csharp
// Models/MultiUnit/ApartmentComplex.cs
public class ApartmentComplex : BaseProperty
{
    public override string PropertyCategory => "MultiUnit";

    public string ComplexName { get; set; } // "Sunset Towers", "Oak Ridge Apartments"
    public int TotalUnits { get; set; }
    public int TotalBuildings { get; set; }

    // Amenities specific to complexes
    public bool HasPool { get; set; }
    public bool HasGym { get; set; }
    public bool HasLaundry { get; set; }
    public string ParkingType { get; set; } // "Covered", "Open", "Garage"

    // Navigation
    public List<ApartmentBuilding> Buildings { get; set; }
    public List<ApartmentUnit> Units { get; set; }
}

// Models/MultiUnit/ApartmentBuilding.cs
public class ApartmentBuilding : BaseModel
{
    public int ComplexId { get; set; }
    public ApartmentComplex Complex { get; set; }

    public string BuildingName { get; set; } // "Building A", "North Tower"
    public string BuildingNumber { get; set; }
    public int Floors { get; set; }
    public int UnitsPerFloor { get; set; }

    public List<ApartmentUnit> Units { get; set; }
}

// Models/MultiUnit/ApartmentUnit.cs
public class ApartmentUnit : BaseModel
{
    public int ComplexId { get; set; }
    public int? BuildingId { get; set; } // Nullable if single building

    public string OrganizationId { get; set; }
    public string UnitNumber { get; set; } // "301", "2B"
    public int Floor { get; set; }

    public string UnitType { get; set; } // "Studio", "1BR", "2BR"
    public int Bedrooms { get; set; }
    public decimal Bathrooms { get; set; }
    public int SquareFeet { get; set; }
    public decimal MonthlyRent { get; set; }

    public bool IsAvailable { get; set; }
    public string? AvailabilityStatus { get; set; } // "Vacant", "Occupied", "Notice", "Turnover"

    // Navigation
    public ApartmentComplex Complex { get; set; }
    public ApartmentBuilding? Building { get; set; }
    public int? CurrentLeaseId { get; set; }
    public ApartmentLease? CurrentLease { get; set; }
}
```

---

## 4. Database Schema

### Separate Tables (Same Database)

**DbContext Configuration:**

```csharp
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    // Residential Domain
    public DbSet<ResidentialProperty> ResidentialProperties { get; set; }
    public DbSet<ResidentialLease> ResidentialLeases { get; set; }
    public DbSet<ResidentialTenant> ResidentialTenants { get; set; }

    // Multi-Unit Domain
    public DbSet<ApartmentComplex> ApartmentComplexes { get; set; }
    public DbSet<ApartmentBuilding> ApartmentBuildings { get; set; }
    public DbSet<ApartmentUnit> ApartmentUnits { get; set; }
    public DbSet<ApartmentLease> ApartmentLeases { get; set; }
    public DbSet<ApartmentTenant> ApartmentTenants { get; set; }

    // Shared Domain
    public DbSet<Document> Documents { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<UserOrganization> UserOrganizations { get; set; }

    // Legacy (keep for backward compatibility during migration)
    public DbSet<Property> Properties { get; set; }
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Lease> Leases { get; set; }
}
```

**Database Tables:**

- `Organizations` (enhanced with Type, TotalUnits, TotalBuildings, PrimaryState)
- `UserOrganizations` (new - junction table)
- `ResidentialProperties`
- `ResidentialLeases`
- `ResidentialTenants`
- `ApartmentComplexes`
- `ApartmentBuildings`
- `ApartmentUnits`
- `ApartmentLeases`
- `ApartmentTenants`
- Shared: `Documents`, `Invoices`, `Payments`

---

## 5. Shared Financial Models - NEEDS DISCUSSION

### Current Question

**How do we handle documents and invoices across different property types?**

### Option A: Polymorphic References (Flexible)

```csharp
public class Invoice : BaseModel
{
    public string OrganizationId { get; set; }

    // Polymorphic relationship
    public string PropertyType { get; set; } // "Residential", "ApartmentUnit"
    public int PropertyId { get; set; } // ID in respective table

    public int? TenantId { get; set; }
    public int? LeaseId { get; set; }

    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    // ... invoice fields
}

public class Document : BaseModel
{
    public string OrganizationId { get; set; }

    // Polymorphic relationship
    public string EntityType { get; set; } // "ResidentialProperty", "ApartmentUnit", "Lease"
    public int EntityId { get; set; }

    public string FileName { get; set; }
    // ... document fields
}
```

**Pros:**

- Single invoice/document table
- Works with any property type
- Easy to add new property types later

**Cons:**

- Can't use foreign keys (no referential integrity)
- Queries require type checking
- More complex to retrieve related entities

### Option B: Separate Invoice Tables

```csharp
public class ResidentialInvoice : BaseModel
{
    public int ResidentialPropertyId { get; set; }
    public ResidentialProperty Property { get; set; }

    public int ResidentialLeaseId { get; set; }
    public ResidentialLease Lease { get; set; }

    // ... invoice fields
}

public class ApartmentInvoice : BaseModel
{
    public int ApartmentUnitId { get; set; }
    public ApartmentUnit Unit { get; set; }

    public int ApartmentLeaseId { get; set; }
    public ApartmentLease Lease { get; set; }

    // ... invoice fields
}
```

**Pros:**

- Strong typing
- Foreign key constraints
- Clear domain separation

**Cons:**

- Duplicate code
- Hard to get "all invoices across all types"
- More tables to manage

### Option C: Hybrid - Shared Base with Type-Specific Extensions

```csharp
// Shared invoice table
public class Invoice : BaseModel
{
    public string OrganizationId { get; set; }
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public InvoiceType Type { get; set; } // Residential, MultiUnit
    // ... common fields
}

// Extension tables for type-specific data
public class ResidentialInvoiceDetail
{
    public int InvoiceId { get; set; }
    public Invoice Invoice { get; set; }

    public int ResidentialPropertyId { get; set; }
    public ResidentialProperty Property { get; set; }
}

public class ApartmentInvoiceDetail
{
    public int InvoiceId { get; set; }
    public Invoice Invoice { get; set; }

    public int ApartmentUnitId { get; set; }
    public ApartmentUnit Unit { get; set; }
}
```

**Pros:**

- Shared invoice logic
- Can query all invoices easily
- Type-specific relationships preserved

**Cons:**

- More complex schema
- Join required for full details

### **DECISION NEEDED:** Which approach for shared financial models?

**Questions to Consider:**

1. Do we need to query "all invoices across all property types" frequently?
2. How important is foreign key integrity for invoices?
3. Will invoice business logic differ significantly between residential and multi-unit?
4. Are there fields on invoices that only apply to one property type?

**Recommendation (pending discussion):**
Start with **Option A (Polymorphic)** because:

- Simpler initial implementation
- Documents already use this pattern (EntityType/EntityId)
- Easy to consolidate financial reporting
- Can refactor later if needed

---

## 6. Separate Services

### Service Layer Structure

```
/Services/
  /Residential/
    - ResidentialPropertyService.cs
    - ResidentialLeaseService.cs
    - ResidentialTenantService.cs

  /MultiUnit/
    - MultiUnitPropertyService.cs
    - ApartmentUnitService.cs
    - ApartmentLeaseService.cs

  /Shared/
    - InvoiceService.cs
    - PaymentService.cs
    - DocumentService.cs
    - StateComplianceService.cs
```

### Example Services

```csharp
// Services/Residential/ResidentialPropertyService.cs
public class ResidentialPropertyService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserContextService _userContext;

    public async Task<List<ResidentialProperty>> GetAvailablePropertiesAsync()
    {
        var orgId = await _userContext.GetOrganizationIdAsync();
        return await _dbContext.ResidentialProperties
            .Where(p => p.OrganizationId == orgId && p.IsActive)
            .ToListAsync();
    }

    // Simple lease creation - one property, one tenant
    public async Task CreateLeaseAsync(ResidentialLease lease) { }
}

// Services/MultiUnit/MultiUnitPropertyService.cs
public class MultiUnitPropertyService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserContextService _userContext;

    public async Task<List<ApartmentUnit>> GetVacantUnitsAsync(int complexId)
    {
        return await _dbContext.ApartmentUnits
            .Include(u => u.Complex)
            .Include(u => u.Building)
            .Where(u => u.ComplexId == complexId && u.IsAvailable)
            .OrderBy(u => u.UnitNumber)
            .ToListAsync();
    }

    public async Task<OccupancyReport> GetComplexOccupancyAsync(int complexId)
    {
        var complex = await _dbContext.ApartmentComplexes
            .Include(c => c.Units)
            .FirstOrDefaultAsync(c => c.Id == complexId);

        return new OccupancyReport
        {
            TotalUnits = complex.TotalUnits,
            OccupiedUnits = complex.Units.Count(u => !u.IsAvailable),
            VacantUnits = complex.Units.Count(u => u.IsAvailable),
            OccupancyRate = (decimal)complex.Units.Count(u => !u.IsAvailable) / complex.TotalUnits * 100
        };
    }

    // Bulk operations: lease renewals, rent increases, move-out processing
}
```

---

## 7. Completely Separate UIs

### UI Folder Structure

```
/Components/
  /ResidentialManagement/        ← Single-family UI
    /Properties/
      - Index.razor               (List of houses/condos)
      - Create.razor
      - View.razor
      - Edit.razor
    /Tenants/
      - Index.razor
      - Create.razor
      - View.razor
    /Leases/
      - Create.razor
      - View.razor
    /Maintenance/
      - Index.razor
      - Create.razor

  /MultiUnitManagement/          ← Apartment complex UI
    /Complexes/
      - Index.razor               (List of complexes)
      - Create.razor
      - View.razor                (Complex dashboard with occupancy)
    /Buildings/
      - Manage.razor              (Manage buildings in complex)
    /Units/
      - Index.razor               (All units grid view)
      - UnitDetails.razor
      - BulkActions.razor         (Bulk rent increase, status changes)
    /Leases/
      - BatchRenewal.razor        (Process multiple renewals)
      - Index.razor
    /Maintenance/
      - WorkOrderQueue.razor      (Centralized work orders)
      - Create.razor
    /Reports/
      - OccupancyReport.razor
      - RevenueAnalysis.razor
      - UnitTurnoverReport.razor

  /Shared/                       ← Common components
    /Financial/
      - InvoiceList.razor
      - PaymentHistory.razor
      - CreateInvoice.razor
    /Documents/
      - DocumentManager.razor
      - Upload.razor
    /Layout/
      - ResidentialLayout.razor
      - MultiUnitLayout.razor
```

### Different Navigation Menus

```razor
<!-- ResidentialNavMenu.razor -->
<div class="nav-menu-residential">
    <div class="nav-item">
        <NavLink href="/residential/dashboard">
            <i class="bi bi-speedometer"></i> Dashboard
        </NavLink>
    </div>
    <div class="nav-item">
        <NavLink href="/residential/properties">
            <i class="bi bi-house"></i> Properties
        </NavLink>
    </div>
    <div class="nav-item">
        <NavLink href="/residential/tenants">
            <i class="bi bi-people"></i> Tenants
        </NavLink>
    </div>
    <div class="nav-item">
        <NavLink href="/residential/leases">
            <i class="bi bi-file-text"></i> Leases
        </NavLink>
    </div>
    <div class="nav-item">
        <NavLink href="/residential/maintenance">
            <i class="bi bi-tools"></i> Maintenance
        </NavLink>
    </div>
</div>

<!-- MultiUnitNavMenu.razor -->
<div class="nav-menu-multiunit">
    <div class="nav-item">
        <NavLink href="/multiunit/dashboard">
            <i class="bi bi-building"></i> Dashboard
        </NavLink>
    </div>
    <div class="nav-item">
        <NavLink href="/multiunit/complexes">
            <i class="bi bi-buildings"></i> Complexes
        </NavLink>
    </div>
    <div class="nav-item">
        <NavLink href="/multiunit/units">
            <i class="bi bi-grid-3x3"></i> Unit Management
        </NavLink>
    </div>
    <div class="nav-item">
        <NavLink href="/multiunit/leases">
            <i class="bi bi-file-earmark-text"></i> Leases
        </NavLink>
    </div>
    <div class="nav-item">
        <NavLink href="/multiunit/workorders">
            <i class="bi bi-tools"></i> Work Orders
        </NavLink>
    </div>
    <div class="nav-item">
        <NavLink href="/multiunit/reports">
            <i class="bi bi-graph-up"></i> Analytics
        </NavLink>
    </div>
</div>
```

### Route-Based Layout Selection

```csharp
// App.razor
<Router AppAssembly="@typeof(App).Assembly">
    <Found Context="routeData">
        <RouteView RouteData="@routeData" DefaultLayout="@GetLayoutForRoute(routeData)" />
    </Found>
</Router>

@code {
    private Type GetLayoutForRoute(RouteData routeData)
    {
        var path = routeData.RouteValues["page"]?.ToString() ?? "";

        if (path.StartsWith("/residential/"))
            return typeof(ResidentialLayout);
        else if (path.StartsWith("/multiunit/"))
            return typeof(MultiUnitLayout);
        else
            return typeof(MainLayout);
    }
}
```

---

## 8. Organization Switcher

### Enhanced UserContextService

```csharp
public class UserContextService
{
    private string? _activeOrganizationId;
    private Organization? _activeOrganization;
    private List<Organization> _availableOrganizations;

    public async Task<string?> GetActiveOrganizationIdAsync()
    {
        await EnsureInitializedAsync();
        return _activeOrganizationId ?? await GetPrimaryOrganizationIdAsync();
    }

    public async Task<Organization?> GetActiveOrganizationAsync()
    {
        await EnsureInitializedAsync();
        return _activeOrganization;
    }

    public async Task<List<Organization>> GetAvailableOrganizationsAsync()
    {
        await EnsureInitializedAsync();
        return _availableOrganizations;
    }

    public async Task SwitchOrganizationAsync(string newOrgId)
    {
        // Validate user has access
        var userOrgs = await GetAvailableOrganizationsAsync();
        if (!userOrgs.Any(o => o.Id.ToString() == newOrgId))
            throw new UnauthorizedAccessException("User does not have access to this organization");

        // Update active org
        _activeOrganizationId = newOrgId;
        _activeOrganization = userOrgs.First(o => o.Id.ToString() == newOrgId);

        // Persist to database
        var user = await _userManager.GetUserAsync(_authenticationStateProvider.GetAuthenticationStateAsync().Result.User);
        if (user != null)
        {
            user.ActiveOrganizationId = newOrgId;
            await _userManager.UpdateAsync(user);
        }

        // Trigger event for UI refresh
        OnOrganizationChanged?.Invoke();
    }

    public event Action? OnOrganizationChanged;
}
```

### UI Component

```razor
<!-- Components/Shared/OrganizationSwitcher.razor -->
@inject UserContextService UserContext
@inject NavigationManager NavigationManager

<div class="org-switcher dropdown">
    <button class="btn btn-outline-light dropdown-toggle" type="button" data-bs-toggle="dropdown">
        <i class="bi bi-building"></i>
        @currentOrg?.Name
        <span class="badge bg-secondary">@currentOrg?.Type.GetDisplayName()</span>
    </button>
    <ul class="dropdown-menu">
        @foreach (var org in availableOrgs)
        {
            <li>
                <a class="dropdown-item @(org.Id == currentOrg?.Id ? "active" : "")"
                   @onclick="() => SwitchOrg(org.Id)">
                    <i class="bi bi-building"></i> @org.Name
                    <br />
                    <small class="text-muted">@org.Type.GetDisplayName()</small>
                </a>
            </li>
        }
        <li><hr class="dropdown-divider"></li>
        <li>
            <a class="dropdown-item" href="/organizations/manage">
                <i class="bi bi-gear"></i> Manage Organizations
            </a>
        </li>
        <li>
            <a class="dropdown-item" href="/organizations/create">
                <i class="bi bi-plus-circle"></i> Add Organization
            </a>
        </li>
    </ul>
</div>

@code {
    private Organization? currentOrg;
    private List<Organization> availableOrgs = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadOrganizations();
        UserContext.OnOrganizationChanged += HandleOrgChange;
    }

    private async Task LoadOrganizations()
    {
        availableOrgs = await UserContext.GetAvailableOrganizationsAsync();
        currentOrg = await UserContext.GetActiveOrganizationAsync();
    }

    private async Task SwitchOrg(int orgId)
    {
        await UserContext.SwitchOrganizationAsync(orgId.ToString());

        // Redirect to appropriate dashboard based on org type
        var org = availableOrgs.First(o => o.Id == orgId);
        var url = org.Type switch
        {
            OrganizationType.Residential => "/residential/dashboard",
            OrganizationType.MultiUnit => "/multiunit/dashboard",
            OrganizationType.Commercial => "/commercial/dashboard",
            _ => "/"
        };

        NavigationManager.NavigateTo(url, forceLoad: true); // Full page refresh
    }

    private void HandleOrgChange()
    {
        InvokeAsync(async () =>
        {
            await LoadOrganizations();
            StateHasChanged();
        });
    }

    public void Dispose()
    {
        UserContext.OnOrganizationChanged -= HandleOrgChange;
    }
}
```

---

## 9. State-Specific Compliance Rules

### Compliance Service

```csharp
// Services/Shared/StateComplianceService.cs
public class StateComplianceService
{
    public TenantJourneyConfig GetJourneyForState(string state, OrganizationType orgType)
    {
        return state switch
        {
            "CA" => new CaliforniaTenantJourney(orgType),
            "TX" => new TexasTenantJourney(orgType),
            "NY" => new NewYorkTenantJourney(orgType),
            _ => new DefaultTenantJourney(orgType)
        };
    }

    public LeaseTerms GetDefaultLeaseTerms(string state, OrganizationType orgType)
    {
        var journey = GetJourneyForState(state, orgType);
        return new LeaseTerms
        {
            SecurityDepositMaxMonths = journey.SecurityDepositMaxMonths,
            MinimumNoticeDays = journey.MinimumNoticeDays,
            LateFeeGracePeriodDays = journey.LateFeeGracePeriodDays,
            RequiredDisclosures = journey.RequiredDisclosures
        };
    }
}

// Models/Shared/TenantJourneyConfig.cs
public abstract class TenantJourneyConfig
{
    protected OrganizationType OrgType { get; }

    protected TenantJourneyConfig(OrganizationType orgType)
    {
        OrgType = orgType;
    }

    public abstract int SecurityDepositMaxMonths { get; }
    public abstract int MinimumNoticeDays { get; }
    public abstract bool RequiresRentControlCompliance { get; }
    public abstract List<string> RequiredDisclosures { get; }
    public abstract int LateFeeGracePeriodDays { get; }
    public abstract int MaximumLateFeePercentage { get; }
}

public class CaliforniaTenantJourney : TenantJourneyConfig
{
    public CaliforniaTenantJourney(OrganizationType orgType) : base(orgType) { }

    public override int SecurityDepositMaxMonths => OrgType == OrganizationType.MultiUnit ? 2 : 2;
    public override int MinimumNoticeDays => 30;
    public override bool RequiresRentControlCompliance => OrgType == OrganizationType.MultiUnit;
    public override List<string> RequiredDisclosures => new()
    {
        "Lead Paint Disclosure",
        "Bedbug Notice",
        "Mold Disclosure",
        "Smoke Detector Compliance",
        "Water Conservation Notice"
    };
    public override int LateFeeGracePeriodDays => 3;
    public override int MaximumLateFeePercentage => 10; // CA Civil Code limits
}

public class TexasTenantJourney : TenantJourneyConfig
{
    public TexasTenantJourney(OrganizationType orgType) : base(orgType) { }

    public override int SecurityDepositMaxMonths => 3; // TX has no legal limit
    public override int MinimumNoticeDays => 30;
    public override bool RequiresRentControlCompliance => false; // TX bans rent control
    public override List<string> RequiredDisclosures => new()
    {
        "Lead Paint Disclosure",
        "Security Device Information",
        "Previous Flooding"
    };
    public override int LateFeeGracePeriodDays => 2;
    public override int MaximumLateFeePercentage => 12;
}
```

---

## 10. Migration Strategy

### Phase 1: Foundation - Organization Type

**Goal:** Enable organization type selection without changing existing functionality

**Steps:**

1. Add `Type`, `TotalUnits`, `TotalBuildings`, `PrimaryState` to Organization model
2. Create migration
3. Update existing organizations to `Residential` (default)
4. Update organization create/edit UI to include type selection
5. Test: Verify existing functionality still works

**Database Changes:**

```sql
ALTER TABLE Organizations ADD COLUMN Type INTEGER NOT NULL DEFAULT 1;
ALTER TABLE Organizations ADD COLUMN TotalUnits INTEGER NULL;
ALTER TABLE Organizations ADD COLUMN TotalBuildings INTEGER NULL;
ALTER TABLE Organizations ADD COLUMN PrimaryState TEXT NULL;
```

**Deliverables:**

- Updated Organization model
- Migration script
- Organization management UI updated
- Existing data migrated

---

### Phase 2: User-Organization Relationship

**Goal:** Enable users to belong to multiple organizations

**Steps:**

1. Create `UserOrganization` model
2. Add `ActiveOrganizationId` to ApplicationUser
3. Create migration
4. Migrate existing data: Create UserOrganization records for all users
5. Update `UserContextService` with switching logic
6. Create organization switcher UI component
7. Test: User can switch between organizations

**Database Changes:**

```sql
CREATE TABLE UserOrganizations (
    Id INTEGER PRIMARY KEY,
    UserId TEXT NOT NULL,
    OrganizationId TEXT NOT NULL,
    Role TEXT NOT NULL,
    IsPrimary INTEGER NOT NULL DEFAULT 0,
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
    FOREIGN KEY (OrganizationId) REFERENCES Organizations(Id)
);

ALTER TABLE AspNetUsers ADD COLUMN ActiveOrganizationId TEXT NULL;
```

**Deliverables:**

- UserOrganization model
- Enhanced UserContextService
- Organization switcher component
- Migration script
- Data migration for existing users

---

### Phase 3: Multi-Unit Domain Models

**Goal:** Create separate domain models for multi-unit properties

**Steps:**

1. Create `ApartmentComplex` model
2. Create `ApartmentBuilding` model
3. Create `ApartmentUnit` model
4. Create `ApartmentLease` model
5. Create `ApartmentTenant` model
6. Update ApplicationDbContext with new DbSets
7. Create migrations
8. Test: Can create multi-unit records in database

**Database Tables Created:**

- `ApartmentComplexes`
- `ApartmentBuildings`
- `ApartmentUnits`
- `ApartmentLeases`
- `ApartmentTenants`

**Note:** At this phase, existing residential functionality is unchanged. New tables exist but aren't used yet.

**Deliverables:**

- All multi-unit domain models
- Migration scripts
- DbContext updated
- Unit tests for models

---

### Phase 4: Multi-Unit Services

**Goal:** Create business logic layer for multi-unit operations

**Steps:**

1. Create `MultiUnitPropertyService`
2. Create `ApartmentUnitService`
3. Create `ApartmentLeaseService`
4. Register services in DI container
5. Create unit tests for service methods
6. Test: Services can CRUD multi-unit records

**Deliverables:**

- MultiUnitPropertyService
- ApartmentUnitService
- ApartmentLeaseService
- Service unit tests

---

### Phase 5: Multi-Unit UI

**Goal:** Build complete UI for multi-unit property management

**Steps:**

1. Create `/MultiUnitManagement` folder structure
2. Build complex management pages:
   - Index.razor (list complexes)
   - Create.razor (add new complex)
   - View.razor (complex dashboard)
3. Build unit management pages:
   - Index.razor (unit grid)
   - UnitDetails.razor
   - BulkActions.razor
4. Build lease pages
5. Create MultiUnitLayout and MultiUnitNavMenu
6. Add route-based layout selection
7. Test: Full multi-unit workflow works

**Deliverables:**

- Complete multi-unit UI
- MultiUnitLayout
- MultiUnitNavMenu
- Route-based layout system
- Integration tests

---

### Phase 6: Residential Domain Separation (Optional)

**Goal:** Create dedicated residential domain models

**Steps:**

1. Create `ResidentialProperty` model
2. Create `ResidentialLease` model
3. Create `ResidentialTenant` model
4. Create migrations
5. Create `ResidentialPropertyService`
6. Build `/ResidentialManagement` UI
7. Data migration: Move existing Property/Lease/Tenant to Residential tables

**Note:** This phase is optional if existing models work well for residential. Can be deferred.

---

### Phase 7: Shared Financial Infrastructure

**Goal:** Implement shared invoice/payment system

**Steps:**

1. Decide on approach (Polymorphic vs Separate vs Hybrid)
2. Update Invoice/Payment models
3. Update DocumentService to handle both property types
4. Create shared financial UI components
5. Test: Invoices work across both residential and multi-unit

**Deliverables:**

- Updated Invoice/Payment models
- Financial services
- Shared financial UI components

---

### Phase 8: State Compliance Rules

**Goal:** Implement state-specific business rules

**Steps:**

1. Create `StateComplianceService`
2. Create `TenantJourneyConfig` hierarchy
3. Implement state-specific journey classes (CA, TX, NY, etc.)
4. Integrate with lease creation workflows
5. Add compliance validation
6. Test: State rules are enforced correctly

**Deliverables:**

- StateComplianceService
- State-specific journey configs
- Compliance validation
- Documentation of state-specific rules

---

## 11. Open Questions & Decisions Needed

### Critical Decisions

1. **Shared Financial Models Approach**

   - [ ] Option A: Polymorphic (PropertyType + PropertyId)
   - [ ] Option B: Separate tables per domain
   - [ ] Option C: Hybrid (shared base + extensions)
   - **Impact:** Affects invoice/payment/document schema and queries

2. **Document Attachments**

   - How do documents reference units vs properties vs leases?
   - Same polymorphic pattern as invoices?
   - **Current system:** EntityType + EntityId

3. **Residential Domain Separation**

   - Keep using existing Property/Lease/Tenant models for residential?
   - Or create dedicated ResidentialProperty/ResidentialLease/ResidentialTenant?
   - **Trade-off:** Migration effort vs clean separation

4. **Mixed Portfolio Organizations**

   - How to handle OrgType = Mixed?
   - Show both UIs? Merged UI?
   - **Use case:** Owner expanding from residential to multi-unit

5. **Cross-Organization Reporting**
   - Does Joe need to see consolidated financials across all LLCs?
   - Separate reports per org, or global dashboard?

### Technical Questions

1. **Migration of Existing Data**

   - How to handle current properties with UnitNumber populated?
   - Auto-convert to ApartmentUnit records?
   - Or leave as-is and only use new models for new complexes?

2. **Performance Considerations**

   - Index strategy for unit searches (ComplexId + UnitNumber)
   - Caching for organization switcher
   - Bulk operations for 300+ unit complexes

3. **Security & Authorization**

   - Property manager role scoped to organization?
   - Can manager A access manager B's properties in same org?
   - Cross-organization permissions?

4. **UI State Management**
   - Full page reload on org switch, or preserve some state?
   - Blazor circuit isolation per organization?

---

## 12. Benefits of This Architecture

### Separation of Concerns

✅ Each property type has its own domain model  
✅ Business logic tailored to operational reality  
✅ No UI complexity - residential users never see multi-unit features  
✅ Different workflows for different business types

### Flexibility

✅ Easy to add Commercial, Student Housing, etc. later  
✅ State-specific rules per domain  
✅ Can optimize queries per property type  
✅ Different reporting for each business type

### Scalability

✅ Same database - no deployment/backup complexity  
✅ Supports large multi-unit complexes (300+ units)  
✅ Efficient unit-level queries  
✅ Can add caching per domain

### User Experience

✅ Simple UI for residential managers  
✅ Powerful UI for multi-unit managers  
✅ Easy org switching  
✅ Role-based access per organization

### Shared Infrastructure

✅ Invoices work across all property types  
✅ Payments consolidated  
✅ Documents shared  
✅ User management unified

---

## 13. Risks & Mitigation

### Risk: Data Migration Complexity

**Mitigation:**

- Phase migrations carefully
- Keep legacy tables during transition
- Provide rollback scripts
- Test with production copy

### Risk: UI/UX Confusion

**Mitigation:**

- Clear visual distinction between UIs
- Organization type badge always visible
- Confirmation prompts on org switching
- User education/documentation

### Risk: Performance with Large Complexes

**Mitigation:**

- Database indexes on key fields
- Pagination for unit lists
- Caching for complex metadata
- Background jobs for bulk operations

### Risk: Over-Engineering

**Mitigation:**

- Build incrementally (phases)
- Start with multi-unit only
- Keep residential simple initially
- User feedback at each phase

---

## 14. Success Metrics

### Phase 1-2 Success

- [ ] User can create organizations with types
- [ ] User can switch between organizations
- [ ] Existing functionality unaffected
- [ ] No performance degradation

### Phase 3-5 Success (Multi-Unit)

- [ ] Can create complex with 100+ units
- [ ] Unit search returns results in <1 second
- [ ] Occupancy report accurate
- [ ] Lease creation workflow intuitive

### Phase 6-7 Success (Residential Separation)

- [ ] Residential users unaware of multi-unit features
- [ ] No data leakage between domains
- [ ] Financial reports work across both types

### Overall Success

- [ ] Joe Smith manages both LLCs from single account
- [ ] Property managers see only relevant UI
- [ ] State compliance rules enforced
- [ ] <2 second page load times
- [ ] Zero data loss during migration

---

## 15. Timeline Estimate

**Phase 1:** 1-2 weeks (Organization type)  
**Phase 2:** 1-2 weeks (User-org relationship)  
**Phase 3:** 2-3 weeks (Multi-unit models)  
**Phase 4:** 2-3 weeks (Multi-unit services)  
**Phase 5:** 4-6 weeks (Multi-unit UI)  
**Phase 6:** 2-3 weeks (Residential separation - optional)  
**Phase 7:** 2-3 weeks (Shared financials)  
**Phase 8:** 2-3 weeks (State compliance)

**Total:** 16-25 weeks (4-6 months) for full implementation

---

## 16. Next Steps

### Immediate (Before Implementation)

1. **Decide on shared financial model approach** (Options A/B/C)
2. **Validate multi-unit workflow requirements** with real-world use cases
3. **Review state-specific compliance needs** for primary states
4. **Confirm residential separation strategy**

### When Ready to Proceed

1. Create feature branch
2. Implement Phase 1 (Organization type)
3. Deploy to staging
4. Gather feedback
5. Iterate on design before Phase 2

---

## Appendix A: Example Queries

### Multi-Unit Queries

```csharp
// Get all vacant units in a complex
var vacantUnits = await _dbContext.ApartmentUnits
    .Include(u => u.Complex)
    .Where(u => u.ComplexId == complexId && u.IsAvailable)
    .OrderBy(u => u.Floor)
    .ThenBy(u => u.UnitNumber)
    .ToListAsync();

// Complex occupancy rate
var complex = await _dbContext.ApartmentComplexes
    .Include(c => c.Units)
    .FirstOrDefaultAsync(c => c.Id == complexId);

var occupancyRate = (decimal)complex.Units.Count(u => !u.IsAvailable) / complex.TotalUnits * 100;

// Units expiring in next 60 days
var expiringUnits = await _dbContext.ApartmentUnits
    .Include(u => u.CurrentLease)
    .Where(u => u.ComplexId == complexId
             && u.CurrentLease != null
             && u.CurrentLease.EndDate <= DateTime.Today.AddDays(60))
    .ToListAsync();
```

### Cross-Domain Queries

```csharp
// All invoices for an organization (polymorphic)
var invoices = await _dbContext.Invoices
    .Where(i => i.OrganizationId == orgId)
    .OrderByDescending(i => i.DueDate)
    .ToListAsync();

// Organization revenue across all property types
var residentialRevenue = await _dbContext.ResidentialLeases
    .Where(l => l.OrganizationId == orgId && l.IsActive)
    .SumAsync(l => l.MonthlyRent);

var multiUnitRevenue = await _dbContext.ApartmentUnits
    .Include(u => u.CurrentLease)
    .Where(u => u.OrganizationId == orgId && u.CurrentLease != null)
    .SumAsync(u => u.MonthlyRent);

var totalRevenue = residentialRevenue + multiUnitRevenue;
```

---

## Appendix B: UI Mockup Concepts

### Multi-Unit Dashboard

```
┌─────────────────────────────────────────────────┐
│ Oak Ridge Apartments                  🔄 Switch │
├─────────────────────────────────────────────────┤
│  Occupancy: 285/300 (95%)     Revenue: $342K    │
│  ░░░░░░░░░░░░░░░░░░░ 95%      ↑ 3% vs last mo  │
├─────────────────────────────────────────────────┤
│  Vacant Units: 15     Expiring: 28 (60 days)    │
│  [View All]           [Process Renewals]        │
├─────────────────────────────────────────────────┤
│  Work Orders: 12 Open    Avg Close: 2.3 days    │
│  [View Queue]            [Create Work Order]    │
└─────────────────────────────────────────────────┘
```

### Unit Grid View

```
Building A - Floor 3
┌────┬────┬────┬────┬────┬────┬────┬────┬────┬────┐
│301 │302 │303 │304 │305 │306 │307 │308 │309 │310 │
│ ✓  │ ✓  │ □  │ ✓  │ ⚠️  │ ✓  │ □  │ ✓  │ ✓  │ ✓  │
└────┴────┴────┴────┴────┴────┴────┴────┴────┴────┘
Legend: ✓ Occupied  □ Vacant  ⚠️ Notice Given
```

---

**Document Version:** 1.0  
**Last Updated:** November 28, 2025  
**Status:** Awaiting Decisions on Open Questions
