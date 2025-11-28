# Database Scripts

This directory contains SQL scripts for creating and managing the Aquiis database.

## Script Execution Order

1. **01_CreateTables.sql** - Creates all database tables with proper structure and constraints
2. **02_CreateIndexes.sql** - Creates indexes for optimal query performance
3. **03_SeedData.sql** - Inserts initial data (roles and optional sample data)
4. **04_AddRoleNameConstraint.sql** - Adds unique constraint on role names

## Usage

### Manual Database Creation

To manually create the database using these scripts:

```bash
# Navigate to the project directory
cd /path/to/Aquiis.WebUI

# Create a new database and run the scripts
sqlite3 Data/app.db < Data/Scripts/01_CreateTables.sql
sqlite3 Data/app.db < Data/Scripts/02_CreateIndexes.sql
sqlite3 Data/app.db < Data/Scripts/03_SeedData.sql
sqlite3 Data/app.db < Data/Scripts/04_AddRoleNameConstraint.sql
```

### Automatic Database Creation

The application automatically creates the database using Entity Framework migrations when it starts. These scripts serve as:

- Documentation of the database structure
- Backup method for database creation
- Reference for manual database operations
- Foundation for database deployment scripts

## Database Schema

### Core Tables

- **AspNetUsers** - Extended user accounts with login tracking
- **AspNetRoles** - User roles (Administrator, PropertyManager, Tenant)
- **Properties** - Property listings and details

### Identity Framework Tables

- **AspNetUserClaims** - User claims
- **AspNetUserLogins** - External login providers
- **AspNetUserRoles** - User-role associations
- **AspNetUserTokens** - Authentication tokens
- **AspNetRoleClaims** - Role-based claims

## Custom Fields

### ApplicationUser Extensions

- `LastLoginDate` - Timestamp of last login
- `PreviousLoginDate` - Timestamp of previous login
- `LoginCount` - Total number of logins
- `LastLoginIP` - IP address of last login

### Property Fields

- Complete property management fields including address, type, rent, specifications
- Availability tracking
- Creation and modification timestamps

## Notes

- Scripts use SQLite syntax
- Foreign key constraints are enabled
- WAL journal mode is set for better performance
- Indexes are optimized for common query patterns
- Sample data in seed script is commented out by default
