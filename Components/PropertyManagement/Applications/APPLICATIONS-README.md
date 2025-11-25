# Pre-Lease Application Management System

## Overview

Comprehensive application management system that handles the entire pre-lease pipeline from first contact through lease creation.

---

## System Architecture

### **Module: Prospective Tenant & Application Management**

Unified system tracking prospects from initial inquiry through approved application and conversion to active tenant.

---

## Core Entities

### **1. ProspectiveTenant**

Tracks potential tenants from first contact.

**Properties:**

- Contact Information: FirstName, LastName, Email, Phone
- Status: Lead → ShowingScheduled → Applied → Screening → Approved → Denied → ConvertedToTenant
- Source: Website, Referral, Walk-in, Zillow, etc.
- Notes: General notes about prospect
- InterestedPropertyId: Property they're interested in
- DesiredMoveInDate: When they want to move in
- FirstContactDate: When first contacted
- OrganizationId: Multi-tenant isolation

**Relationships:**

- One-to-Many: Showings
- One-to-One: RentalApplication

### **2. Showing**

Scheduled property viewings.

**Properties:**

- ProspectiveTenantId: Who is viewing
- PropertyId: Which property
- ScheduledDateTime: When viewing is scheduled
- DurationMinutes: Length of showing (default: 30)
- Status: Scheduled, Completed, Cancelled, NoShow
- Feedback: Notes from the showing
- InterestLevel: VeryInterested, Interested, Neutral, NotInterested
- ConductedBy: UserId of property manager who conducted showing
- OrganizationId: Multi-tenant isolation

### **3. RentalApplication**

Formal application submitted by prospect.

**Properties:**

- ProspectiveTenantId: Applicant
- PropertyId: Property applying for
- ApplicationDate: When application submitted
- Status: Submitted, UnderReview, Screening, Approved, Denied

**Current Address:**

- CurrentAddress, CurrentCity, CurrentState, CurrentZipCode
- CurrentRent: What they currently pay
- LandlordName, LandlordPhone: Current landlord contact

**Employment:**

- EmployerName: Current employer
- JobTitle: Position
- MonthlyIncome: Gross monthly income
- EmploymentLengthMonths: How long at current job

**References:**

- Reference1Name, Reference1Phone, Reference1Relationship
- Reference2Name, Reference2Phone, Reference2Relationship

**Fees:**

- ApplicationFee: Amount charged
- ApplicationFeePaid: Payment status
- ApplicationFeePaidDate: When fee was paid

**Decision:**

- DenialReason: Why application was denied (if applicable)
- DecisionDate: When decision was made
- DecisionBy: UserId who made decision

**Relationships:**

- Belongs to: ProspectiveTenant
- Belongs to: Property
- Has one: ApplicationScreening

### **4. ApplicationScreening**

Background and credit check results.

**Properties:**

**Background Check:**

- BackgroundCheckRequested: Boolean
- BackgroundCheckRequestedDate: When requested
- BackgroundCheckPassed: Pass/fail result
- BackgroundCheckCompletedDate: When completed
- BackgroundCheckNotes: Additional details

**Credit Check:**

- CreditCheckRequested: Boolean
- CreditCheckRequestedDate: When requested
- CreditScore: Numeric score
- CreditCheckPassed: Pass/fail result
- CreditCheckCompletedDate: When completed
- CreditCheckNotes: Additional details

**Overall:**

- OverallResult: Pending, Passed, Failed, ConditionalPass
- ResultNotes: Summary and recommendations

**Relationships:**

- Belongs to: RentalApplication

---

## User Interface Structure

```
/PropertyManagement/Applications/
├── Pages/
│   ├── ProspectiveTenants.razor      # Pipeline dashboard (Kanban view)
│   ├── CreateProspectiveTenant.razor # Quick add new prospect
│   ├── ViewProspectiveTenant.razor   # Timeline view of prospect journey
│   ├── Showings.razor                # Calendar view of scheduled showings
│   ├── ScheduleShowing.razor         # Schedule new property showing
│   ├── Applications.razor            # List all applications by status
│   ├── CreateApplication.razor       # Full application form (multi-step)
│   ├── ViewApplication.razor         # Review application & screening
│   └── ReviewApplication.razor       # Approve/deny with decision notes
├── RentalApplicationService.cs        # Business logic service
└── APPLICATIONS-README.md            # This file
```

---

## Key Features

### **ProspectiveTenants.razor - Pipeline Dashboard**

- **Kanban Board View**: Drag & drop between status columns
- **Summary Cards**:
  - Active Leads
  - Scheduled Showings
  - Applications Under Review
  - Approved This Month
- **Quick Actions**: Schedule Showing, Start Application
- **Filters**: By property, date range, status

### **Showings.razor - Calendar View**

- Day/Week/Month calendar views
- Color-coded by property
- Click to view showing details
- Quick reschedule functionality
- Mark as completed with feedback capture

### **CreateApplication.razor - Multi-Step Wizard**

- **Step 1**: Personal Information
- **Step 2**: Current Address & Landlord
- **Step 3**: Employment & Income
- **Step 4**: References
- **Step 5**: Review & Submit
- Auto-populate from ProspectiveTenant data
- Document upload support (ID, pay stubs)
- Application fee payment integration

### **ViewApplication.razor - Application Review**

- Application summary with all details
- Screening section:
  - Request background check button
  - Request credit check button
  - Enter results manually
  - Overall pass/fail decision
- Income qualification calculator (rent-to-income ratio)
- Approve/Deny buttons with workflow
- Convert to Tenant button (creates lease)

---

## RentalApplicationService Methods

### Prospective Tenants

```csharp
Task<List<ProspectiveTenant>> GetProspectiveTenantsAsync()
Task<ProspectiveTenant> AddProspectiveTenantAsync(ProspectiveTenant prospect)
Task UpdateProspectStatus(int prospectId, string newStatus)
```

### Showings

```csharp
Task<List<Showing>> GetShowingsAsync(DateTime? startDate, DateTime? endDate)
Task<Showing> ScheduleShowingAsync(Showing showing)
Task CompleteShowingAsync(int showingId, string feedback, string interestLevel)
Task CancelShowingAsync(int showingId, string reason)
```

### Applications

```csharp
Task<List<RentalApplication>> GetApplicationsAsync(string? status = null)
Task<RentalApplication> SubmitApplicationAsync(RentalApplication application)
Task<ApplicationScreening> RequestScreeningAsync(int applicationId)
Task UpdateScreeningResultsAsync(int screeningId, ApplicationScreening screening)
```

### Decisions

```csharp
Task<RentalApplication> ApproveApplicationAsync(int applicationId, string approvedBy)
Task<RentalApplication> DenyApplicationAsync(int applicationId, string reason, string deniedBy)
Task<Lease> ConvertToLeaseAsync(int applicationId) // Creates Lease & Tenant
```

---

## Integration Points

### Property View Integration

- "Schedule Showing" button on available properties
- Upcoming showings displayed in property sidebar
- Application count badge on property card

### Tenant Integration

**Convert to Tenant Process:**

1. Creates Tenant record (from ProspectiveTenant + Application data)
2. Creates User account (if auto-creation enabled)
3. Creates draft Lease (pre-populated with application details)
4. Links application to final tenant for reference tracking

### Document Integration

- Application documents stored in Documents table
- Document types:
  - "Application"
  - "ID Verification"
  - "Pay Stub"
  - "Reference Letter"
  - "Screening Report"
- Screening reports stored as documents

---

## Implementation Roadmap

### **Sprint 1: Foundation** (CURRENT)

- [x] Create ProspectiveTenant entity
- [x] Create Showing entity
- [x] Create RentalApplication entity
- [x] Create ApplicationScreening entity
- [x] Database migration
- [x] Add to ApplicationDbContext
- [x] Create RentalApplicationService with basic CRUD

### **Sprint 2: Showing System**

- [ ] ProspectiveTenants.razor (list view with add)
- [ ] ScheduleShowing.razor (form)
- [ ] Showings.razor (simple list)
- [ ] Integration with property view

### **Sprint 3: Application Form**

- [ ] CreateApplication.razor (full form)
- [ ] Applications.razor (list by status)
- [ ] ViewApplication.razor (read-only)
- [ ] Status workflow

### **Sprint 4: Screening & Approval**

- [ ] ApplicationScreening UI
- [ ] ReviewApplication.razor (approve/deny)
- [ ] Income qualification calculator
- [ ] ConvertToLeaseAsync method

### **Sprint 5: Polish & Enhancements**

- [ ] Kanban board view for prospects
- [ ] Calendar view for showings
- [ ] Email notifications
- [ ] Dashboard widgets
- [ ] Reporting

---

## Workflow Diagram

```
┌─────────────────┐
│   First Contact │
│  (Phone/Email)  │
└────────┬────────┘
         │
         v
┌─────────────────┐
│ Create Prospect │
│  (Lead Status)  │
└────────┬────────┘
         │
         v
┌─────────────────┐
│ Schedule Showing│
│  (Property Tour)│
└────────┬────────┘
         │
         v
┌─────────────────┐
│Complete Showing │
│ (Capture Feedback)
└────────┬────────┘
         │
    Interested?
         │
         v YES
┌─────────────────┐
│Submit Application│
│  (Full Form)    │
└────────┬────────┘
         │
         v
┌─────────────────┐
│Request Screening│
│ (Background +   │
│  Credit Check)  │
└────────┬────────┘
         │
         v
┌─────────────────┐
│ Review Results  │
│  (Pass/Fail)    │
└────────┬────────┘
         │
    Approved?
         │
         v YES
┌─────────────────┐
│Convert to Tenant│
│ (Create Lease)  │
└─────────────────┘
```

---

## Constants

### ProspectiveStatus

- Lead
- ShowingScheduled
- Applied
- Screening
- Approved
- Denied
- ConvertedToTenant

### ShowingStatus

- Scheduled
- Completed
- Cancelled
- NoShow

### ApplicationStatus

- Submitted
- UnderReview
- Screening
- Approved
- Denied

### InterestLevel

- VeryInterested
- Interested
- Neutral
- NotInterested

### ScreeningResult

- Pending
- Passed
- Failed
- ConditionalPass

---

## Business Rules

### Application Submission

- Must have active prospect record
- Property must be available
- Application fee must be paid before screening
- All required fields must be completed

### Screening

- Can only request screening on submitted applications
- Both background and credit checks recommended
- Overall result based on both checks
- Manual override allowed with notes

### Approval

- Can only approve applications with completed screening
- Income must meet minimum threshold (typically 3x rent)
- No active evictions or bankruptcies (configurable)
- Denial requires reason for fair housing compliance

### Conversion to Tenant

- Only approved applications can be converted
- Creates tenant record with application data
- Auto-generates draft lease
- Prospect status updated to ConvertedToTenant
- Original application linked for audit trail

---

## Security & Compliance

### Data Privacy

- Sensitive data (SSN, etc.) encrypted at rest
- PII access logged for audit
- Screening results access restricted to authorized users
- Document retention policy compliance

### Fair Housing

- Denial reasons required and logged
- Consistent screening criteria applied
- No discriminatory language in notes
- Screening criteria documented

### Multi-Tenant Isolation

- All queries filtered by OrganizationId
- Documents scoped to organization
- Users can only see own organization's prospects

---

_Last Updated: November 25, 2025_
