# Aquiis - Property Management System

![.NET 9.0](https://img.shields.io/badge/.NET-9.0-blue)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-9.0-blueviolet)
![Blazor Server](https://img.shields.io/badge/Blazor-Server-orange)
![Entity Framework](https://img.shields.io/badge/Entity%20Framework-9.0-green)
![SQLite](https://img.shields.io/badge/Database-SQLite-lightblue)

A comprehensive web-based property management system built with ASP.NET Core 9.0 and Blazor Server. Aquiis streamlines rental property management for property owners, managers, and tenants with an intuitive interface and robust feature set.

## 🏢 Overview

Aquiis is designed to simplify property management operations through a centralized platform that handles everything from property listings and tenant management to lease tracking and document storage. Built with modern web technologies, it provides a responsive, secure, and scalable solution for property management professionals.

## ✨ Key Features

### 🏠 Property Management

- **Property Portfolio** - Comprehensive property listings with detailed information
- **Property Details** - Address, type, rent, bedrooms, bathrooms, square footage
- **Availability Tracking** - Real-time property availability status
- **Property Photos** - Image management and gallery support
- **Search & Filter** - Advanced property search and filtering capabilities
- **Property Analytics** - Dashboard with property performance metrics

### 👥 Tenant Management

- **Tenant Profiles** - Complete tenant information management
- **Contact Management** - Phone, email, emergency contacts
- **Tenant History** - Track tenant interactions and lease history
- **Tenant Portal** - Dedicated tenant dashboard and self-service features
- **Communication Tools** - Built-in messaging and notification system
- **Tenant Screening** - Application and background check workflow

### 📄 Lease Management

- **Lease Creation** - Digital lease agreement generation
- **Lease Tracking** - Active, pending, expired, and terminated lease monitoring
- **Rent Tracking** - Monthly rent amounts and payment schedules
- **Security Deposits** - Deposit tracking and management
- **Lease Renewals** - Automated renewal notifications and processing
- **Terms Management** - Flexible lease terms and conditions

### 💰 Financial Management

- **Payment Tracking** - Rent payment monitoring and history
- **Invoice Generation** - Automated invoice creation and delivery
- **Payment Methods** - Multiple payment option support
- **Financial Reporting** - Revenue and expense reporting
- **Late Fee Management** - Automatic late fee calculation and tracking
- **Security Deposit Tracking** - Deposit handling and return processing

### 📁 Document Management

- **File Storage** - Secure document upload and storage
- **Document Categories** - Organized by type (leases, receipts, photos, etc.)
- **Version Control** - Document revision tracking
- **Digital Signatures** - Electronic signature support
- **Document Sharing** - Secure document sharing with tenants
- **Bulk Operations** - Mass document upload and organization

### 🔐 User Management & Security

- **Role-Based Access** - Administrator, Property Manager, and Tenant roles
- **Authentication** - Secure login with ASP.NET Core Identity
- **User Profiles** - Comprehensive user account management
- **Permission Management** - Granular access control
- **Activity Tracking** - User login and activity monitoring
- **Data Security** - Encrypted data storage and transmission

### 🎛️ Administration Features

- **User Administration** - Complete user account management
- **System Configuration** - Application settings and preferences
- **Application Monitoring** - System health and performance tracking
- **Backup Management** - Data backup and recovery tools
- **Audit Logging** - Comprehensive activity and change tracking

## 🛠️ Technology Stack

### Backend

- **Backend**: ASP.NET Core 9.0
- **UI Framework**: Blazor Server
- **Database**: SQLite with Entity Framework Core 9.0
- **Authentication**: ASP.NET Core Identity
- **Architecture**: Clean Architecture with vertical slice organization

### Frontend

- **UI Components**: Blazor Server Components
- **Styling**: Bootstrap 5 with custom CSS
- **Icons**: Bootstrap Icons
- **Responsive Design**: Mobile-first responsive layout
- **Real-time Updates**: Blazor Server SignalR integration

### Development Tools

- **IDE**: Visual Studio Code with C# extension
- **Database Tools**: Entity Framework Core Tools
- **Version Control**: Git with GitHub integration
- **Package Management**: NuGet
- **Build System**: .NET SDK build system

## 📋 Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Git](https://git-scm.com/)
- [Visual Studio Code](https://code.visualstudio.com/) (recommended) or Visual Studio 2022
- [C# Dev Kit Extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit) for VS Code

## 🚀 Quick Start

### 1. Clone the Repository

```bash
git clone https://github.com/xnodeoncode/Aquiis.git
cd Aquiis
```

### 2. Build the Application

```bash
dotnet build
```

### 3. Run Database Migrations

```bash
cd Aquiis.SimpleStart
dotnet ef database update
```

### 4. Start the Development Server

```bash
dotnet run
```

### 5. Access the Application

Open your browser and navigate to:

- **HTTPS**: https://localhost:7244
- **HTTP**: http://localhost:5244

## 🔧 Development Setup

### Visual Studio Code Setup

The project includes pre-configured VS Code settings:

1. Open the workspace file: `Aquiis.code-workspace`
2. Install recommended extensions when prompted
3. Use **F5** to start debugging
4. Use **Ctrl+Shift+P** → "Tasks: Run Task" for build operations

### Available Tasks

- **build** - Debug build (default)
- **build-release** - Release build
- **watch** - Hot reload development
- **publish** - Production publish
- **clean** - Clean build artifacts

### Database Management

#### Manual Database Scripts

SQL scripts for manual database operations are located in:

```bash
cd Infrastructure/Data/Scripts
# Available scripts:
# 00_InitialSchema.sql - Initial database schema
# updateTenant.sql - Tenant table updates
```

#### Entity Framework Commands

```bash
# Create new migration
dotnet ef migrations add [MigrationName]

# Update database
dotnet ef database update

# Remove last migration
dotnet ef migrations remove
```

## 📁 Project Structure

The application follows Clean Architecture principles with clear separation of concerns:

```
Aquiis.SimpleStart/
├── Core/                                # Domain Layer (no dependencies)
│   ├── Entities/                        # Domain models & business entities
│   │   ├── BaseModel.cs                # Base entity with common properties
│   │   ├── Property.cs                 # Property entity
│   │   ├── Tenant.cs                   # Tenant entity
│   │   ├── Lease.cs                    # Lease entity
│   │   ├── SecurityDeposit.cs          # Security deposit entity
│   │   └── ...                         # Other domain entities
│   └── Constants/                       # Application constants
│       ├── ApplicationConstants.cs
│       └── ApplicationSettings.cs
│
├── Infrastructure/                      # Infrastructure Layer
│   ├── Data/                           # Database & persistence
│   │   ├── ApplicationDbContext.cs    # EF Core DbContext
│   │   ├── Migrations/                # EF Core migrations (44 files)
│   │   ├── Scripts/                   # SQL scripts for manual operations
│   │   └── Backups/                   # Database backups
│   └── Services/                       # External service implementations
│
├── Application/                         # Application Layer (business logic)
│   └── Services/                       # Domain services
│       ├── PropertyManagementService.cs
│       ├── SecurityDepositService.cs
│       ├── TenantConversionService.cs
│       ├── FinancialReportService.cs
│       ├── ChecklistService.cs
│       ├── CalendarEventService.cs
│       ├── NoteService.cs
│       └── PdfGenerators/             # PDF generation services
│           ├── LeasePdfGenerator.cs
│           ├── InvoicePdfGenerator.cs
│           ├── PaymentPdfGenerator.cs
│           └── ...
│
├── Features/                            # Presentation Layer (Vertical Slices)
│   ├── PropertyManagement/             # Property management features
│   │   ├── Properties/                 # Property CRUD & management
│   │   ├── Tenants/                   # Tenant management
│   │   ├── Leases/                    # Lease management
│   │   ├── SecurityDeposits/          # Security deposit tracking
│   │   ├── Payments/                  # Payment processing
│   │   ├── Invoices/                  # Invoice management
│   │   ├── Documents/                 # Document management
│   │   ├── Inspections/               # Property inspections
│   │   ├── MaintenanceRequests/       # Maintenance tracking
│   │   ├── Applications/              # Rental applications
│   │   ├── Checklists/                # Checklists & templates
│   │   ├── Reports/                   # Financial & operational reports
│   │   └── Calendar.razor             # Calendar view
│   └── Administration/                 # Admin features
│       ├── Application/               # Application screening
│       ├── PropertyManagement/        # Property admin
│       ├── Settings/                  # System settings
│       ├── Users/                     # User management
│       └── Dashboard.razor
│
├── Shared/                              # Shared UI Layer
│   ├── Layout/                         # Layout components
│   │   ├── MainLayout.razor
│   │   └── NavMenu.razor
│   ├── Components/                     # Reusable UI components
│   │   ├── Account/                   # Authentication components
│   │   ├── Pages/                     # Shared pages (Home, About, Error)
│   │   ├── NotesTimeline.razor
│   │   ├── SessionTimeoutModal.razor
│   │   └── ToastContainer.razor
│   └── Services/                       # UI-specific services
│       ├── ToastService.cs
│       ├── ThemeService.cs
│       ├── SessionTimeoutService.cs
│       ├── UserContextService.cs
│       └── DocumentService.cs
│
├── Components/                          # Root Blazor components
│   ├── App.razor                       # App root component
│   ├── Routes.razor                    # Routing configuration
│   └── _Imports.razor                  # Global using directives
│
├── Utilities/                           # Helper utilities
│   ├── CalendarEventRouter.cs
│   └── SchedulableEntityRegistry.cs
│
├── wwwroot/                             # Static files
│   ├── assets/                         # Images & static assets
│   ├── js/                             # JavaScript files
│   └── lib/                            # Client libraries
│
├── Program.cs                           # Application entry point
├── appsettings.json                    # Configuration
└── appsettings.Development.json        # Development config
```

### Architecture Principles

**Clean Architecture Layers:**

```
Features → Application → Core
    ↓
Infrastructure → Core
    ↓
Shared → Core
```

**Dependency Rules:**

- ✅ **Core** has NO dependencies (pure domain logic)
- ✅ **Infrastructure** depends only on Core (data access)
- ✅ **Application** depends only on Core (business logic)
- ✅ **Features** depends on Application + Core (UI features)
- ✅ **Shared** depends on Core (cross-cutting UI)

**Benefits:**

- **Separation of Concerns**: Domain, business logic, data access, and UI clearly separated
- **Testability**: Each layer can be tested independently
- **Maintainability**: Easy to locate and modify specific functionality
- **Scalability**: Simple to add new features as vertical slices
- **Reusability**: Domain and application layers can be shared across projects

## 🔑 Default User Roles

The system includes three primary user roles:

### Administrator

- Full system access
- User management capabilities
- System configuration
- All property management features

### Property Manager

- Property portfolio management
- Tenant management
- Lease administration
- Financial tracking
- Document management

### Tenant

- Personal dashboard
- Lease information access
- Payment history
- Maintenance requests
- Document viewing

## 🎯 Key Components

### Property Management Service

Core business logic service in the Application layer:

- Property CRUD operations
- Tenant management workflows
- Lease tracking and renewals
- Document handling and storage
- Financial calculations
- Entity relationship management

### Authentication & Authorization

- ASP.NET Core Identity integration
- Role-based access control
- Secure session management
- Password policies
- Account lockout protection

### Database Architecture

- Entity Framework Core with SQLite
- Code-first approach with migrations
- Optimized indexing for performance
- Foreign key constraints
- Soft delete patterns

## 📊 Dashboard Features

### Property Manager Dashboard

- Total properties count
- Available properties metrics
- Active lease tracking
- Tenant statistics
- Recent activity feed
- Quick action buttons

### Administrator Dashboard

- User account metrics
- System health monitoring
- Application statistics
- Administrative quick actions
- Recent system activity

### Tenant Dashboard

- Personal lease information
- Payment history
- Maintenance requests
- Document access
- Communication center

## 🔧 Configuration

### Application Settings

Configuration is managed through:

- `appsettings.json` - Base configuration
- `appsettings.Development.json` - Development overrides
- Environment variables
- User secrets (for sensitive data)

### Key Configuration Areas

- Database connection strings
- Authentication settings
- File storage configuration
- Email service settings
- Application-specific settings

## 🚀 Deployment

### Prerequisites for Production

- Windows/Linux server with .NET 9.0 runtime
- IIS or reverse proxy (nginx/Apache)
- SSL certificate for HTTPS
- Database server (or SQLite for smaller deployments)

### Build for Production

```bash
dotnet publish -c Release -o ./publish
```

### Environment Variables

Set the following for production:

```bash
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=https://+:443;http://+:80
ConnectionStrings__DefaultConnection=[your-connection-string]
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Development Guidelines

- Follow C# coding conventions
- Use meaningful commit messages
- Update documentation for new features
- Add unit tests for new functionality
- Ensure responsive design compatibility

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

### Documentation

- Check the `REVISIONS.md` file for recent changes
- Review component-specific README files in subdirectories
- Refer to ASP.NET Core and Blazor documentation

### Common Issues

1. **Database Connection Issues**: Verify SQLite file permissions and path
2. **Build Errors**: Ensure .NET 9.0 SDK is installed
3. **Authentication Problems**: Check Identity configuration and user roles
4. **Performance Issues**: Review database indexing and query optimization

### Getting Help

- Create an issue on GitHub for bugs
- Check existing issues for known problems
- Review the project documentation
- Contact the development team

## 🏗️ Roadmap

### Upcoming Features

- Mobile application support
- Advanced reporting and analytics
- Integration with accounting software
- Automated rent collection
- Multi-language support
- Advanced tenant screening
- IoT device integration
- API for third-party integrations

### Performance Improvements

- Database optimization
- Caching implementation
- Background job processing
- File storage optimization
- Search performance enhancements

---

**Aquiis** - Streamlining Property Management for the Modern World

Built with ❤️ using ASP.NET Core 9.0 and Blazor Server
