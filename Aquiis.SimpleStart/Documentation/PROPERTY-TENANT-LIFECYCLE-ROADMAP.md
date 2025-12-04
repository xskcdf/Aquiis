# Property & Tenant Lifecycle Implementation Roadmap

## Overview

This roadmap outlines the complete implementation of the Property & Tenant Lifecycle system, including property status management, application workflow, lease offers, security deposits, and investment dividend distribution.

---

## 🚀 Implementation Phases

### **Phase 1: Foundation - Property Status & Enums** ✅ **COMPLETED**

**Goal:** Add property status tracking and all new enums

**Status:** Complete

**Tasks:**

- [x] Plan documented
- [x] Add all new enumfs to `ApplicationSettings.cs`:
  - `PropertyStatus`
  - Enhanced `ProspectStatus`
  - Enhanced `ApplicationStatus`
  - `LeaseStatus`
  - `DepositDispositionStatus`
  - `DividendPaymentMethod`
  - `DividendPaymentStatus`
- [x] Update `ApplicationConstants.PropertyStatuses` with new status values
- [x] Add `Status` property to `Property` model (string field, not enum)
- [x] Create migration (`AddPropertyStatusField`)
- [x] Apply migration to database
- [x] Update Property Index page to use new status field
- [x] Add status badge component for visual indication (color-coded badges)
- [x] Update Property Create page with status dropdown
- [x] Update Property Edit page with status dropdown
- [x] Test and verify build

**Implementation Notes:**

- Status stored as `string` (max 50 chars) using `ApplicationConstants.PropertyStatuses.*` constants
- Enums in `ApplicationSettings.cs` are for reference/type safety, NOT used in database
- Status badge colors: Available=green, ApplicationPending=info, LeasePending=warning, Occupied=danger
- Backward compatibility maintained with existing code
- Create/Edit pages include full status dropdown with all 6 status options
- Default status for new properties: "Available"

**Completed:** November 30, 2025

---

### **Phase 2: Application Fees & Enhanced Applications** ✅ **COMPLETED**

**Goal:** Track application fees and implement expiration logic

**Status:** Complete

**Tasks:**

- [x] Add `Expired` and `Withdrawn` status to `ApplicationConstants.ApplicationStatuses`
- [x] Add `ApplicationFeePaymentMethod` field to `RentalApplication`
- [x] Add `ExpiresOn` field to `RentalApplication`
- [x] Add application fee settings to `OrganizationSettings`:
  - `ApplicationFeeEnabled`, `DefaultApplicationFee`, `ApplicationExpirationDays`
- [x] Update `CreateRentalApplicationAsync` to set fee and expiration from settings
- [x] Create scheduled job in `ScheduledTaskService` to auto-expire old applications
- [x] Update application view UI to show payment method and expiration date
- [x] Create and apply migration: `AddApplicationFeeEnhancements`

**Implementation Notes:**

- Default application fee: $50.00 (configurable per organization)
- Default expiration: 30 days from submission (configurable per organization)
- Payment method field tracks how fee was paid (string, max 50 chars)
- Expiration runs daily via `ScheduledTaskService.ExpireOldApplications()`
- Auto-expires applications in Submitted/UnderReview/Screening status past ExpiresOn date
- UI shows expiration date with badges: "Expired" (red), "Expires Soon" (yellow, <7 days)
- Payment info shows paid date and method when fee is paid

**Completed:** November 30, 2025

---

### **Phase 3: Prospect-to-Tenant Conversion** ✅ **COMPLETED**

**Goal:** Implement lease offer → acceptance → tenant creation workflow

**Status:** Complete

**Tasks:**

- [x] Add `ProspectiveTenantId` to `Tenant` model
- [x] Add enhanced lease status fields to `Lease`:
  - `OfferedOn`, `SignedOn`, `DeclinedOn`
  - `ExpiresOn` (30 days from offer)
- [x] Create `TenantConversionService`
- [x] Build lease offer generation page
- [x] Build lease acceptance page (with signature audit)
- [x] Implement auto-property status updates
- [x] Add new status constants for lease workflow

**Implementation Notes:**

- Created `GenerateLeaseOffer.razor` page for generating lease offers from approved applications
- Created `AcceptLease.razor` page for lease acceptance with full audit trail (IP, timestamp)
- TenantConversionService handles prospect-to-tenant conversion with duplicate prevention
- Property status auto-updates: Available → ApplicationPending → LeasePending → Occupied
- Lease offers expire in 30 days from `OfferedOn` date
- Competing applications auto-denied when lease offer generated
- Signature audit includes: timestamp, IP address, user ID, payment method
- Added status constants: LeaseOffered, LeaseAccepted, LeaseDeclined to ProspectiveStatuses and ApplicationStatuses
- Added Offered and Declined to LeaseStatuses

**Completed:** November 30, 2025

---

### **Phase 4: Security Deposits** ✅ **COMPLETED**

**Goal:** Full security deposit lifecycle

**Status:** Complete

**Tasks:**

- [x] Create `SecurityDeposit` model with all fields
- [x] Add security deposit collection to lease signing flow
- [x] Build security deposit management UI
- [x] Add deposit settings to `OrganizationSettings`
- [x] Create move-out disposition workflow

**Implementation Notes:**

- Created `SecurityDeposit` entity with complete lifecycle tracking:
  - Collection tracking: Amount, DateReceived, PaymentMethod, TransactionReference
  - Status management: Held, Released, Refunded, Forfeited, PartiallyRefunded
  - Investment pool tracking: InInvestmentPool, PoolEntryDate, PoolExitDate
  - Refund disposition: RefundAmount, DeductionsAmount, DeductionsReason
- Security deposit collection integrated into `AcceptLease.razor` workflow
- Created `SecurityDepositService` with full CRUD and lifecycle methods
- Built 5 UI pages in `Features/PropertyManagement/SecurityDeposits/Pages/`:
  - SecurityDeposits.razor (main list view)
  - InvestmentPools.razor (pool management)
  - ViewInvestmentPool.razor (pool details)
  - RecordPoolPerformance.razor (record earnings)
  - CalculateDividends.razor (dividend calculation)
- Added OrganizationSettings fields: SecurityDepositInvestmentEnabled, AutoCalculateSecurityDeposit, SecurityDepositMultiplier
- Created migration: `20251201142701_AddSecurityDepositModels`
- Added navigation menu entry for Security Deposits
- Service registered in Program.cs

**Completed:** December 1, 2025

---

### **Phase 5: Investment Pool & Dividends** ✅ **COMPLETED**

**Goal:** Security deposit investment tracking and dividend distribution

**Status:** Complete

**Tasks:**

- [x] Create `SecurityDepositInvestmentPool` model
- [x] Create `SecurityDepositDividend` model
- [x] Build annual pool calculation service
- [x] Build dividend distribution service (with pro-rating)
- [x] Create admin UI for pool management
- [x] Create tenant dashboard to view dividends
- [x] Implement tenant choice (credit vs. check)
- [x] Build scheduled job for year-end processing

**Implementation Notes:**

- Created `SecurityDepositInvestmentPool` entity with annual tracking:
  - Year-based performance: StartingBalance, EndingBalance, TotalEarnings, ReturnRate
  - Profit/loss split: OrganizationSharePercentage (default 20%), OrganizationShare, TenantShareTotal
  - Dividend distribution: ActiveLeaseCount, DividendPerLease, DividendsCalculatedOn, DividendsDistributedOn
  - Status workflow: Open → Calculated → Distributed → Closed
- Created `SecurityDepositDividend` entity with per-lease tracking:
  - Amount calculations: BaseDividendAmount, ProrationFactor, DividendAmount
  - Tenant choice: PaymentMethod (Pending/LeaseCredit/Check), ChoiceMadeOn
  - Payment tracking: PaymentProcessedOn, PaymentReference, MailingAddress
  - Pro-ration support: MonthsInPool for mid-year move-ins
- Implemented dividend calculation algorithm with pro-rating for mid-year move-ins
- Added investment pool methods to SecurityDepositService:
  - GetOrCreateInvestmentPoolAsync(), RecordInvestmentPerformanceAsync()
  - CalculateDividendsAsync() with automatic pro-ration
  - RecordDividendChoiceAsync(), ProcessDividendPaymentAsync()
- Business rules implemented:
  - Organization takes 20% of profits (configurable)
  - Losses absorbed by organization (no negative dividends)
  - Pro-ration: months in pool / 12
  - Dividend = TenantShareTotal / ActiveLeaseCount × ProrationFactor
- Added OrganizationSettings: DividendDistributionMonth, AllowTenantDividendChoice, DefaultDividendPaymentMethod
- UI pages handle complete investment pool workflow
- Created comprehensive workflow documentation: SECURITY-DEPOSIT-WORKFLOW.md

**Completed:** December 1, 2025

---

### **Phase 5.5: Multi-Organization Management** ⬅️ **NEXT**

**Goal:** Enable account owners to create and manage multiple organizations with context switching and granular role-based access control

**Status:** Not Started

**Important Architectural Decision - Role Consolidation:**

This phase **replaces the current ASP.NET Identity global roles** with **organization-scoped roles** in the UserOrganizations table. This eliminates role duplication and enables proper multi-tenant role management where a user can have different roles in different organizations.

**Current System (To Be Replaced):**

- Global ASP.NET Identity Roles: SuperAdministrator, Administrator, PropertyManager, Tenant, User, Guest
- Roles stored in AspNetUserRoles table (global, not org-specific)
- Problem: Cannot have different roles per organization

**New System (Phase 5.5):**

- Organization-scoped roles in UserOrganizations table
- Roles: Owner, Administrator, Manager, Staff
- User can be "Administrator" in Org A and "Staff" in Org B
- All authorization checks query UserOrganizations, not Identity roles

**Migration Strategy:**

1. Keep ASP.NET Identity roles table (required by Identity framework)
2. Stop using Identity roles for authorization (deprecate in code)
3. Use UserOrganizations.Role for all permission checks
4. Remove old role constants from ApplicationConstants after migration
5. Update all `IsInRoleAsync()` calls to use `GetCurrentOrganizationRoleAsync()`

**Tasks:**

- [ ] Create `Organization` entity with settings
- [ ] Create `UserOrganization` junction table with role-based assignments
- [ ] Add `ActiveOrganizationId` to `ApplicationUser` for context tracking
- [ ] Migrate existing data (create Organization records)
- [ ] Create `OrganizationService` with CRUD operations
- [ ] Update `UserContextService` to support context switching
- [ ] Build organization management UI (create/edit/list)
- [ ] Build organization switcher component (navbar dropdown)
- [ ] Update settings page to be organization-specific
- [ ] **Deprecate ASP.NET Identity global roles (SuperAdministrator, Administrator, etc.)**
- [ ] **Update all authorization code to use UserOrganizations roles instead of Identity roles**
- [ ] Add authorization guards with role-based permissions (Owner/Administrator/Manager/Staff)
- [ ] Build user-organization management UI (grant/revoke access, change roles)
- [ ] Implement permission matrix for feature-level access control
- [ ] Test multi-org data isolation

**Business Rules:**

1. **Account Owner** (UserId == OrganizationId from registration):

   - Can create multiple organizations
   - Can switch between organizations using dropdown
   - Has full access to all their organizations (Owner role in UserOrganizations)
   - Each organization has independent settings, properties, tenants, etc.
   - **Can delegate access**: Promote users to Administrator or grant multi-org access

2. **Administrator Users** (Promoted by Account Owner):

   - Can have access to multiple organizations (via UserOrganizations assignments)
   - Can switch between assigned organizations
   - Has delegated owner-level permissions across assigned orgs
   - Full access to all property management and administration features
   - Can manage organization settings (late fees, dividend rules, etc.)
   - Can modify data retention policies
   - Cannot create new organizations (only Owner can)
   - Cannot delete organizations or organization data (only Owner can)
   - Cannot backup organization data (only Owner can)
   - Example: Joe promotes Sarah to Administrator for "CA Properties" and "TX Rentals"

3. **Property Manager Users** (Standard property management role):

   - Can have access to single OR multiple organizations (via UserOrganizations)
   - If assigned to multiple orgs, can switch between them
   - Full access to all property management features:
     - Properties, tenants, leases, applications
     - Inspections, maintenance, documents
     - Invoices, payments, security deposits
   - Cannot access administration features:
     - Cannot manage organization settings
     - Cannot manage users
     - Cannot modify data retention policies
     - Cannot access backup/restore features
   - Example: Property manager handles day-to-day operations for 2 of Joe's 3 companies

4. **User Role** (Limited access level):

   - Can have access to single OR multiple organizations (via UserOrganizations)
   - If assigned to multiple orgs, can switch between them
   - Access to select features (TBD - will be defined during implementation)
   - Likely limited to:
     - Viewing properties, tenants, leases (read-only)
     - Creating/viewing maintenance requests
     - Basic reporting
   - Cannot create/edit/delete core entities
   - Cannot access any administration features
   - Example: Assistant or limited staff member with view/report access

5. **Permission Levels** (per UserOrganizations entry):

   - **Owner** (Super Administrator):

     - Full control over all organizations they create
     - Create/delete organizations
     - Backup/restore organization data
     - Delete organization data
     - All Administrator permissions plus data management

   - **Administrator**:

     - Delegated owner access (all features except org creation/deletion/data management)
     - Manage organization settings
     - Modify data retention policies
     - Full property management access
     - Full administration features access
     - Manage users and grant/revoke organization access

   - **PropertyManager**:

     - Full property management features
     - Properties, tenants, leases, applications, inspections
     - Maintenance, documents, invoices, payments, deposits
     - Cannot access administration/settings

   - **User**:
     - Limited feature access (TBD)
     - Likely view-only or basic operations
     - No settings, no user management

   _Note: These replace the old global roles (SuperAdministrator, Administrator, PropertyManager, User)_

6. **Organization Isolation**:

   - All entities scoped by OrganizationId
   - Settings (late fees, dividend rules, etc.) per organization
   - Example: "California Properties LLC" vs "Texas Rentals Inc." with different state regulations
   - User sees only data for organizations they're assigned to

7. **Context Switching**:
   - Any user with multiple org assignments can switch via navbar dropdown
   - `ActiveOrganizationId` stored in ApplicationUser
   - All queries automatically filtered to active organization
   - UI shows current organization badge/indicator with role
   - Dropdown shows only organizations user has access to (not all owner's orgs)

**Database Schema:**

```sql
-- New Organization entity
CREATE TABLE Organizations (
    Id INTEGER PRIMARY KEY,
    OwnerId TEXT NOT NULL,              -- UserId of account owner
    Name TEXT(200) NOT NULL,            -- "California Properties LLC"
    DisplayName TEXT(200),              -- Short name for UI
    State TEXT(2),                      -- US state code (CA, TX, etc.)
    IsActive INTEGER NOT NULL,          -- Soft delete flag
    CreatedOn DATETIME NOT NULL,
    CreatedBy TEXT(100) NOT NULL,
    LastModifiedOn DATETIME,
    LastModifiedBy TEXT(100),
    IsDeleted INTEGER NOT NULL,
    FOREIGN KEY (OwnerId) REFERENCES AspNetUsers(Id)
);

-- Junction table for multi-org user assignments with granular permissions
CREATE TABLE UserOrganizations (
    Id INTEGER PRIMARY KEY,
    UserId TEXT NOT NULL,
    OrganizationId INTEGER NOT NULL,
    Role TEXT(50) NOT NULL,             -- "Owner", "Administrator", "PropertyManager", "User"
    GrantedBy TEXT NOT NULL,            -- UserId who granted access
    GrantedOn DATETIME NOT NULL,        -- When access was granted
    RevokedOn DATETIME,                 -- NULL if active, date if revoked
    IsActive INTEGER NOT NULL,          -- Active assignment flag
    CreatedOn DATETIME NOT NULL,
    CreatedBy TEXT(100) NOT NULL,
    LastModifiedOn DATETIME,
    LastModifiedBy TEXT(100),
    IsDeleted INTEGER NOT NULL,
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
    FOREIGN KEY (OrganizationId) REFERENCES Organizations(Id),
    FOREIGN KEY (GrantedBy) REFERENCES AspNetUsers(Id),
    UNIQUE(UserId, OrganizationId)      -- One role per user per org
);

-- Update ApplicationUser
ALTER TABLE AspNetUsers ADD COLUMN ActiveOrganizationId INTEGER;
ALTER TABLE AspNetUsers ADD CONSTRAINT FK_ActiveOrganization
    FOREIGN KEY (ActiveOrganizationId) REFERENCES Organizations(Id);
```

**First-Time Setup (Registration-Based Initialization):**

1. **First User Registration**:

   - When first user registers (no users in AspNetUsers table)
   - Create ApplicationUser record
   - Create Organization record (Name: "Default Organization", OwnerId: new user's Id)
   - Create UserOrganizations entry (Role: "Owner", GrantedBy: user's Id)
   - Set ApplicationUser.ActiveOrganizationId to new Organization.Id
   - Create default OrganizationSettings for the organization
   - **Disable registration** after first user creation

2. **Subsequent Users**:

   - Added by existing users with Owner/Administrator role
   - Via /administration/users/invite or similar
   - Grant access via UserOrganizations table
   - Registration page shows "Registration disabled - Contact administrator"

3. **Program.cs Startup Changes**:
   - **REMOVE** role seeding code (lines 410-417)
   - **REMOVE** SuperAdmin user creation (lines 420-432)
   - Keep Identity infrastructure (required for authentication)
   - Registration workflow handles first-time setup

**Data Migration Strategy (Existing Installations):**

For each existing user where `UserId == OrganizationId`:

1. Create Organization record with Id = UserId (as integer conversion)
2. Set Organization.OwnerId = UserId
3. Set Organization.Name from user profile or default to "Default Organization"
4. Create UserOrganizations entry with Role = "Owner", GrantedBy = UserId
5. Set ApplicationUser.ActiveOrganizationId = Organization.Id
6. Link existing OrganizationSettings record to new Organization.Id
7. After migration, disable registration functionality

**Service Layer Changes:**

```csharp
// New OrganizationService
public class OrganizationService
{
    // CRUD operations
    Task<Organization> CreateOrganizationAsync(string ownerId, string name, string? state);
    Task<Organization?> GetOrganizationByIdAsync(int organizationId);
    Task<List<Organization>> GetUserOrganizationsAsync(string userId);
    Task<bool> UpdateOrganizationAsync(Organization org);
    Task<bool> DeleteOrganizationAsync(int organizationId);

    // Ownership/permission checks
    Task<bool> IsOwnerAsync(string userId, int organizationId);
    Task<bool> IsAdministratorAsync(string userId, int organizationId);
    Task<bool> CanAccessOrganizationAsync(string userId, int organizationId);
    Task<string?> GetUserRoleForOrganizationAsync(string userId, int organizationId);

    // User-Organization assignment management
    Task<bool> GrantOrganizationAccessAsync(string userId, int organizationId, string role, string grantedBy);
    Task<bool> RevokeOrganizationAccessAsync(string userId, int organizationId);
    Task<bool> UpdateUserRoleAsync(string userId, int organizationId, string newRole);
    Task<List<UserOrganization>> GetOrganizationUsersAsync(int organizationId);
    Task<List<UserOrganization>> GetUserAssignmentsAsync(string userId);
}

// Updated UserContextService
public class UserContextService
{
    // Existing methods...
    Task<string?> GetOrganizationIdAsync(); // Now returns ActiveOrganizationId

    // New methods
    Task<bool> SwitchOrganizationAsync(int organizationId);
    Task<List<Organization>> GetAccessibleOrganizationsAsync();
    Task<bool> IsAccountOwnerAsync();
    Task<Organization?> GetActiveOrganizationAsync();
    Task<string?> GetCurrentOrganizationRoleAsync(); // Role in active org
    Task<bool> HasPermissionAsync(string permission); // Feature-level permission check
}
```

**UI Components:**

1. **Organization Switcher** (Navbar):

   ```razor
   @if (accessibleOrganizations.Count > 1)
   {
       <div class="dropdown">
           <button class="btn btn-outline-secondary dropdown-toggle">
               <i class="bi bi-building"></i> @currentOrg?.DisplayName
               <span class="badge bg-info ms-2">@currentRole</span>
           </button>
           <ul class="dropdown-menu">
               @foreach (var orgAccess in accessibleOrganizations)
               {
                   <li>
                       <a @onclick="() => SwitchOrg(orgAccess.OrganizationId)">
                           @orgAccess.Organization.Name
                           <small class="text-muted">(@orgAccess.Role)</small>
                       </a>
                   </li>
               }
               @if (isAccountOwner)
               {
                   <li><hr class="dropdown-divider"></li>
                   <li><a href="/organizations/create">+ New Organization</a></li>
               }
           </ul>
       </div>
   }
   ```

2. **Organization Management Page** (`/organizations`):

   - List all organizations user has access to (with role badges)
   - Create new organization button (Account Owner only)
   - Edit organization details (name, state, settings link)
   - Delete organization (Account Owner only, with confirmation)
   - **NEW: User Management per Organization**

3. **Organization User Management** (`/organizations/{id}/users`):

   - List all users assigned to organization (with roles)
   - Add user to organization (select user, assign role)
   - Change user's role (Owner → Administrator → Manager → Staff)
   - Revoke user's access to organization
   - Shows who granted access and when
   - Only visible to Owner and Administrator roles

4. **Organization Settings Integration**:
   - Update `/settings` to show "Settings for [Organization Name]"
   - Settings are always for the currently active organization
   - Show user's role and permissions in current org
   - Disable settings editing if role is Staff (read-only)

**Authorization Rules:**

```csharp
// BEFORE (Global Identity Roles - Being Deprecated):
var isAdmin = await _userManager.IsInRoleAsync(user, "Administrator");
// Problem: User is Admin everywhere or nowhere

// AFTER (Organization-Scoped Roles - New System):
var userId = await _userContext.GetUserIdAsync();
var activeOrgId = await _userContext.GetOrganizationIdAsync();
var userRole = await _userContext.GetCurrentOrganizationRoleAsync();
// Benefit: User can be "Administrator" in Org A, "Staff" in Org B

// Check permissions based on organization-scoped role
var canAccessOrg = await _organizationService.CanAccessOrganizationAsync(userId, activeOrgId);
if (!canAccessOrg)
{
    throw new UnauthorizedAccessException("User does not have access to this organization");
}

// Feature-level permissions by organization role
switch (userRole)
{
    case "Owner":
        // Full data sovereignty and access:
        // - Create/delete organizations
        // - Backup organization data
        // - Delete organization data
        // - All Administrator permissions
        break;

    case "Administrator":
        // Delegated owner access (all features except data management):
        // - Manage organization settings
        // - Modify data retention policies
        // - Full property management features
        // - Full administration features
        // - Manage users (grant/revoke organization access)
        // CANNOT: Create/delete orgs, backup data, delete org data
        break;

    case "PropertyManager":
        // Full property management features:
        // - Properties, tenants, leases, applications
        // - Inspections, maintenance, documents
        // - Invoices, payments, security deposits
        // CANNOT: Access administration features, change settings, manage users
        break;

    case "User":
        // Limited feature access (TBD):
        // - View-only or basic operations
        // - Likely: view properties/tenants/leases, create maintenance requests
        // CANNOT: Edit core entities, access administration, change settings
        break;
}

// Example permission check
if (!await _userContext.HasPermissionAsync("settings.edit"))
{
    // User's organization role doesn't allow editing settings
    return Forbid();
}
```

**Role Constants (New):**

```csharp
// Add to ApplicationConstants.cs
public static class OrganizationRoles
{
    public const string Owner = "Owner";
    public const string Administrator = "Administrator";
    public const string Manager = "Manager";
    public const string Staff = "Staff";

    public static IReadOnlyList<string> AllRoles { get; } = new List<string>
    {
        Owner,
        Administrator,
        Manager,
        Staff
    };
}

// DEPRECATED - Remove after Phase 5.5 migration:
// DefaultSuperAdminRole, DefaultAdminRole, DefaultPropertyManagerRole, etc.
```

**Permission Matrix:**
| Feature | Owner | Administrator | PropertyManager | User |
| -------------------- | ----- | ----------------- | ----------------- | ----------------- |
| Create Organization | ✅ | ❌ | ❌ | ❌ |
| Delete Organization | ✅ | ❌ | ❌ | ❌ |
| Backup Org Data | ✅ | ❌ | ❌ | ❌ |
| Delete Org Data | ✅ | ❌ | ❌ | ❌ |
| Edit Org Settings | ✅ | ✅ | ❌ | ❌ |
| Modify Retention Policy | ✅ | ✅ | ❌ | ❌ |
| Manage Users | ✅ | ✅ | ❌ | ❌ |
| Grant Org Access | ✅ | ✅ | ❌ | ❌ |
| Properties (CRUD) | ✅ | ✅ | ✅ | View Only |
| Tenants (CRUD) | ✅ | ✅ | ✅ | View Only |
| Leases (CRUD) | ✅ | ✅ | ✅ | View Only |
| Applications | ✅ | ✅ | ✅ | View Only |
| Inspections | ✅ | ✅ | ✅ | ❌ |
| Maintenance | ✅ | ✅ | ✅ | Create Requests |
| Documents | ✅ | ✅ | ✅ | View Only |
| Invoices/Payments | ✅ | ✅ | ✅ | ❌ |
| Security Deposits | ✅ | ✅ | ✅ | ❌ |
| Investment Pool | ✅ | ✅ | ❌ | ❌ |
| Financial Reports | ✅ | ✅ | ✅ | ❌ |
| Switch Organizations | ✅ | ✅ (if multi-org) | ✅ (if multi-org) | ✅ (if multi-org) |

_Note: Owner has data sovereignty (backup/delete org data), Administrator has operational control (settings/retention policy), PropertyManager has property operations, User has limited access (TBD)_

**Testing Scenarios:**

1. **Account Owner (Joe):**

   - Creates 3 organizations: CA Properties, TX Rentals, FL Estates
   - Switches between all 3 organizations
   - Each org has different settings, properties, tenants

2. **Promoted Administrator (Sarah):**

   - Joe grants Sarah "Administrator" access to CA Properties and TX Rentals
   - Sarah logs in, sees org switcher with 2 orgs
   - Sarah can switch between CA and TX (not FL)
   - Sarah can manage users, edit settings for CA and TX
   - Sarah cannot create new organizations
   - Sarah cannot delete organizations

3. **Multi-Org Manager (Mike):**

   - Joe grants Mike "Manager" access to TX Rentals and FL Estates
   - Mike can switch between TX and FL
   - Mike can manage properties, leases, tenants
   - Mike cannot edit organization settings
   - Mike cannot manage users

4. **Single-Org Staff (Lisa):**

   - Joe grants Lisa "Staff" access to CA Properties only
   - Lisa sees no org switcher (only 1 org)
   - Lisa has view-only access to properties/tenants
   - Lisa cannot edit settings or manage users

5. **Data Isolation:**

   - Sarah switches to CA Properties, sees only CA properties/tenants
   - Sarah switches to TX Rentals, sees different set of properties/tenants
   - Mike cannot see CA Properties data at all
   - Settings changes in TX don't affect CA or FL

6. **Permission Enforcement:**

   - Sarah tries to delete organization → Forbidden
   - Mike tries to edit settings → Forbidden
   - Lisa tries to edit tenant → Forbidden
   - All role-based restrictions enforced server-side

7. **Access Revocation:**
   - Joe revokes Sarah's access to TX Rentals
   - Sarah can only access CA Properties now
   - Sarah's org switcher updates immediately

**Implementation Notes:**

1. **Registration Changes**:

   - Modify Account/Pages/Register.razor to detect first user
   - If first user: Create org + UserOrganizations entry, disable registration
   - If not first user: Show "Registration disabled" message
   - Registration re-enable only via database flag (future: admin setting)

2. **User Invitation Workflow** (for subsequent users):

   - Owner/Administrator creates user manually in /administration/users
   - Sends invitation email with temporary password
   - New user logs in, forced to change password
   - Access granted via UserOrganizations assignments

3. **Identity Role Constants** (ApplicationConstants.cs):
   - Keep existing role constants for backward compatibility
   - Add new OrganizationRoles class (Owner, Administrator, PropertyManager, User)
   - Old role constants become deprecated (not used for authorization)

**Estimated Time:** 6-8 hours

**Dependencies:**

- Completes before Phase 6 (Workflow Services need correct org context)
- Leverages existing OrganizationId infrastructure
- No changes needed to existing entities (already have OrganizationId)

**Navigation & Authorization Migration Strategy:**

The current system uses ASP.NET Identity roles in two key places that must be migrated:

1. **NavMenu.razor Navigation** - Currently uses `<AuthorizeView Roles="Administrator">` and `<AuthorizeView Roles="PropertyManager">`
2. **Page-Level Authorization** - Currently uses `@attribute [Authorize(Roles = "PropertyManager, Administrator")]`

**Migration Approach:**

Create custom authorization components and attributes that check `UserOrganizations` roles instead of Identity roles:

```razor
<!-- NEW: Custom organization-aware authorization component -->
<!-- Create: Shared/Components/OrganizationAuthorizeView.razor -->
@inject UserContextService UserContext

@if (isAuthorized)
{
    @ChildContent
}

@code {
    [Parameter] public string Roles { get; set; } = string.Empty; // "Owner,Administrator"
    [Parameter] public RenderFragment? ChildContent { get; set; }

    private bool isAuthorized = false;

    protected override async Task OnInitializedAsync()
    {
        var currentRole = await UserContext.GetCurrentOrganizationRoleAsync();
        if (!string.IsNullOrEmpty(currentRole))
        {
            var allowedRoles = Roles.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                    .Select(r => r.Trim());
            isAuthorized = allowedRoles.Contains(currentRole, StringComparer.OrdinalIgnoreCase);
        }
    }
}
```

```csharp
// NEW: Custom authorization attribute for pages
// Create: Shared/Authorization/OrganizationAuthorizeAttribute.cs
using Microsoft.AspNetCore.Authorization;

namespace Aquiis.SimpleStart.Shared.Authorization
{
    public class OrganizationAuthorizeAttribute : AuthorizeAttribute
    {
        public OrganizationAuthorizeAttribute(params string[] roles)
        {
            // Store org roles in Policy field to distinguish from Identity roles
            Policy = $"OrgRoles:{string.Join(",", roles)}";
        }
    }
}

// Register authorization handler in Program.cs
builder.Services.AddScoped<IAuthorizationHandler, OrganizationRoleAuthorizationHandler>();

builder.Services.AddAuthorizationCore(options =>
{
    // Organization role policies are created dynamically
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});
```

```csharp
// NEW: Authorization handler
// Create: Shared/Authorization/OrganizationRoleAuthorizationHandler.cs
public class OrganizationRoleAuthorizationHandler : AuthorizationHandler<OrganizationRoleRequirement>
{
    private readonly UserContextService _userContext;

    public OrganizationRoleAuthorizationHandler(UserContextService userContext)
    {
        _userContext = userContext;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OrganizationRoleRequirement requirement)
    {
        var currentRole = await _userContext.GetCurrentOrganizationRoleAsync();

        if (!string.IsNullOrEmpty(currentRole) &&
            requirement.AllowedRoles.Contains(currentRole, StringComparer.OrdinalIgnoreCase))
        {
            context.Succeed(requirement);
        }
    }
}
```

**NavMenu.razor Migration:**

```razor
<!-- BEFORE (Identity Roles): -->
<AuthorizeView Roles="Administrator" Context="administratorContext">
    <Authorized>
        <div class="nav-item px-3">
            <NavLink class="nav-link" href="administration/dashboard">
                <div><span class="bi bi-window-dock"></span>Application</div>
            </NavLink>
        </div>
    </Authorized>
</AuthorizeView>

<AuthorizeView Roles="PropertyManager" Context="pmContext">
    <Authorized>
        <div class="nav-item px-3">
            <NavLink class="nav-link" href="propertymanagement/properties">
                <div><span class="bi bi-house-fill"></span>Properties</div>
            </NavLink>
        </div>
    </Authorized>
</AuthorizeView>

<!-- AFTER (Organization Roles): -->
<OrganizationAuthorizeView Roles="Owner,Administrator" Context="adminContext">
    <div class="nav-item px-3">
        <NavLink class="nav-link" href="administration/dashboard">
            <div><span class="bi bi-window-dock"></span>Application</div>
        </NavLink>
    </div>
</OrganizationAuthorizeView>

<OrganizationAuthorizeView Roles="Owner,Administrator,Manager" Context="pmContext">
    <div class="nav-item px-3">
        <NavLink class="nav-link" href="propertymanagement/properties">
            <div><span class="bi bi-house-fill"></span>Properties</div>
        </NavLink>
    </div>
</OrganizationAuthorizeView>

<!-- Staff users see limited navigation (view-only pages) -->
<OrganizationAuthorizeView Roles="Staff" Context="staffContext">
    <div class="nav-item px-3">
        <NavLink class="nav-link" href="propertymanagement/properties/list">
            <div><span class="bi bi-house-fill"></span>Properties (View)</div>
        </NavLink>
    </div>
</OrganizationAuthorizeView>
```

**Page-Level Authorization Migration:**

```razor
<!-- BEFORE (Identity Roles): -->
@page "/propertymanagement/properties"
@attribute [Authorize(Roles = "PropertyManager")]

<!-- AFTER (Organization Roles): -->
@page "/propertymanagement/properties"
@attribute [OrganizationAuthorize("Owner", "Administrator", "Manager")]
<!-- Staff users get Forbidden error -->

<!-- For view-only pages accessible to Staff: -->
@page "/propertymanagement/properties/view/{id:int}"
@attribute [OrganizationAuthorize("Owner", "Administrator", "Manager", "Staff")]
```

**Role Mapping Table:**

| Old Identity Role  | New Org Role(s)               | Notes                    |
| ------------------ | ----------------------------- | ------------------------ |
| SuperAdministrator | Owner                         | Account creator only     |
| Administrator      | Owner, Administrator          | Full admin access        |
| PropertyManager    | Owner, Administrator, Manager | Property operations      |
| User               | Staff                         | Limited/view-only access |
| Tenant             | (Tenant Portal - Phase 8)     | Not in UserOrganizations |
| Guest              | (Public pages)                | No org assignment needed |

**Migration Checklist:**

- [ ] Create `OrganizationAuthorizeView` component
- [ ] Create `OrganizationAuthorizeAttribute` and handler
- [ ] Update NavMenu.razor (replace 2 `AuthorizeView` sections)
- [ ] Update ~40 page files with new `[OrganizationAuthorize]` attribute:
  - 7 Administration pages
  - 20+ PropertyManagement pages
  - Settings pages
  - Reports pages
- [ ] Test navigation visibility per role (Owner sees all, Staff sees limited)
- [ ] Test page access (403 Forbidden for unauthorized roles)
- [ ] Remove old Identity role checks after verification

**Benefits of New System:**

✅ **Context-aware**: Navigation and permissions change based on active organization  
✅ **Role flexibility**: Sarah sees Admin menu in Org A, Staff menu in Org B  
✅ **Cleaner code**: Single source of truth for permissions (UserOrganizations table)  
✅ **Better UX**: Users see only what they can access in current org context

---

### **Phase 6: Workflow Services & Automation**

**Goal:** Tie it all together with state management

**Status:** Not Started

**Tasks:**

- [ ] Create `ApplicationWorkflowService`:
  - Submit, approve, deny, expire
  - Auto-update property status
  - Auto-deny competing applications
- [ ] Create `LeaseWorkflowService`:
  - Generate offer, accept, decline
  - Trigger tenant conversion
  - Schedule move-in inspection
- [ ] Update `ScheduledTaskService` with new jobs:
  - Application expiration
  - Lease offer expiration
  - Dividend calculation
- [ ] Add workflow notifications/alerts

**Estimated Time:** 6-8 hours

**Dependencies:** Requires Phase 5.5 (org context must work correctly)

---

### **Phase 7: Multi-Lease Support & Refinements**

**Goal:** Handle edge cases and multi-lease scenarios

**Status:** Not Started

**Tasks:**

- [ ] Update all queries to support tenants with multiple leases
- [ ] Build tenant portfolio view (all properties/leases)
- [ ] Handle security deposit per-lease in UI
- [ ] Test all workflows with complex scenarios
- [ ] Add validation rules and business logic guards

**Estimated Time:** 4-5 hours

---

### **Phase 8: Tenant Portal** (Future)

**Goal:** Self-service portal for tenants

**Status:** Not Started

**Tasks:**

- [ ] Dashboard with lease info
- [ ] Investment performance view
- [ ] Dividend history and payment choice
- [ ] Payment portal
- [ ] Maintenance request submission
- [ ] Document access

**Estimated Time:** 12-15 hours

---

## Business Rules Reference

### Property Status Lifecycle

- **Available** → **ApplicationPending** (when application submitted)
- **ApplicationPending** → **LeasePending** (when application approved)
- **LeasePending** → **Occupied** (when lease signed)
- **LeasePending** → **Available** (when lease declined or all apps denied)
- **Occupied** → **Available** (when lease ends/terminated)

### Application Workflow Rules

1. Application fee required per application (non-refundable)
2. Application cannot be processed until payment captured
3. Applications expire after 30 days if not processed
4. First approved application that accepts lease gets the property
5. All other pending applications auto-denied when one is approved

### Lease Workflow Rules

1. Lease offer generated upon application approval
2. Lease offer expires in 30 days if not signed
3. Security deposit must be paid in full upfront at signing
4. Full signature audit trail (IP, timestamp, document version)
5. Unsigned leases after 30 days roll to month-to-month at higher rate

### Security Deposit Investment Rules

1. All deposits pooled into investment account
2. Annual earnings distributed as dividends
3. Organization takes 20% (configurable), remainder to tenants
4. Dividend per lease = (TenantShare / ActiveLeaseCount)
5. Pro-rated for mid-year move-ins
6. Distributed even if tenant has moved out (to forwarding address)
7. Tenant chooses: lease credit OR check payment
8. Losses absorbed by organization (no negative dividends)
9. Multiple leases = multiple separate dividends

### Tenant Conversion Rules

1. Tenant created from ProspectiveTenant data at lease signing
2. Tenant.ProspectiveTenantId links back to prospect history
3. Prospect history retained for compliance/audit
4. Same tenant can have multiple active leases
5. Multiple leases can be in same or different buildings

---

## Progress Tracking

**Total Estimated Time:** 51-65 hours

**Completed Phases:** 5/9 (56%)

**Current Phase:** Phase 5.5 - Multi-Organization Management

**Started:** November 30, 2025

**Phase 1 Completed:** November 30, 2025

**Phase 2 Completed:** November 30, 2025

**Phase 3 Completed:** November 30, 2025

**Phase 4 Completed:** December 1, 2025

**Phase 5 Completed:** December 1, 2025

---

## Notes & Decisions

### Key Architectural Decisions

- Enums stored in `ApplicationSettings.cs` for reference/type safety
- Status/type values stored as string constants in `ApplicationConstants.cs` static classes
- Database fields use `string` type, not enums (e.g., `Property.Status` is string)
- EF Core Migrations for all schema changes
- SQLite database (not SQL Server)
- Migrations stored in `Data/Migrations/`
- **Multi-Organization Model:**
  - Account Owner (UserId == OrganizationId): Can create/manage multiple organizations
  - Staff Users (UserId ≠ OrganizationId): Scoped to single organization
  - Organization context switching via ActiveOrganizationId in ApplicationUser
  - All entities use OrganizationId for data isolation
  - Settings, late fees, dividend rules are per-organization

### Development Approach

- Sequential phase completion for complete feature delivery
- Each phase tested before moving to next
- Migration created at end of each phase

### Dependencies

- Phase 2+ depends on Phase 1 completion
- Phase 5 depends on Phase 4 completion
- **Phase 5.5 (Multi-Org) must complete before Phase 6**
- Phase 6 integrates Phases 2-5 with correct org context
- Phase 7 refines all previous phases
- Phase 8 builds on complete system

```

```
