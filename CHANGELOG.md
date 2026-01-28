# Changelog

All notable changes to **Aquiis SimpleStart** will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [1.0.0] - 2026-01-29

### 🎉 Initial Production Release

**Aquiis SimpleStart v1.0.0** is the first production-ready release of our desktop property management application for landlords managing 1-9 residential properties.

### Added

#### Property Management

- Property profiles supporting up to 9 residential properties
- Property photos and document attachments
- Property status tracking (Available, Occupied, Under Renovation, Off-Market)
- Property portfolio overview dashboard
- Property-specific settings and customization

#### Tenant Management

- Complete prospect-to-tenant lifecycle workflow
- Prospective tenant profiles with 9 status transitions
- Digital rental application system with online submission
- Application screening workflow (background checks, credit checks)
- Tenant conversion system with audit trail
- Tenant profiles with contact information and emergency contacts
- Multi-lease support (tenants can have multiple active leases)

#### Lease Management

- Digital lease creation and management
- Lease offer generation with expiration tracking (30-day validity)
- E-signature workflow with full audit trail (IP address, timestamp, user agent)
- Lease acceptance with security deposit collection
- Multi-lease support per tenant
- Security deposit investment pool with annual dividends
- Dividend distribution system (pro-rated, tenant choice: credit or check)
- Lease status tracking (Offered, Active, Declined, Terminated, Expired)
- Automatic lease expiration notifications
- Month-to-month rollover for unsigned lease offers

#### Financial Management

- Automated rent invoice generation
- Monthly recurring invoice support
- Payment tracking with multiple payment methods
- Late fee calculation after configurable grace period
- Automatic late fee application via background service (daily at 2 AM)
- Payment history and tracking
- Financial reports (income summary, payment history)
- Security deposit dividend calculations and distribution

#### Maintenance & Inspections

- Maintenance request tracking and management
- Vendor assignment and coordination
- Comprehensive 26-item inspection checklist (5 categories: Exterior, Interior, Kitchen, Bathroom, Systems)
- Move-in, Move-out, and Routine inspection types
- Scheduled routine inspections with automatic reminders
- PDF inspection report generation with QuestPDF
- Photo attachments for inspection items
- Inspection history and tracking

#### Calendar & Scheduling

- Calendar event management system
- Event types: Tours, Inspections, Maintenance, Payments, Lease Events, Other
- Automated recurring event creation
- Event reminders and notifications
- Property-specific event filtering
- Timeline view of upcoming events

#### Notifications & Communication

- Multi-channel notification system (in-app, email, SMS)
- Graceful degradation if external services unavailable
- Notification preferences per user
- Email notifications via SendGrid integration
- SMS notifications via Twilio integration
- Toast notifications for in-app feedback
- Automatic notifications for: late fees, lease expiration, inspection reminders, payment confirmations

#### Database Management

- SQLite file-based database (no server required)
- Automatic schema migration system
- Manual backup creation (on-demand)
- Scheduled automatic backups (daily/weekly/monthly at 2 AM)
- Staged restore with preview functionality
- Full restore with automatic application restart
- Database health checks (startup, hourly, before backup)
- Database optimization (VACUUM/ANALYZE)
- Database reset with safety confirmations
- Backup retention management
- External backup support (cloud sync, USB)
- 3-2-1 backup strategy recommended

#### Background Automation

- Scheduled task service running daily at 2 AM
- Automatic late fee application after grace period
- Lease expiration monitoring and notifications
- Security deposit dividend calculation and distribution
- Routine inspection scheduling
- Database cleanup and maintenance
- Cache refresh and optimization

#### User Management

- ASP.NET Core Identity integration
- Three user roles: Administrator, PropertyManager, Tenant
- Role-based access control (RBAC)
- User account creation and management
- Password policies and security
- Account lockout protection
- Session timeout after 18 minutes of inactivity
- Session timeout modal with extend/logout options
- User limit: 3 users maximum (1 system + 2 login users)

#### Reports & Analytics

- Financial reports (income summary, payment history)
- Property portfolio overview
- Maintenance summary reports
- Inspection history reports
- Payment tracking reports
- PDF report generation with QuestPDF
- Export capabilities for reports

#### Desktop Application Features

- ElectronNET desktop wrapper for native experience
- Cross-platform support (Linux and Windows)
- Offline-capable database (SQLite)
- Native file system integration
- System tray integration
- Application auto-updates (future)
- Single-instance enforcement

#### Documentation

- Comprehensive Release Notes (48 sections)
- Quick Start Guide (15-minute tutorial with 9 steps)
- Database Management Guide (10 chapters: backup, restore, troubleshooting)
- System Requirements documentation
- Installation guides (Linux AppImage/deb, Windows NSIS/portable)
- FAQ sections (15+ questions per guide)
- Troubleshooting guides (5+ scenarios per guide)

### Known Limitations

**Aquiis SimpleStart** is intentionally designed with the following constraints for simplified workflows and performance:

| Limitation        | Value                          | Reason                         |
| ----------------- | ------------------------------ | ------------------------------ |
| **Properties**    | Maximum 9                      | Simplified workflows           |
| **Users**         | Maximum 3 (1 system + 2 login) | Simplified access control      |
| **Organizations** | 1 only                         | Desktop application scope      |
| **File uploads**  | 10MB per file                  | Performance management         |
| **Calendar**      | Uses legacy service            | Technical debt (refactor v1.1) |

**Need more capacity?** Watch for **Aquiis Professional** (v2.0.0, 2027) with unlimited properties and multi-organization support.

### Technical Specifications

- **Framework:** ASP.NET Core 10.0 + Blazor Server
- **Desktop:** ElectronNET 23.6.2
- **Database:** SQLite (Microsoft.EntityFrameworkCore.Sqlite 10.0.1)
- **PDF Generation:** QuestPDF 2025.12.1
- **Email:** SendGrid 9.29.3
- **SMS:** Twilio 7.14.0
- **UI:** Bootstrap 5.3, Material Design Icons
- **Architecture:** Clean Architecture with service layer pattern
- **Test Coverage:** 303 unit tests passing

### System Requirements

**Minimum:**

- **OS:** Linux (Ubuntu 20.04+, Debian 11+) or Windows 10/11 (64-bit)
- **CPU:** 2-core, 1.5 GHz
- **RAM:** 2 GB
- **Disk:** 500 MB

**Recommended:**

- **CPU:** 4-core, 2.5 GHz
- **RAM:** 4 GB
- **Disk:** 1 GB
- **Display:** 1920x1080

### Installation Formats

- **Linux:** AppImage (universal), .deb package (Debian/Ubuntu)
- **Windows:** NSIS installer, portable executable
- **macOS:** Coming in future release

### Breaking Changes

**N/A** - This is the first production release.

**Note:** This is a clean v1.0.0 release. There is **no upgrade path** from pre-release versions (v0.x.x). All database migrations have been squashed into a single v1.0.0_Initial migration. Previous development databases cannot be migrated and must be recreated using the New Setup Wizard.

### Changed

**N/A** - This is the first production release.

### Fixed

**N/A** - This is the first production release.

### Security

**N/A** - This is the first production release.

### Deprecated

**N/A** - This is the first production release.

---

## Release Notes

For detailed information about this release, see:

- 📖 [Full Release Notes](/home/cisguru/Documents/Orion/Projects/Aquiis/Documentation/v1.0.0/v1.0.0-Release-Notes.md)
- 🚀 [Quick Start Guide](/home/cisguru/Documents/Orion/Projects/Aquiis/Documentation/v1.0.0/v1.0.0-Quick-Start-Guide.md)
- 💾 [Database Management Guide](/home/cisguru/Documents/Orion/Projects/Aquiis/Documentation/v1.0.0/v1.0.0-Database-Management-Guide.md)

---

## Roadmap

### v1.1.0 (Planned - Q2 2026)

**Tenant Portal & Mobile Enhancements**

- Tenant self-service portal for online payments
- Maintenance request submission by tenants
- Document access for tenants
- Calendar refactoring (remove legacy service dependencies)
- Enhanced reporting and customization
- Mobile companion app (view-only)

### v1.2.0 (Planned - Q3 2026)

**Payment Processing & Analytics**

- Online rent payment processing (Stripe integration)
- Automated ACH/credit card payments
- Payment scheduling and autopay
- Advanced analytics and forecasting
- Improved financial reporting

### v2.0.0 (Planned - 2027) - Aquiis Professional

**Enterprise Features**

- 🏢 Unlimited properties
- 👥 Multi-organization support
- 🌐 Web-based deployment option
- 📱 Full-featured mobile app
- 🔐 Advanced security and audit logging
- 📊 Advanced reporting and business intelligence
- 🔌 API for third-party integrations

---

## Support

### Getting Help

- 📧 **Email:** cisguru@outlook.com
- 🐛 **Bug Reports:** [GitHub Issues](https://github.com/xnodeoncode/Aquiis/issues)
- 💡 **Feature Requests:** [GitHub Discussions](https://github.com/xnodeoncode/Aquiis/discussions)
- 📖 **Documentation:** [/Documentation/v1.0.0/](Documentation/v1.0.0/)

### Community

- ⭐ Star this repository
- 🍴 Fork and contribute
- 💬 Join discussions
- 📢 Share feedback

---

**Copyright © 2026 CIS Guru. All rights reserved.**

Licensed under the **MIT License** - see [LICENSE](LICENSE) file for details.
