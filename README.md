# Aquiis SimpleStart

**Modern Property Management for Landlords**

[![Version](https://img.shields.io/badge/version-1.1.0-blue.svg)](https://github.com/xnodeoncode/Aquiis/releases)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-purple.svg)](https://dotnet.microsoft.com/)
[![Platform](https://img.shields.io/badge/platform-Linux%20%7C%20Windows-lightgrey.svg)](#installation)

---

**Aquiis SimpleStart** is a standalone desktop application designed for landlords managing 1-9 residential rental properties. Built with ASP.NET Core 10 and Blazor Server, wrapped in Electron for native desktop experience, it provides professional-grade property management features without the complexity or subscription costs of enterprise solutions.

**Perfect for:**

- Independent landlords with a few properties
- Property owners who self-manage their rentals
- New landlords starting their portfolio
- Anyone seeking affordable, easy-to-use property management software

## ✨ Key Features

### Property Management

- 📋 Manage up to 9 residential properties
- 🏡 Property profiles with photos and documents
- 🔍 Track property status (Available, Occupied, Under Renovation)
- 📊 Property portfolio overview and analytics

### Tenant Management

- 👥 Complete prospect-to-tenant journey
- 📝 Digital rental applications with screening
- ✅ Application approval workflow
- 🤝 Tenant profiles with contact information

### Lease Management

- 📄 Digital lease creation and management
- ✍️ Lease offers with acceptance tracking
- 🔄 Multi-lease support (tenants can have multiple active leases)
- 💰 Security deposit investment tracking with annual dividends

### Financial Management

- 🧾 Automated rent invoice generation
- 💳 Payment tracking by multiple methods
- ⏰ Automatic late fee application after grace period
- 📈 Financial reports and payment history

### Maintenance & Inspections

- 🔧 Maintenance request tracking with vendor assignment
- ✅ Comprehensive 26-item inspection checklist
- 📅 Scheduled routine inspections
- 📄 PDF inspection reports with QuestPDF

### Notifications & Automation

- 🔔 In-app, email, and SMS notifications
- ⏰ Automatic late fees and lease expiration warnings
- 📅 Background tasks for scheduling and cleanup
- 🎯 Configurable notification preferences

### Database & Security

- 💾 SQLite file-based database (no server required)
- 🔒 **NEW:** Database encryption at rest (SQLCipher AES-256)
- 🔑 **NEW:** OS keychain integration for password management
- 🔄 Automatic schema migrations
- 📦 Manual and scheduled backups
- ♻️ Staged restore with preview
- 🔐 Content Security Policy (CSP) headers for web security

---

## � Download

**Latest Release: v1.1.0**

[![Download for Linux](https://img.shields.io/badge/Download-Linux%20AppImage-blue.svg?style=for-the-badge&logo=linux)](https://github.com/xnodeoncode/Aquiis/releases/download/v1.1.0/Aquiis-1.1.0-x86_64.AppImage)
[![Download for Windows](https://img.shields.io/badge/Download-Windows%20Setup-blue.svg?style=for-the-badge&logo=windows)](https://github.com/xnodeoncode/Aquiis/releases/download/v1.1.0/Aquiis-1.1.0-x64-Setup.exe)

**All Downloads:** [View v1.1.0 Release](https://github.com/xnodeoncode/Aquiis/releases/tag/v1.1.0)

---

## 🚀 Quick Start

### Installation

#### Linux (AppImage)

```bash
# Download from releases page or use wget
wget https://github.com/xnodeoncode/Aquiis/releases/download/v1.1.0/Aquiis-1.1.0-x86_64.AppImage

# Make executable
chmod +x Aquiis-1.1.0-x86_64.AppImage

# Option 1: Desktop integration (recommended)
wget https://github.com/xnodeoncode/Aquiis/releases/download/v1.1.0/install-desktop-integration.sh
chmod +x install-desktop-integration.sh
./install-desktop-integration.sh Aquiis-1.1.0-x86_64.AppImage

# Option 2: Run directly
./Aquiis-1.1.0-x86_64.AppImage
```

#### Windows (Installer or Portable)

**Option A: Installer (Recommended)**

1. **Download** `Aquiis-1.1.0-x64-Setup.exe` from [releases page](https://github.com/xnodeoncode/Aquiis/releases/tag/v1.1.0)
2. **Run installer** and follow the setup wizard
3. **Launch** from Start Menu or Desktop shortcut

**Option B: Portable Executable**

1. **Download** `Aquiis-1.1.0-x64-Portable.exe` from [releases page](https://github.com/xnodeoncode/Aquiis/releases/tag/v1.1.0)
2. **Move to permanent location** (e.g., `C:\Program Files\Aquiis\`)
   - ⚠️ Database and settings are stored relative to the .exe location
3. **Double-click** to run

**Note:** Windows SmartScreen warning may appear (app is unsigned). Click **"More info"** → **"Run anyway"**. Code signing planned for future release.

#### Verify Download (Optional)

```bash
# Download checksums
wget https://github.com/xnodeoncode/Aquiis/releases/download/v1.1.0/checksums.txt

# Linux
sha256sum -c checksums.txt

# Windows PowerShell
Get-FileHash "Aquiis-1.1.0-x64-Setup.exe" -Algorithm SHA256
```

### First Run

1. **New Setup Wizard** guides you through initial configuration
2. Create your **organization** (business name and contact info)
3. Register your **first user account**
4. Start managing properties!

**New to Aquiis?** Follow our **[Quick Start Guide](Documentation/Quick-Start-Guide.md)** for a 15-minute tutorial.

### 15-Minute Tutorial

Follow our [Quick Start Guide](Documentation/Quick-Start-Guide.md) to:

- Add your first property
- Add a tenant
- Create a lease
- Generate an invoice
- Record a payment
- Schedule an inspection

---

## 📋 System Requirements

### Minimum Requirements

- **OS:** Linux (Ubuntu 20.04+, Debian 11+) or Windows 10/11 (64-bit)
- **CPU:** 2-core, 1.5 GHz
- **RAM:** 2 GB
- **Disk:** 500 MB

### Universal Linux Support:

Aquiis is distributed as an AppImage, which runs on all major Linux distributions—including Ubuntu, Debian, Fedora, RedHat, Arch, openSUSE, and more. No installation required: just download, make executable, and run.

### Windows Portable Version:

Aquiis is available as a portable Windows executable (.exe). No installation required—just download, extract, and run. All application data is stored locally in the same folder, making it easy to use Aquiis from a USB drive or move between systems.

### Recommended

- **CPU:** 4-core, 2.5 GHz
- **RAM:** 4 GB
- **Disk:** 1 GB
- **Display:** 1920x1080

### Software

- All dependencies bundled (no installation required)
- Optional: SendGrid (email) and Twilio (SMS) for notifications

### Data & Backups

- SQLite database stored locally in application data folder
- Regular backups recommended (manual or scheduled)
- See **[Database Management Guide](Documentation/Database-Management-Guide.md)** for backup/restore procedures

---

## 📚 Documentation

### User Documentation

- 📖 **[Release Notes](Documentation/v1.1.0/v1.1.0-Release-Notes.md)** - What's new in v1.1.0
- 🚀 **[Quick Start Guide](Documentation/Quick-Start-Guide.md)** - Get started in 15 minutes
- 💾 **[Database Management Guide](Documentation/Database-Management-Guide.md)** - Backup, restore, troubleshooting
- 🔒 **[Security Features](Documentation/v1.1.0/Security-Enhancements.md)** - Database encryption, CSP, and more
- 📊 **[Compatibility Matrix](Documentation/Compatibility-Matrix.md)** - Version compatibility and upgrade paths

### Developer Documentation

- 📝 **[Copilot Instructions](.github/copilot-instructions.md)** - Architecture and development guidelines
- 🏛️ **[Roadmap](Documentation/Roadmap/)** - Feature planning and implementation status
- 🔄 **[CHANGELOG](CHANGELOG.md)** - Version history

---

## ⚠️ Known Limitations

**SimpleStart is with intentional constraints:**

| Limitation        | Value                          | Reason                    |
| ----------------- | ------------------------------ | ------------------------- |
| **Properties**    | Maximum 9                      | Simplified workflows      |
| **Users**         | Maximum 3 (1 system + 2 login) | Simplified access control |
| **Organizations** | 1 only                         | Desktop application scope |
| **File uploads**  | 10MB per file                  | Performance management    |

**Need more capacity?** Watch for **Aquiis Professional** (coming 2026) with unlimited properties and multi-organization support.

---

## 🛠️ Technology Stack

- **Framework:** ASP.NET Core 10.0 + Blazor Server
- **Desktop:** ElectronNET 23.6.2
- **Database:** SQLite (Microsoft.EntityFrameworkCore.Sqlite 10.0.1)
- **PDF Generation:** QuestPDF 2025.12.1
- **Email:** SendGrid 9.29.3
- **SMS:** Twilio 7.14.0
- **UI:** Bootstrap 5.3, Material Design Icons
- **Architecture:** Clean Architecture with service layer pattern

---

## 🏗️ Project Structure

```
Aquiis/
├── 0-Aquiis.Core/              # Domain entities and interfaces
├── 1-Aquiis.Infrastructure/    # Data access and external services
├── 2-Aquiis.Application/       # Business logic and services
├── 3-Aquiis.UI.Shared/         # Shared UI components (SimpleStart + Professional)
├── 4-Aquiis.SimpleStart/       # SimpleStart desktop application
├── 5-Aquiis.Professional/      # Professional web application (future)
└── 6-Tests/                    # Unit and integration tests
```

---

## 🧪 Testing

**Test Suite:**

- ✅ **303 unit tests** passing
- ✅ **Application layer:** 243 tests (services, workflows, business logic)
- ✅ **UI.Shared components:** 47 tests (layout, notifications, common components)
- ✅ **Core validation:** 13 tests (utilities, attributes)

**Integration tests** require running applications and are validated during UAT.

**Run tests:**

```bash
dotnet test Aquiis.sln
```

---

## 🤝 Contributing

We welcome contributions! Here's how to get started:

1. **Fork the repository**
2. **Create a feature branch:** `git checkout -b feature/your-feature-name`
3. **Read [copilot-instructions.md](.github/copilot-instructions.md)** for architecture guidelines
4. **Make your changes** following the coding standards
5. **Write tests** for new features
6. **Submit a pull request**

### Development Workflow

**Branch Strategy:**

```
main (protected, production-ready)
  ↑ Pull Request
development (integration testing)
  ↑ Direct merge
feature/your-feature-name
```

**Build and run:**

```bash
# Build
dotnet build Aquiis.sln

# Run SimpleStart
cd 4-Aquiis.SimpleStart
dotnet run

# Or use hot reload
dotnet watch
```

---

## 📊 Versioning

We use [Semantic Versioning](https://semver.org/):

- **MAJOR** version (X.0.0): Breaking changes, database schema updates
- **MINOR** version (0.X.0): New features, UI changes (backward compatible)
- **PATCH** version (0.0.X): Bug fixes, minor updates

**Current version:** 1.1.0 (Security and schema enhancement release)
**Database version:** 1.1.0

---

## 🗺️ Roadmap

### v1.1.0 (Released February 2026) ✅

- ✅ Database encryption at rest (SQLCipher AES-256)
- ✅ OS keychain integration (Linux)
- ✅ Sample data management (IsSampleData flag)
- ✅ Multi-tenant index fixes
- ✅ Content Security Policy headers
- ✅ Database schema v1.1.0 with automatic migrations

### v1.2.0 (Q2 2026)

- 🎯 Windows/macOS keychain integration
- 🎯 Rate limiting for login endpoints
- 🎯 Antiforgery tokens in all forms
- 🎯 Code signing for Windows
- 🎯 Calendar refactoring

### v1.3.0 (Q3 2026)

- 🎯 Tenant portal for online payment and maintenance requests
- 🎯 Enhanced reporting and customization
- 💳 Online rent payment processing (Stripe integration)
- 📊 Advanced analytics and forecasting

### v2.0.0 (2027) - Aquiis Professional

- 🏢 Unlimited properties
- 👥 Multi-organization support
- 🌐 Web-based deployment
- 📱 Full mobile app

---

## 📜 License

Copyright © 2026 CIS Guru. All rights reserved.

Licensed under the **MIT License** - see [LICENSE](LICENSE) file for details.

---

## 📞 Support

### Getting Help

- 📧 **Email:** cisguru@outlook.com
- 🐛 **Bug Reports:** [GitHub Issues](https://github.com/xnodeoncode/Aquiis/issues)
- 💡 **Feature Requests:** [GitHub Discussions](https://github.com/xnodeoncode/Aquiis/discussions)
- 📖 **Documentation:** [/Documentation/](Documentation/)
- 🔒 **Security Issues:** Please report privately to cisguru@outlook.com

### Community

- ⭐ Star this repository
- 🍴 Fork and contribute
- 💬 Join discussions
- 📢 Share feedback

---

## 🙏 Acknowledgments

**Built with:**

- ASP.NET Core team for the amazing framework
- Electron.NET team for desktop integration
- QuestPDF team for PDF generation
- SendGrid and Twilio for notification services
- GitHub Copilot for AI-assisted development

**Special thanks:**

- All beta testers and early adopters
- Open source community contributors
- Everyone who provided feedback and suggestions

---

## 🎊 Status

**v1.1.0 - Security & Schema Enhancement Release** 🎉

- ✅ **Production Ready**
- ✅ **303 tests passing**
- ✅ **Database encryption at rest**
- ✅ **CI/CD pipeline complete**
- ✅ **Documentation complete**
- 🔒 **Enterprise-grade security features**
- 🚀 **Ready for production use!**

---

**Made with ❤️ for independent landlords everywhere**

**Happy property managing!** 🏠
