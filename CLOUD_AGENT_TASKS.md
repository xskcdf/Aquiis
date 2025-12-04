# Tasks Delegatable to Cloud Agents

This document outlines the types of tasks that can be effectively delegated to cloud-based AI agents (such as GitHub Copilot, Claude, or similar tools) when working with the Aquiis Property Management System.

---

## Overview

Cloud agents are AI-powered assistants that can help with various software development tasks. They work best with clearly defined, repeatable tasks that follow established patterns. This guide helps developers understand which tasks are suitable for delegation and how to structure requests for optimal results.

---

## ✅ Tasks Well-Suited for Cloud Agents

### 1. Code Generation Tasks

#### CRUD Component Creation
Cloud agents excel at generating Blazor components that follow the established patterns:

- **Create new entity pages** - Generate `Create[Entity].razor` components with forms, validation, and navigation
- **Edit pages** - Generate `Edit[Entity].razor` components with data loading and update logic
- **List/Index pages** - Generate `[Entities].razor` pages with filtering, sorting, and pagination
- **View/Detail pages** - Generate `View[Entity].razor` pages with related data display

**Example prompt:**
> "Create a new Blazor component for creating Vendors following the pattern in CreateProperty.razor. Include OrganizationId filtering and audit fields."

#### Service Method Generation
- Add CRUD methods to `PropertyManagementService.cs` following the existing multi-tenant pattern
- Generate query methods with proper `.Include()` statements and filtering
- Create new service classes following the established patterns

**Example prompt:**
> "Add GetVendorsBySpecialtyAsync method to PropertyManagementService that filters by OrganizationId and Specialty field."

#### Model/Entity Creation
- Generate new model classes inheriting from `BaseModel`
- Add navigation properties and relationships
- Create DbSet configurations for `ApplicationDbContext`

### 2. Database Tasks

#### SQL Script Generation
- Create numbered migration scripts (e.g., `41_AddVendorTable.sql`)
- Generate index creation scripts
- Create seed data scripts following existing patterns

**Example prompt:**
> "Create SQL script 41_AddVendorTable.sql for a Vendors table with Id, OrganizationId, Name, Email, Phone, Specialty, Rating, and BaseModel audit fields."

### 3. PDF Generation

Cloud agents can generate PDF document generators using QuestPDF:

- Invoice PDF templates
- Lease agreement PDFs
- Inspection report PDFs
- Receipt generators
- Financial report exports

**Example prompt:**
> "Create VendorReportPdfGenerator.cs following the pattern in InvoicePdfGenerator.cs to generate a vendor listing PDF."

### 4. Test Creation

- Unit tests for service methods
- Integration tests for component behavior
- Test data setup and fixtures

### 5. Documentation Tasks

- Generate XML documentation comments for methods
- Create component usage documentation
- Update README files
- Generate API documentation

### 6. Code Refactoring

- Extract common code into shared components
- Implement interfaces for existing services
- Convert synchronous code to async patterns
- Apply consistent error handling patterns

### 7. Constants and Configuration

- Add new values to `ApplicationConstants.cs`
- Generate configuration classes
- Create settings models

---

## ⚠️ Tasks Requiring Human Oversight

### Security-Related Changes
While cloud agents can assist, humans should always review:

- Authentication/authorization logic modifications
- Multi-tenant filtering changes (OrganizationId)
- Password handling or encryption code
- API endpoint security

### Database Schema Design
Cloud agents can generate scripts, but architects should approve:

- New table designs and relationships
- Index strategies for performance
- Data type decisions for sensitive fields

### Business Logic
Complex business rules require human validation:

- Financial calculations (late fees, pro-rating)
- Lease term calculations
- Compliance-related logic

### Integration Code
External service integrations need careful review:

- Payment gateway implementations
- Email/SMS service integrations
- Third-party API connections

---

## ❌ Tasks Not Suitable for Cloud Agents

### Tasks requiring runtime context:
- Debugging production issues
- Performance profiling
- Real-time database queries
- Live system monitoring

### Tasks requiring business decisions:
- Feature prioritization
- UI/UX design choices
- Pricing or business rule definitions
- Compliance interpretations

### Tasks requiring external access:
- Deploying to servers
- Managing cloud infrastructure
- Accessing external APIs directly
- Database administration

---

## Best Practices for Delegating Tasks

### 1. Provide Context
Always reference existing patterns:
```
"Following the pattern in PropertyManagementService.cs, create methods for..."
```

### 2. Be Specific About Requirements
Include:
- Entity names and fields
- Relationships to other entities
- Required validation rules
- Access control requirements

### 3. Reference Constants
Point to `ApplicationConstants.cs` for status values, types, and roles:
```
"Use ApplicationConstants.VendorTypes for the type dropdown values"
```

### 4. Mention Multi-Tenancy
Always remind about OrganizationId filtering:
```
"Ensure all queries filter by OrganizationId using UserContextService"
```

### 5. Specify File Locations
Be explicit about where files should be created:
```
"Create the component at Components/PropertyManagement/Vendors/Pages/CreateVendor.razor"
```

---

## Example Task Delegations

### Example 1: New Feature Component

**Task:** Create a vendor management feature

**Delegatable sub-tasks:**
1. ✅ Generate `Vendor.cs` model inheriting from `BaseModel`
2. ✅ Create SQL migration script for Vendors table
3. ✅ Add DbSet and configuration to `ApplicationDbContext.cs`
4. ✅ Generate CRUD service methods in `PropertyManagementService.cs`
5. ✅ Create `Vendors.razor` list component
6. ✅ Create `CreateVendor.razor` form component
7. ✅ Create `ViewVendor.razor` detail component
8. ✅ Create `EditVendor.razor` edit component
9. ✅ Add constants to `ApplicationConstants.cs`

**Requires human review:**
- Database schema approval
- Navigation menu placement
- Access control decisions

### Example 2: Report Generation

**Task:** Create a new financial report

**Delegatable sub-tasks:**
1. ✅ Generate report data model classes
2. ✅ Create service method to aggregate data
3. ✅ Generate PDF generator class using QuestPDF
4. ✅ Create report viewer Blazor component

**Requires human review:**
- Financial calculation accuracy
- Report layout approval
- Data security compliance

### Example 3: Scheduled Task

**Task:** Add automated maintenance reminder

**Delegatable sub-tasks:**
1. ✅ Add method to `ScheduledTaskService.cs`
2. ✅ Create notification model
3. ✅ Generate logging statements
4. ✅ Add configuration settings

**Requires human review:**
- Business timing rules
- Notification content
- Multi-tenant isolation

---

## Automated Tasks in Aquiis

The following automated tasks are already implemented in `ScheduledTaskService.cs` and run without human intervention:

### Daily Tasks (2 AM)
- Apply late fees to overdue invoices
- Update invoice statuses (Pending → Overdue)
- Send payment reminders
- Check lease renewals (90/60/30 day notifications)
- Check overdue routine inspections

### Hourly Tasks
- Mark no-show property tours
- Check for expiring leases
- Calculate payment totals

These provide good examples of tasks that can be enhanced or replicated by cloud agents following the established patterns.

---

## Summary

Cloud agents are most effective when:
- ✅ Tasks follow established code patterns
- ✅ Clear examples exist in the codebase
- ✅ Requirements are well-defined
- ✅ Output can be validated through builds/tests

Cloud agents require human oversight when:
- ⚠️ Security implications exist
- ⚠️ Business logic decisions are needed
- ⚠️ External systems are involved
- ⚠️ Compliance requirements apply

---

**Last Updated:** November 2025
