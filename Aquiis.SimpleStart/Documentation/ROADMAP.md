# Aquiis Property Management System - Roadmap

This document outlines planned improvements and feature enhancements for the Aquiis property management application.

---

## High Priority (Immediate Impact)

### 1. Data Management Strategy (Active/Inactive vs Deletion)

- **Status**: ✅ Completed
- **Description**: Application does not delete data. Properties and tenants can be marked Active/Inactive. Leases, invoices, payments can be Cancelled if necessary.
- **Benefits**: Data integrity, audit trail, historical records
- **Implementation**: Soft delete pattern with IsDeleted flag, Status fields for cancellation
- **Completed**: November 2025

### 2. Toast Notifications

- **Status**: ✅ Completed
- **Description**: Implement non-blocking toast notifications for success/error messages
- **Benefits**: Better user feedback without requiring dismissal
- **Implementation**: Created ToastService, ToastContainer component with auto-dismiss
- **Components**:
  - `Services/ToastService.cs` - Service for showing toasts
  - `Components/Shared/ToastContainer.razor` - Toast display component
  - Registered as scoped service in Program.cs
  - Added to MainLayout.razor
- **Features**:
  - Success, Error, Warning, Info toast types
  - Auto-dismiss after configurable duration
  - Manual dismiss option
  - Time-ago display
  - Icon indicators for each type
- **Completed**: November 2025

### 3. Payment Reminders and Automated Late Fees

- **Status**: ✅ Completed
- **Description**:
  - Send email/SMS reminders before invoice due dates
  - Automatically calculate and add late fees to overdue invoices
- **Benefits**: Improved cash flow, reduced manual work
- **Implementation**: Background job service for scheduled tasks
- **Components**:
  - `Services/ScheduledTaskService.cs` - Background service running daily at 2 AM
  - `Components/Administration/Settings/Pages/LateFeeSettings.razor` - Admin configuration page
  - Database migration: `37_AddLateFeeAndReminderToInvoices.sql`
  - Test data script: `38_TestDataForLateFees.sql`
- **Features**:
  - Automatic late fee application (5% of invoice amount, capped at $50)
  - 3-day grace period before late fees apply
  - Payment reminders sent 3 days before due date
  - Automatic status updates (Pending → Overdue)
  - Prevents duplicate charges and reminders
  - Audit trail with timestamps
  - Admin configuration page for settings
  - Email integration placeholder (ready for future SMTP/SendGrid)
- **Business Rules**:
  - Grace Period: 3 days after due date
  - Late Fee: 5% of original invoice amount
  - Late Fee Cap: $50 maximum
  - Reminder Timing: 3 days before due date
  - Updates logged to invoice Notes field
- **Completed**: November 2025

### 4. Lease Renewal Tracking

- **Status**: ✅ Completed
- **Description**:
  - Track expiring leases with alerts
  - Generate renewal offers
  - Dashboard widget showing leases expiring in 30/60/90 days
- **Benefits**: Proactive lease management, reduced vacancy
- **Implementation**: Service methods and dashboard component
- **Components**:
  - Updated `Lease.cs` model with renewal tracking fields
  - `Services/ScheduledTaskService.cs` - Automated renewal checking (90/60/30 day notifications)
  - `Components/Shared/LeaseRenewalWidget.razor` - Dashboard widget with filtering
  - Enhanced `ViewLease.razor` - Renewal offer management interface
  - Database migration: `39_AddLeaseRenewalTracking.sql`
- **Features**:
  - Automatic notifications at 90, 60, and 30 days before expiration
  - Renewal status tracking (Pending, Offered, Accepted, Declined, Expired)
  - Proposed rent tracking with percentage change calculation
  - Dashboard widget with 30/60/90 day filters
  - Visual urgency indicators (color-coded by days remaining)
  - Send renewal offers directly from lease view
  - Track offer dates and tenant responses
  - Automatic lease expiration handling
  - Renewal notes and audit trail
- **Workflow**:
  - System sends initial notification 90 days before expiration
  - Reminder sent at 60 days
  - Final reminder at 30 days
  - Property manager can send renewal offer with proposed rent
  - Track tenant acceptance/decline
  - Expired leases automatically updated
- **Completed**: November 2025

### 5. Financial Reports

- **Status**: ✅ Completed
- **Description**: Generate comprehensive financial reports:
  - Monthly income statements
  - Rent roll reports
  - Profit/loss by property
  - Tax reports (Schedule E for IRS)
- **Benefits**: Better financial visibility and compliance
- **Implementation**: Report generation service with PDF export
- **Components**:
  - `Components/PropertyManagement/Reports/IncomeStatement.cs` - Report models and DTOs
  - `Services/FinancialReportService.cs` - Report generation logic
  - `Services/FinancialReportPdfGenerator.cs` - PDF export functionality
  - `Components/PropertyManagement/Reports/Pages/Reports.razor` - Report dashboard
  - `Components/PropertyManagement/Reports/Pages/IncomeStatementReport.razor` - Income statement report
  - `Components/PropertyManagement/Reports/Pages/RentRollReport.razor` - Rent roll report
  - `Components/PropertyManagement/Reports/Pages/PropertyPerformanceReport.razor` - Property comparison
  - `Components/PropertyManagement/Reports/Pages/TaxReport.razor` - Schedule E tax report
- **Features**:
  - Income Statement: Revenue and expense breakdown by category
  - Rent Roll: Current tenant status, rent amounts, payment balances
  - Property Performance: ROI, occupancy rates, net income comparison
  - Tax Report: Schedule E format with depreciation calculations
  - PDF export for all reports
  - Date range filtering
  - Property-specific or portfolio-wide reports
  - Real-time data from database
- **Completed**: November 2025

---

## Medium Priority (Enhanced Functionality)

### 6. Email/SMS Communication System

- **Status**: Planned
- **Description**:
  - In-app messaging between property managers and tenants
  - Email templates for common communications
  - SMS integration for urgent notifications
  - Communication history logging
- **Benefits**: Centralized communication, better tenant relations
- **Implementation**: Email service (SendGrid/SMTP), SMS service (Twilio)

### 7. Vendor Management for Maintenance

- **Status**: Planned
- **Description**:
  - Track contractors and vendors
  - Assign maintenance requests to specific vendors
  - Track vendor performance and ratings
  - Vendor contact information and specialties
- **Benefits**: Streamlined maintenance workflow
- **Implementation**: Vendor entity, service methods, assignment workflow

### 8. Document Version Control

- **Status**: Planned
- **Description**:
  - Track document versions and changes
  - View document history
  - Restore previous versions
  - Track who modified what and when
- **Benefits**: Document integrity and audit trail
- **Implementation**: Document versioning table and service layer

### 9. Advanced Filtering and Saved Searches

- **Status**: Planned
- **Description**:
  - Advanced filters for properties (price range, availability date, inspection status)
  - Advanced filters for tenants (lease status, payment history)
  - Save frequently used filter combinations
  - Global search across all entities
- **Benefits**: Faster data access, improved productivity
- **Implementation**: Filter persistence, search service

### 10. Tenant Self-Service Portal

- **Status**: Planned
- **Description**:
  - Tenants can log in to view their information
  - Pay rent online
  - Submit maintenance requests
  - View lease documents
  - Update contact information
  - View payment history
- **Benefits**: Reduced administrative burden, better tenant satisfaction
- **Implementation**: Tenant role, tenant-specific views, payment gateway integration

---

## Security & Authentication

### Multi-Factor Authentication (MFA)

- **Status**: Planned
- **Description**: Add 2FA support for enhanced security
- **Benefits**: Protect sensitive tenant and financial data
- **Implementation**: ASP.NET Core Identity 2FA

### Role-Based Feature Flags

- **Status**: Planned
- **Description**: More granular permissions (ReadOnly, Maintenance Staff, Accountant, etc.)
- **Benefits**: Fine-grained access control
- **Implementation**: Custom authorization policies

### Audit Logging

- **Status**: Planned
- **Description**: Track who views/modifies sensitive data (SSNs, payment info)
- **Benefits**: Compliance and security monitoring
- **Implementation**: Audit log table, interceptor pattern

### Session Timeout

- **Status**: ✅ Completed
- **Description**: Automatic logout after inactivity with warning modal and countdown
- **Benefits**: Security for shared computers, prevents unauthorized access
- **Implementation**: SessionTimeoutService, JavaScript activity tracking, warning modal with session refresh
- **Components**:
  - `Services/SessionTimeoutService.cs` - Core timeout management
  - `Components/Shared/SessionTimeoutModal.razor` - Warning dialog with countdown
  - `wwwroot/js/sessionTimeout.js` - Client-side activity monitoring
  - `/api/session/refresh` - Session extension endpoint
- **Features**:
  - Configurable timeout durations per environment
  - Warning modal appears before timeout
  - Countdown timer showing seconds remaining
  - "Stay Logged In" to extend session
  - Activity tracking (mouse, keyboard, touch, scroll)
  - Automatic logout on timeout expiration
  - Disabled by default in development and Electron modes
- **Configuration**:
  - Production: 30 min timeout, 2 min warning
  - Development: 60 min timeout, disabled for easier testing
  - Electron: 120 min timeout, disabled (desktop convenience)
- **Completed**: November 2025

### API Rate Limiting

- **Status**: Planned
- **Description**: Prevent abuse of service methods
- **Benefits**: Performance and security
- **Implementation**: ASP.NET Core rate limiting middleware

---

## Data Validation & Integrity

### Email Verification

- **Status**: Planned
- **Description**: Send verification emails to tenants when accounts are created
- **Benefits**: Ensure valid contact information
- **Implementation**: Email confirmation workflow

### Enhanced Duplicate Detection

- **Status**: Planned
- **Description**:
  - Check for duplicate emails when creating tenants
  - Warn about similar addresses when creating properties
  - Detect duplicate invoices by date range and lease
- **Benefits**: Data quality and integrity
- **Implementation**: Service-layer validation methods

### Phone Number Validation

- **Status**: Planned
- **Description**: Use regex or library to validate phone number formats consistently
- **Benefits**: Standardized phone number format
- **Implementation**: Custom validation attribute

### Soft Delete Consistency Audit

- **Status**: Planned
- **Description**: Ensure all entities properly implement soft delete
- **Benefits**: Data recovery capability
- **Implementation**: Code review and testing

---

## User Experience

### Breadcrumb Navigation

- **Status**: Planned
- **Description**: Show current location in app hierarchy
- **Benefits**: Easier navigation
- **Implementation**: Breadcrumb component

### Recent Activity Widget

- **Status**: Planned
- **Description**: Dashboard showing recent actions (new tenants, payments, maintenance requests)
- **Benefits**: Quick overview of system activity
- **Implementation**: Activity tracking service

### Keyboard Shortcuts

- **Status**: Planned
- **Description**: Common actions like "N" for new, "S" for search, "?" for help
- **Benefits**: Power user productivity
- **Implementation**: JavaScript keyboard event handlers

### Dark Mode

- **Status**: Planned
- **Description**: Toggle for user preference between light and dark themes
- **Benefits**: User comfort, accessibility
- **Implementation**: CSS variables, theme switcher

### Favorites/Bookmarks

- **Status**: Planned
- **Description**: Quick access to frequently viewed properties/tenants
- **Benefits**: Faster navigation
- **Implementation**: User preferences table

---

## Search & Filtering

### Global Search

- **Status**: Planned
- **Description**: Search across all entities from a single search bar
- **Benefits**: Quick information access
- **Implementation**: Full-text search or search service

### Export Functionality

- **Status**: Planned
- **Description**: Export filtered results to CSV/Excel
- **Benefits**: External data analysis
- **Implementation**: Export service with file download

---

## Financial Management

### Payment Plans

- **Status**: Planned
- **Description**: Support for installment payments on large invoices
- **Benefits**: Flexibility for tenants
- **Implementation**: Payment plan entity and tracking

### Receipt Generation

- **Status**: Planned
- **Description**: Automatic PDF receipts for payments
- **Benefits**: Professional documentation
- **Implementation**: PDF generation service

### Bank Integration

- **Status**: Planned
- **Description**: Import bank transactions for reconciliation
- **Benefits**: Automated accounting
- **Implementation**: OFX/QFX file import or Plaid API

### Recurring Invoices

- **Status**: Planned
- **Description**: Auto-generate monthly rent invoices
- **Benefits**: Reduced manual work
- **Implementation**: Background job for invoice creation

---

## Maintenance Management

### Cost Estimation vs Actual

- **Status**: Planned
- **Description**: Track estimated costs vs actual costs for maintenance
- **Benefits**: Budget accuracy
- **Implementation**: Add fields to MaintenanceRequest entity

### Before/After Photos

- **Status**: Planned
- **Description**: Photo uploads for maintenance requests
- **Benefits**: Visual documentation
- **Implementation**: Image upload and storage

### Tenant Notifications

- **Status**: Planned
- **Description**: Auto-notify tenants when work is scheduled/completed
- **Benefits**: Better communication
- **Implementation**: Email/SMS service integration

### Maintenance History Timeline

- **Status**: Planned
- **Description**: View all maintenance for a property over time
- **Benefits**: Historical tracking
- **Implementation**: Timeline view component

### Preventive Maintenance

- **Status**: Planned
- **Description**: Schedule recurring tasks (HVAC service, filter changes)
- **Benefits**: Proactive property management
- **Implementation**: Scheduled maintenance entity

---

## Document Management

### Bulk Upload

- **Status**: Planned
- **Description**: Upload multiple documents at once
- **Benefits**: Efficiency
- **Implementation**: Multi-file upload component

### Document Templates

- **Status**: Planned
- **Description**: Lease agreement templates, standard forms
- **Benefits**: Consistency and speed
- **Implementation**: Template library

### E-Signatures

- **Status**: Planned
- **Description**: Digital signature support for leases
- **Benefits**: Faster lease execution
- **Implementation**: DocuSign or HelloSign integration

### OCR

- **Status**: Planned
- **Description**: Extract data from uploaded documents
- **Benefits**: Data entry automation
- **Implementation**: Azure Computer Vision or Tesseract

### Document Expiration Alerts

- **Status**: Planned
- **Description**: Alerts for expiring insurance, licenses
- **Benefits**: Compliance tracking
- **Implementation**: Scheduled job checking expiration dates

### Folder Organization

- **Status**: Planned
- **Description**: Organize documents by category/property
- **Benefits**: Better organization
- **Implementation**: Document category hierarchy

---

## Reporting & Analytics

### Dashboard Customization

- **Status**: Planned
- **Description**: Let users choose which widgets to display
- **Benefits**: Personalized experience
- **Implementation**: User preferences for dashboard layout

### Occupancy Trends

- **Status**: Planned
- **Description**: Historical occupancy rates and forecasting
- **Benefits**: Strategic planning
- **Implementation**: Analytics service

### Revenue Trends

- **Status**: Planned
- **Description**: Income over time by property/portfolio
- **Benefits**: Financial insights
- **Implementation**: Time-series charts

### Maintenance Analytics

- **Status**: Planned
- **Description**: Common issues, average resolution time, costs by category
- **Benefits**: Identify patterns and issues
- **Implementation**: Aggregation queries and charts

### Tenant Analytics

- **Status**: Planned
- **Description**: Average lease length, turnover rate, payment patterns
- **Benefits**: Better tenant management
- **Implementation**: Statistical analysis service

### Custom Report Builder

- **Status**: Planned
- **Description**: Ad-hoc report creation tool
- **Benefits**: Flexibility
- **Implementation**: Query builder interface

### Scheduled Reports

- **Status**: Planned
- **Description**: Auto-email reports weekly/monthly
- **Benefits**: Automated reporting
- **Implementation**: Background job service

---

## Mobile Optimization

### Progressive Web App (PWA)

- **Status**: Planned
- **Description**: Installable mobile experience
- **Benefits**: App-like experience on mobile
- **Implementation**: PWA manifest and service worker

### Mobile-First Layouts

- **Status**: Planned
- **Description**: Better responsive design for smaller screens
- **Benefits**: Improved mobile usability
- **Implementation**: CSS breakpoint review

### Touch Gestures

- **Status**: Planned
- **Description**: Swipe actions for common tasks
- **Benefits**: Native mobile feel
- **Implementation**: Touch event handlers

### Offline Mode

- **Status**: Planned
- **Description**: Cache data for viewing without connection
- **Benefits**: Always available
- **Implementation**: Service worker caching

### Camera Integration

- **Status**: Planned
- **Description**: Take photos directly for maintenance/inspections
- **Benefits**: Convenience
- **Implementation**: File input with camera attribute

---

## Integration & Automation

### Calendar Integration

- **Status**: Planned
- **Description**: Sync inspections/appointments to Google/Outlook
- **Benefits**: Consolidated scheduling
- **Implementation**: Calendar API integration

### Accounting Software Export

- **Status**: Planned
- **Description**: Export to QuickBooks, Xero
- **Benefits**: Streamlined accounting
- **Implementation**: Export file generation

### Payment Gateways

- **Status**: Planned
- **Description**: Stripe, PayPal for online rent payments
- **Benefits**: Convenient payment options
- **Implementation**: Payment gateway SDK integration

### Background Check Services

- **Status**: Planned
- **Description**: Integrate tenant screening APIs
- **Benefits**: Automated screening
- **Implementation**: Third-party API integration

### Zapier/Make Integration

- **Status**: Planned
- **Description**: Connect to thousands of other services
- **Benefits**: Workflow automation
- **Implementation**: Webhook endpoints

### Webhook Support

- **Status**: Planned
- **Description**: Notify external systems of events
- **Benefits**: System integration
- **Implementation**: Event-driven architecture

---

## Performance & Scalability

### Caching Strategy

- **Status**: Planned
- **Description**: Implement Redis for frequently accessed data
- **Benefits**: Faster response times
- **Implementation**: Distributed cache

### Lazy Loading

- **Status**: Planned
- **Description**: Load images and data on-demand
- **Benefits**: Faster initial page load
- **Implementation**: Intersection Observer API

### Database Indexing Review

- **Status**: Planned
- **Description**: Review and optimize indexes for common queries
- **Benefits**: Query performance
- **Implementation**: Database profiling

### Query Optimization

- **Status**: Planned
- **Description**: Review N+1 query issues, use projection
- **Benefits**: Reduced database load
- **Implementation**: EF Core performance review

### CDN for Static Assets

- **Status**: Planned
- **Description**: Serve static assets from CDN
- **Benefits**: Faster asset delivery
- **Implementation**: Azure CDN or Cloudflare

---

## Data Management

### Bulk Operations

- **Status**: Planned
- **Description**:
  - Bulk update property rents
  - Bulk invoice generation
  - Bulk tenant communications
- **Benefits**: Efficiency for large portfolios
- **Implementation**: Batch processing services

### Data Import/Export

- **Status**: Planned
- **Description**:
  - Import properties from CSV
  - Export all data for backup
- **Benefits**: Data portability
- **Implementation**: CSV import/export services

### Data Archival

- **Status**: Planned
- **Description**: Archive old leases/invoices without deleting
- **Benefits**: Clean active data, retain history
- **Implementation**: Archived flag and filtered queries

### Backup Automation

- **Status**: Planned
- **Description**: Scheduled database backups
- **Benefits**: Data safety
- **Implementation**: Automated backup script

---

## Lease Management Enhancements

### Rent Escalation

- **Status**: Planned
- **Description**: Track and apply annual rent increases
- **Benefits**: Automated rent adjustments
- **Implementation**: Escalation tracking in lease

### Lease Amendments

- **Status**: Planned
- **Description**: Version control for lease modifications
- **Benefits**: Legal documentation
- **Implementation**: Lease version history

### Move-in/Move-out Checklists

- **Status**: Planned
- **Description**: Digital inspection forms
- **Benefits**: Standardized process
- **Implementation**: Checklist templates

### Security Deposit Tracking

- **Status**: Planned
- **Description**: Track deposits, deductions, returns
- **Benefits**: Financial accuracy
- **Implementation**: Deposit entity and workflow

---

## Compliance & Legal

### Regulatory Compliance Tracking

- **Status**: Planned
- **Description**: Fair Housing Act compliance tracking
- **Benefits**: Legal protection
- **Implementation**: Compliance checklist

### Lease Clause Library

- **Status**: Planned
- **Description**: Standard legal clauses
- **Benefits**: Consistency and legal safety
- **Implementation**: Clause database

### Compliance Alerts

- **Status**: Planned
- **Description**: Notify of required inspections, certifications
- **Benefits**: Stay compliant
- **Implementation**: Alert service

### Privacy Controls

- **Status**: Planned
- **Description**: GDPR/CCPA compliance for tenant data
- **Benefits**: Privacy compliance
- **Implementation**: Data access controls

### Retention Policies

- **Status**: Planned
- **Description**: Auto-delete old data per legal requirements
- **Benefits**: Compliance automation
- **Implementation**: Data retention service

---

## Testing & Quality

### Unit Tests

- **Status**: Planned
- **Description**: Comprehensive test coverage for services
- **Benefits**: Code reliability
- **Implementation**: xUnit or NUnit tests

### Integration Tests

- **Status**: Planned
- **Description**: Test API endpoints and database operations
- **Benefits**: System reliability
- **Implementation**: Integration test suite

### E2E Tests

- **Status**: Planned
- **Description**: Automated browser testing of user flows
- **Benefits**: User experience validation
- **Implementation**: Playwright or Selenium

### Error Tracking

- **Status**: Planned
- **Description**: Integrate Sentry or similar for error monitoring
- **Benefits**: Proactive issue detection
- **Implementation**: Error tracking service integration

### Performance Monitoring

- **Status**: Planned
- **Description**: Track page load times, query performance
- **Benefits**: Performance insights
- **Implementation**: Application Insights or New Relic

---

## Property-Specific Features

### Floor Plans

- **Status**: Planned
- **Description**: Upload and display floor plans
- **Benefits**: Better property presentation
- **Implementation**: Image upload and viewer

### Amenity Tracking

- **Status**: Planned
- **Description**: Pool, gym, parking space management
- **Benefits**: Facility management
- **Implementation**: Amenity entity and scheduling

### Utility Management

- **Status**: Planned
- **Description**: Track utility providers and accounts
- **Benefits**: Centralized utility information
- **Implementation**: Utility entity

### Property Photos Gallery

- **Status**: Planned
- **Description**: Photo gallery for each property
- **Benefits**: Visual property documentation
- **Implementation**: Image gallery component

### Virtual Tours

- **Status**: Planned
- **Description**: 360° photos or video tours
- **Benefits**: Remote property viewing
- **Implementation**: 360° viewer integration

### Availability Calendar

- **Status**: Planned
- **Description**: Visual calendar showing unit availability
- **Benefits**: Quick availability view
- **Implementation**: Calendar component

---

## Low Priority (Nice to Have)

These features would enhance the application but are not critical for core functionality:

- Advanced analytics and forecasting
- AI-powered maintenance prediction
- Chatbot for tenant support
- Mobile native apps (iOS/Android)
- Property comparison tools
- Market analysis integration
- Social media integration for property marketing
- Referral program management
- Smart home integration

---

## Implementation Strategy

### Phase 1: Core Improvements (Q1 2026)

- ✅ Toast notifications
- ✅ Payment reminders and automated late fees
- ✅ Lease renewal tracking
- ✅ Financial reports

### Phase 2: Enhanced Features (Q2 2026)

- Communication system
- Vendor management
- Document version control
- Advanced filtering
- Tenant portal

### Phase 3: Integration & Scale (Q3 2026)

- Payment gateway integration
- Accounting software integration
- Performance optimization
- Mobile PWA
- Automated backups

### Phase 4: Advanced Features (Q4 2026)

- Analytics and reporting
- Compliance automation
- Advanced security features
- Third-party integrations
- Custom report builder

---

## Contributing

This roadmap is a living document. Features may be added, removed, or reprioritized based on user feedback and business needs.

**Last Updated**: November 16, 2025
