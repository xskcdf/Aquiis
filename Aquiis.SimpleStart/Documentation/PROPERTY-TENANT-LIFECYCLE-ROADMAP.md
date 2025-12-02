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

### **Phase 4: Security Deposits** ⬅️ **NEXT**

**Goal:** Full security deposit lifecycle

**Status:** Not Started

**Tasks:**

- [ ] Create `SecurityDeposit` model with all fields
- [ ] Add security deposit collection to lease signing flow
- [ ] Build security deposit management UI
- [ ] Add deposit settings to `OrganizationSettings`
- [ ] Create move-out disposition workflow

**Estimated Time:** 5-6 hours

---

### **Phase 5: Investment Pool & Dividends**

**Goal:** Security deposit investment tracking and dividend distribution

**Status:** Not Started

**Tasks:**

- [ ] Create `SecurityDepositInvestmentPool` model
- [ ] Create `SecurityDepositDividend` model
- [ ] Build annual pool calculation service
- [ ] Build dividend distribution service (with pro-rating)
- [ ] Create admin UI for pool management
- [ ] Create tenant dashboard to view dividends
- [ ] Implement tenant choice (credit vs. check)
- [ ] Build scheduled job for year-end processing

**Estimated Time:** 8-10 hours

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

**Total Estimated Time:** 47-60 hours

**Completed Phases:** 3/8 (38%)

**Current Phase:** Phase 4 - Security Deposits

**Started:** November 30, 2025

**Phase 1 Completed:** November 30, 2025

**Phase 2 Completed:** November 30, 2025

**Phase 3 Completed:** November 30, 2025

---

## Notes & Decisions

### Key Architectural Decisions

- Enums stored in `ApplicationSettings.cs` for reference/type safety
- Status/type values stored as string constants in `ApplicationConstants.cs` static classes
- Database fields use `string` type, not enums (e.g., `Property.Status` is string)
- EF Core Migrations for all schema changes
- SQLite database (not SQL Server)
- Migrations stored in `Data/Migrations/`

### Development Approach

- Sequential phase completion for complete feature delivery
- Each phase tested before moving to next
- Migration created at end of each phase

### Dependencies

- Phase 2+ depends on Phase 1 completion
- Phase 5 depends on Phase 4 completion
- Phase 6 integrates Phases 2-5
- Phase 7 refines all previous phases
