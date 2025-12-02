# Clean Architecture Refactoring Plan

**Date Started:** December 2, 2025  
**Objective:** Refactor project from mixed feature/technical structure to Clean Architecture with vertical slices

---

## Current Structure (Before)

```
Aquiis.SimpleStart/
├── Components/
│   ├── Account/
│   ├── Administration/
│   │   └── Application/
│   │       └── ApplicationConstants.cs      # ❌ Should be in Core
│   ├── Layout/
│   ├── Pages/
│   ├── PropertyManagement/
│   │   ├── Applications/
│   │   ├── Inspections/
│   │   ├── Leases/
│   │   ├── Maintenance/
│   │   ├── Properties/
│   │   ├── SecurityDeposits/
│   │   │   ├── SecurityDeposit.cs           # ❌ Should be in Core/Entities
│   │   │   ├── SecurityDepositInvestmentPool.cs
│   │   │   ├── SecurityDepositDividend.cs
│   │   │   └── Pages/
│   │   └── Tenants/
│   │       └── Tenant.cs                    # ❌ Should be in Core/Entities
│   └── Shared/
├── Data/
│   ├── ApplicationDbContext.cs              # ❌ Should be in Infrastructure
│   ├── Migrations/                          # ❌ Should be in Infrastructure
│   └── Scripts/
├── Models/                                   # ❌ Should be in Core/Entities
│   ├── BaseModel.cs
│   ├── CalendarEvent.cs
│   ├── Note.cs
│   ├── Property.cs
│   ├── Lease.cs
│   └── ...
├── Services/                                 # ❌ Should be in Application
│   ├── PropertyManagementService.cs
│   ├── SecurityDepositService.cs
│   ├── TenantConversionService.cs
│   └── ...
├── Utilities/
└── wwwroot/
```

---

## Target Structure (After)

```
Aquiis.SimpleStart/
├── Core/                                     # ✅ Domain layer (no dependencies)
│   ├── Entities/                            # All domain models
│   │   ├── BaseModel.cs
│   │   ├── Property.cs
│   │   ├── Tenant.cs
│   │   ├── Lease.cs
│   │   ├── SecurityDeposit.cs
│   │   ├── SecurityDepositInvestmentPool.cs
│   │   ├── SecurityDepositDividend.cs
│   │   ├── CalendarEvent.cs
│   │   ├── Note.cs
│   │   └── ... (all other entities)
│   ├── Interfaces/                          # Repository & service contracts
│   │   ├── Repositories/
│   │   │   ├── IPropertyRepository.cs
│   │   │   └── ISecurityDepositRepository.cs
│   │   └── Services/
│   │       ├── IPropertyManagementService.cs
│   │       └── ISecurityDepositService.cs
│   └── Constants/
│       └── ApplicationConstants.cs
│
├── Infrastructure/                           # ✅ Data access & external services
│   ├── Data/
│   │   ├── ApplicationDbContext.cs
│   │   ├── Migrations/
│   │   └── Repositories/                    # EF implementations
│   │       ├── PropertyRepository.cs
│   │       └── SecurityDepositRepository.cs
│   └── Services/                            # External service implementations
│       ├── EmailService.cs
│       └── FileStorageService.cs
│
├── Application/                              # ✅ Business logic layer
│   ├── Services/                            # Domain services
│   │   ├── PropertyManagementService.cs
│   │   ├── SecurityDepositService.cs
│   │   ├── TenantConversionService.cs
│   │   ├── FinancialReportService.cs
│   │   └── ... (all other services)
│   ├── DTOs/                                # Data transfer objects (future)
│   └── Validators/                          # Business rules (future)
│
├── Features/                                 # ✅ Vertical slices (Blazor UI)
│   ├── PropertyManagement/
│   │   ├── Properties/
│   │   │   ├── Pages/
│   │   │   │   ├── List.razor
│   │   │   │   ├── View.razor
│   │   │   │   └── Edit.razor
│   │   │   └── Components/
│   │   ├── Tenants/
│   │   │   ├── Pages/
│   │   │   └── Components/
│   │   ├── Leases/
│   │   │   ├── Pages/
│   │   │   └── Components/
│   │   ├── SecurityDeposits/
│   │   │   ├── Pages/
│   │   │   └── Components/
│   │   ├── Applications/
│   │   ├── Inspections/
│   │   └── Maintenance/
│   ├── FinancialReports/
│   └── Administration/
│
├── Shared/                                   # ✅ Cross-cutting UI components
│   ├── Layout/
│   │   ├── MainLayout.razor
│   │   └── NavMenu.razor
│   ├── Components/
│   │   ├── Account/
│   │   └── Pages/
│   └── Services/                            # UI-specific services
│       ├── ToastService.cs
│       ├── ThemeService.cs
│       ├── SessionTimeoutService.cs
│       └── UserContextService.cs
│
├── Utilities/                                # Helper utilities (stays)
└── wwwroot/                                  # Static files (stays)
```

---

## Namespace Mapping

| Old Namespace                                                       | New Namespace                                                 |
| ------------------------------------------------------------------- | ------------------------------------------------------------- |
| `Aquiis.SimpleStart.Models`                                         | `Aquiis.SimpleStart.Core.Entities`                            |
| `Aquiis.SimpleStart.Components.PropertyManagement.Tenants`          | `Aquiis.SimpleStart.Core.Entities` (Tenant.cs)                |
| `Aquiis.SimpleStart.Components.PropertyManagement.SecurityDeposits` | `Aquiis.SimpleStart.Core.Entities` (models only)              |
| `Aquiis.SimpleStart.Components.Administration.Application`          | `Aquiis.SimpleStart.Core.Constants` (ApplicationConstants.cs) |
| `Aquiis.SimpleStart.Data`                                           | `Aquiis.SimpleStart.Infrastructure.Data`                      |
| `Aquiis.SimpleStart.Services`                                       | `Aquiis.SimpleStart.Application.Services`                     |
| `Aquiis.SimpleStart.Components.PropertyManagement.*`                | `Aquiis.SimpleStart.Features.PropertyManagement.*` (UI only)  |
| `Aquiis.SimpleStart.Components.Layout`                              | `Aquiis.SimpleStart.Shared.Layout`                            |
| `Aquiis.SimpleStart.Components.Shared`                              | `Aquiis.SimpleStart.Shared.Components`                        |

---

## Migration Phases

### ✅ Phase 0: Preparation

- [x] Create refactoring plan document
- [x] Commit all current changes to git
- [x] Create refactoring branch: `git checkout -b refactor/clean-architecture`
- [x] Run full build to establish baseline

### ✅ Phase 1: Create New Folder Structure

- [x] Create `Core/` directory with subdirectories
  - [x] `Core/Entities/`
  - [x] `Core/Interfaces/Repositories/` (deferred)
  - [x] `Core/Interfaces/Services/` (deferred)
  - [x] `Core/Constants/`
- [x] Create `Infrastructure/` directory
  - [x] `Infrastructure/Data/`
  - [x] `Infrastructure/Data/Repositories/` (deferred)
  - [x] `Infrastructure/Services/` (deferred)
- [x] Create `Application/` directory
  - [x] `Application/Services/`
  - [x] `Application/Services/PdfGenerators/`
- [x] Create `Features/` directory (empty, ready for Phase 7)
  - [ ] `Features/PropertyManagement/` (Phase 7)
  - [ ] `Features/Administration/` (Phase 7)
- [x] Create `Shared/` directory
  - [x] `Shared/Layout/` (deferred to Phase 8)
  - [x] `Shared/Components/` (deferred to Phase 8)
  - [x] `Shared/Services/`

### ✅ Phase 2: Move Entity Models to Core

**Order matters: Move in dependency order (BaseModel first)**

- [x] Move `Models/BaseModel.cs` → `Core/Entities/BaseModel.cs`
- [x] Move all simple entities (no navigation properties first):
  - [x] `Models/Note.cs`
  - [x] `Models/SchemaVersion.cs`
  - [x] `Models/OrganizationSettings.cs`
  - [x] `Models/CalendarSettings.cs`
  - [x] `Models/CalendarEventTypes.cs`
- [x] Move property management entities:
  - [x] All entities from various locations
  - [x] Total: 29 entity files moved
- [x] Update all model namespaces to `Aquiis.SimpleStart.Core.Entities`
- [x] Delete empty `Models/` directory

### ✅ Phase 3: Move Constants to Core

- [x] Move `Components/Administration/Application/ApplicationConstants.cs` → `Core/Constants/ApplicationConstants.cs`
- [x] Move `Components/Administration/Application/ApplicationSettings.cs` → `Core/Constants/ApplicationSettings.cs`
- [x] Update namespace to `Aquiis.SimpleStart.Core.Constants`

### ✅ Phase 4: Move DbContext to Infrastructure

- [x] Move `Data/ApplicationDbContext.cs` → `Infrastructure/Data/ApplicationDbContext.cs`
- [x] Move `Data/Migrations/` → `Infrastructure/Data/Migrations/` (44 migrations)
- [x] Move SQL scripts to `Infrastructure/Data/`
- [x] Update namespace to `Aquiis.SimpleStart.Infrastructure.Data`
- [x] Update DbContext using directives for new entity locations
- [x] Delete empty `Data/` directory

### ✅ Phase 5: Move Services to Application

**Move all services from Services/ to Application/Services/**

- [x] Move 11 business logic services to `Application/Services/`:
  - [x] PropertyManagementService.cs
  - [x] SecurityDepositService.cs
  - [x] TenantConversionService.cs
  - [x] FinancialReportService.cs
  - [x] ChecklistService.cs
  - [x] CalendarEventService.cs
  - [x] CalendarSettingsService.cs
  - [x] NoteService.cs
  - [x] ScheduledTaskService.cs
  - [x] SchemaValidationService.cs
  - [x] ApplicationService.cs
- [x] Move 7 PDF generators to `Application/Services/PdfGenerators/`:
  - [x] ChecklistPdfGenerator.cs
  - [x] FinancialReportPdfGenerator.cs
  - [x] InspectionPdfGenerator.cs
  - [x] InvoicePdfGenerator.cs
  - [x] LeasePdfGenerator.cs
  - [x] LeaseRenewalPdfGenerator.cs
  - [x] PaymentPdfGenerator.cs
- [x] Update all service namespaces to `Aquiis.SimpleStart.Application.Services`
- [x] Delete duplicate PDF generators from `Components/PropertyManagement/Documents/`

### ✅ Phase 6: Move UI Services to Shared

- [x] Move 7 cross-cutting services to `Shared/Services/`:
  - [x] ToastService.cs
  - [x] ThemeService.cs
  - [x] SessionTimeoutService.cs
  - [x] UserContextService.cs
  - [x] DatabaseBackupService.cs
  - [x] DocumentService.cs
  - [x] ElectronPathService.cs
- [x] Update namespaces to `Aquiis.SimpleStart.Shared.Services`
- [x] Delete empty `Services/` directory

### ✅ Phase 7: Reorganize Blazor Components to Features

- [x] Move `Components/PropertyManagement/` → `Features/PropertyManagement/`
  - [x] Applications/
  - [x] Checklists/
  - [x] Documents/
  - [x] Inspections/
  - [x] Invoices/
  - [x] LeaseOffers/
  - [x] Leases/
  - [x] MaintenanceRequests/
  - [x] Payments/
  - [x] Properties/
  - [x] Reports/
  - [x] SecurityDeposits/
  - [x] Tenants/
  - [x] Calendar.razor, CalendarListView.razor
- [x] Move `Components/Administration/` → `Features/Administration/`
  - [x] Application/ (3 pages)
  - [x] PropertyManagement/ (7 management pages)
  - [x] Settings/ (4 settings pages)
  - [x] Users/ (3 pages)
  - [x] Dashboard.razor
- [x] Create `Features/_Imports.razor` with all necessary using directives
- [x] Update all component namespaces to `Aquiis.SimpleStart.Features.*`
- [x] Update all @page routes (namespaces only, routes unchanged)
- [x] Remove obsolete using statements from Application services
- [x] **Build Status**: ✅ 0 errors

### ✅ Phase 8: Move Shared UI Components

- [x] Move `Components/Layout/` → `Shared/Layout/`
- [x] Move `Components/Shared/` → `Shared/Components/`
- [x] Move `Components/Account/` → `Shared/Components/Account/`
- [x] Move `Components/Pages/` → `Shared/Components/Pages/`
- [x] Update namespaces to `Aquiis.SimpleStart.Shared.*`
- [x] Create `Shared/_Imports.razor`
- [x] Update cross-references in Application, Infrastructure, Core
- [x] Clean up empty subdirectories
- [x] **Build Status**: ✅ 0 errors

### ✅ Phase 9: Update All References (Completed with Phases 2-8)

- [x] Update `Program.cs`:
  - [x] Update service registrations with new namespaces
  - [x] Update DbContext registration
  - [x] Add using directives for all service namespaces
- [x] Update `Components/_Imports.razor`:
  - [x] Add `Application.Services`
  - [x] Add `Shared.Services`
  - [x] Add `Core.Entities`
  - [x] Add `Core.Constants`
- [x] Find and replace old using statements (205 files updated):
  - [x] `using Aquiis.SimpleStart.Models;` → `using Aquiis.SimpleStart.Core.Entities;`
  - [x] `using Aquiis.SimpleStart.Data;` → `using Aquiis.SimpleStart.Infrastructure.Data;`
  - [x] `using Aquiis.SimpleStart.Services;` → split into 3 service namespaces
  - [x] `@using` statements in all Razor files
  - [x] Fixed namespace collisions (CalendarSettings, OrganizationSettings)
- [x] **Build Status**: ✅ 0 errors, 0 warnings

### 🔄 Phase 10: Clean Up Old Folders

- [x] Delete empty `Models/` directory
- [x] Delete empty `Services/` directory
- [x] Delete empty `Data/` directory (kept Scripts)
- [x] Delete empty `Components/Account`, `Components/Layout`, `Components/Pages`, `Components/Shared` directories
- [ ] Verify no orphaned files
- [ ] Final cleanup check

### 🔄 Phase 11: Build & Test

- [ ] Run full build: `dotnet build`
- [ ] Fix any compilation errors
- [ ] Run application and test core workflows:
  - [ ] Property listing
  - [ ] Tenant creation
  - [ ] Lease acceptance
  - [ ] Security deposit creation
  - [ ] Financial reports
- [ ] Fix any runtime errors

### 🔄 Phase 12: Documentation & Finalization

- [ ] Update `README.md` with new structure
- [ ] Update `ROADMAP.md` if needed
- [ ] Create architecture documentation (optional)
- [ ] Commit refactoring changes
- [ ] Merge refactoring branch to main

---

## Dependency Flow (Clean Architecture Rules)

```
Features → Application → Core
    ↓
Infrastructure → Core
    ↓
Shared → Core
```

**Rules:**

- ✅ Core has NO dependencies (pure domain logic)
- ✅ Infrastructure depends only on Core
- ✅ Application depends only on Core
- ✅ Features depends on Application + Core
- ✅ Shared depends only on Core

---

## Benefits After Refactoring

1. **Clear Separation of Concerns**

   - Domain logic isolated in Core
   - Infrastructure details isolated
   - UI separated from business logic

2. **Testability**

   - Can test Core without any dependencies
   - Can mock Infrastructure easily
   - Can test Application logic independently

3. **Scalability**

   - Easy to add new features (add folder in Features/)
   - Easy to add new entities (add to Core/Entities/)
   - Easy to swap infrastructure (e.g., change from SQLite to SQL Server)

4. **Maintainability**

   - Models all in one place
   - Clear ownership of code
   - Easy to find what you need

5. **Future-Proof**
   - Can add Web API project later (shares Core, Application, Infrastructure)
   - Can add mobile app later (shares Core, Application)
   - Can extract features as microservices if needed

---

## Risk Mitigation

- ✅ **Git Branch**: All work on `refactor/clean-architecture` branch
- ✅ **Incremental**: Move files in phases, build after each phase
- ✅ **Reversible**: Can revert if issues arise
- ✅ **Testing**: Test after each phase to catch issues early

---

## Estimated Timeline

- **Phase 1**: 15 minutes (folder creation)
- **Phase 2**: 30 minutes (move entities, update namespaces)
- **Phase 3**: 5 minutes (move constants)
- **Phase 4**: 10 minutes (move DbContext)
- **Phase 5**: 30 minutes (move services)
- **Phase 6**: 10 minutes (move UI services)
- **Phase 7**: 20 minutes (move Features)
- **Phase 8**: 15 minutes (move Shared)
- **Phase 9**: 30 minutes (update references)
- **Phase 10**: 5 minutes (cleanup)
- **Phase 11**: 30 minutes (build & test)
- **Phase 12**: 15 minutes (documentation)

**Total: ~3-4 hours**

---

## Current Status

**Phase:** 8 Complete - Starting Phase 10  
**Last Updated:** December 2, 2025 10:22 AM  
**Completed Phases:** 0-8 (All entities, services, namespaces, Features, and Shared UI reorganized)  
**Build Status:** ✅ 0 errors, 122 warnings  
**Files Changed Since Phase 8:** 61 files moved and updated  
**Total Commits:** 2 (Phase 0-6, Phase 7)  
**Next Step:** Phase 10 cleanup verification, then test critical workflows

---

## Notes

- Keep this document updated as we progress
- Mark items with ✅ when completed
- Add any issues or blockers discovered
- Update namespace mappings if changes needed
