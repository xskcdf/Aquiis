# Database Backup & Recovery System

## Overview

The Aquiis application uses **SQLite** as its embedded database, which is ideal for desktop applications because:

- Single file database (easy to package with Electron)
- No external dependencies (unlike SQL Server LocalDB)
- Full EF Core support with excellent integration
- Cross-platform compatible
- Industry standard for embedded databases

## Automatic Protection

### On Application Startup (Electron Mode)

1. **Health Check**: Validates database integrity using SQLite's PRAGMA integrity_check
2. **Auto-Recovery**: If corruption detected, automatically restores from most recent valid backup
3. **Migration Safety**:
   - Creates backup before applying any migrations
   - Validates database health after migration
   - Automatically rolls back if migration causes corruption

### Backup Creation Points

- **Initial Setup**: When new database is created
- **Before Migrations**: Every time migrations are pending
- **Manual**: On-demand via admin interface
- **Auto-Cleanup**: Keeps last 10 backups, deletes older ones

## Admin Interface

### Access

Navigate to: `/administration/database-backup`  
Required Role: SuperAdmin or Admin

### Features

#### 1. Database Health Status

- Real-time health check
- Green indicator: Database is healthy
- Red indicator: Database corrupted
- Shows last check time and detailed message

#### 2. Create Manual Backup

- Click "Create Manual Backup" button
- Backup saved to `Data/Backups/` folder
- Named: `Aquiis_Backup_Manual_{timestamp}.db`

#### 3. View Available Backups

- Table showing all backup files
- Displays: filename, creation date, file size
- Sorted by date (newest first)

#### 4. Restore from Backup

- Click "Restore" on any backup
- Confirmation dialog shown
- Current database saved as `.corrupted` before restore
- Application requires restart after restore

#### 5. Auto-Recovery

- Click "Auto-Recover from Corruption"
- Attempts to restore from most recent valid backup
- Tries each backup until one succeeds
- Shows success/failure message

## Schema Version Tracking

### How It Works

The schema version is stored **inside the database itself** in the `SchemaVersions` table:

- **Table Structure**:

  - `Id` - Auto-increment primary key
  - `Version` - String (e.g., "1.0.0")
  - `AppliedOn` - DateTime timestamp
  - `Description` - Text description of changes

- **Version History**: Each schema update adds a new row (maintains complete history)
- **Current Version**: Most recent row (ordered by `AppliedOn DESC`) is the active version

### Version Validation Process

On application startup:

1. Reads latest version from database `SchemaVersions` table
2. Compares to `appsettings.json` → `"SchemaVersion": "1.0.0"`
3. **Match**: Application continues normally
4. **Mismatch**: Warning banner displayed on dashboard

### Why This Matters

- **Backup Safety**: Each `.db` file contains its own `SchemaVersions` table
- **Restore Protection**: When you restore an old backup, the schema version comes with it
- **Version Warning**: If backup is from older app version, you'll get a warning before data corruption occurs
- **Upgrade Path**: Prevents using incompatible database with newer application code

### Example Scenarios

**Scenario 1: Normal Operation**

```
Database SchemaVersion: "1.0.0"
App SchemaVersion: "1.0.0"
Result: ✅ No warning, application runs normally
```

**Scenario 2: Restored Old Backup**

```
Database SchemaVersion: "1.0.0" (from backup)
App SchemaVersion: "2.0.0" (current app)
Result: ⚠️ Warning displayed, recommends updating app or restoring compatible backup
```

**Scenario 3: Downgraded Application**

```
Database SchemaVersion: "2.0.0" (newer)
App SchemaVersion: "1.0.0" (older app version)
Result: ⚠️ Warning displayed, database structure may not be compatible
```

## Backup File Structure

```
Data/
├── Aquiis.db                          # Main database (includes SchemaVersions table)
└── Backups/                           # Backup folder
    ├── Aquiis_Backup_InitialSetup_20251119_120000.db
    ├── Aquiis_Backup_PreMigration_2Pending_20251119_150000.db
    ├── Aquiis_Backup_Manual_20251119_160000.db
    └── ... (up to 10 backups kept, each with its own SchemaVersions table)
```

## Recovery Scenarios

### Scenario 1: Migration Corruption

```
1. User updates application with new migration
2. App starts → Creates backup before migration
3. Migration runs
4. Health check fails (corruption detected)
5. App automatically restores from pre-migration backup
6. Error logged, application continues with old schema
```

### Scenario 2: Startup Corruption

```
1. App starts
2. Health check detects corruption
3. Auto-recovery initiated
4. Restores from most recent valid backup
5. App continues with recovered data
```

### Scenario 3: Manual Recovery

```
1. Admin notices data issues
2. Navigates to Database Backup page
3. Reviews available backups
4. Selects specific backup to restore
5. Confirms restoration
6. App restarts with selected backup
```

## API Usage (For Developers)

### Inject the Service

```csharp
@inject DatabaseBackupService BackupService
```

### Create Backup

```csharp
var backupPath = await BackupService.CreateBackupAsync("Manual");
```

### Check Health

```csharp
var (isHealthy, message) = await BackupService.ValidateDatabaseHealthAsync();
```

### List Backups

```csharp
var backups = await BackupService.GetAvailableBackupsAsync();
```

### Restore Backup

```csharp
var success = await BackupService.RestoreFromBackupAsync(backupPath);
```

### Auto-Recovery

```csharp
var (success, message) = await BackupService.AutoRecoverFromCorruptionAsync();
```

## Why Not SQL Server?

While SQL Server LocalDB is a great database, SQLite is better for this application:

| Feature              | SQLite               | SQL Server LocalDB              |
| -------------------- | -------------------- | ------------------------------- |
| Installation         | None required        | Requires LocalDB runtime        |
| Packaging            | Single .db file      | .mdf + .ldf files               |
| Backup               | Simple file copy     | Complex BACKUP/RESTORE commands |
| Recovery             | Copy file back       | Detach/Attach operations        |
| Cross-Platform       | Windows, Linux, Mac  | Windows only                    |
| File Size            | Smaller              | Larger                          |
| Desktop App Standard | ✅ Industry standard | ❌ Requires server              |
| EF Core Support      | ✅ Full              | ✅ Full                         |

## Best Practices

### For End Users

1. **Create manual backups** before major data changes
2. **Check database health** if experiencing issues
3. **Keep important backups** by copying them outside Backups folder (only last 10 are retained)
4. **Note**: Application restart required after restore

### For Developers

1. **Test migrations locally** before deployment
2. **Review backup logs** in application logs
3. **Monitor backup folder size** (10 backups × ~database size)
4. **Use health check** in scheduled tasks for proactive monitoring

## Troubleshooting

### Backup Creation Failed

- Check disk space
- Verify write permissions on Data/Backups folder
- Review application logs for details

### Restore Failed

- Verify backup file is not corrupted
- Check backup file permissions
- Ensure application has write access to Data folder
- Try next most recent backup

### Health Check Fails

- Try auto-recovery first
- If recovery fails, restore from known-good manual backup
- Check for disk corruption
- Review recent system changes

### All Backups Corrupted

- Check for system-wide issues (disk corruption, antivirus interference)
- If in Electron mode, check for write permission issues
- Consider creating new database and re-entering data
- Contact support with log files

## Logging

All backup operations are logged with:

- **Information**: Successful operations
- **Warning**: Non-critical issues (cleanup failures, no backups found)
- **Error**: Critical failures (corruption, restore failures)

Check application logs at:

- **Development**: Console output
- **Production**: Application log files (location varies by platform)

## Future Enhancements

Planned features:

- Scheduled automatic backups (daily/weekly)
- Export backup to external location
- Import backup from file picker
- Backup encryption
- Cloud backup integration
- Backup compression
- Multi-database backup (if multiple databases used)
