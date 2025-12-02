# Electron.NET Conversion Outline

## Phase 1: Setup & Dependencies

1. **Install Electron.NET CLI**

   ```bash
   dotnet tool install ElectronNET.CLI -g
   ```

2. **Add Electron.NET NuGet Package**

   ```bash
   dotnet add package ElectronNET.API
   ```

3. **Initialize Electron**
   ```bash
   electronize init
   ```
   This creates `electron.manifest.json` for app configuration (name, icon, build targets)

## Phase 2: Application Configuration

4. **Modify Program.cs**

   - Add `builder.WebHost.UseElectron(args)`
   - Add `await app.StartAsync()` before Electron window creation
   - Create Electron browser window with `Electron.WindowManager.CreateWindowAsync()`
   - Configure window options (size, title, etc.)

5. **Update appsettings.json**

   - Change database path to use Electron app data directory
   - Adjust URLs to use `localhost` with dynamic ports

6. **Adjust launchSettings.json**
   - Add Electron profile for development

## Phase 3: Architecture Adjustments

7. **Authentication Changes**

   - **Option A**: Auto-login single user (simpler for single-user desktop)
   - **Option B**: Keep multi-user with login screen
   - Remove/adjust cookie settings for local-only access
   - Consider removing email confirmation requirements

8. **Database Path Management**

   - Move SQLite database to Electron's user data directory
   - Use `Electron.App.GetPath("userData")` for database location
   - Ensure migrations run on first launch

9. **File Storage**
   - Update `DocumentService` paths to use Electron app data directories
   - Ensure proper permissions for file operations

## Phase 4: Desktop-Specific Features

10. **Menu & Window Management**

    - Create native menu bar (File, Edit, View, Help)
    - Add keyboard shortcuts
    - Handle window close/minimize/maximize events

11. **System Tray Integration** (optional)

    - Add tray icon for background operation
    - Quick access menu from system tray

12. **Notifications**
    - Replace browser notifications with Electron native notifications
    - Payment reminders, lease expiration alerts

## Phase 5: Build & Distribution

13. **Development Testing**

    ```bash
    electronize start
    ```

    Test all features in desktop environment

14. **Production Build**

    ```bash
    # Windows
    electronize build /target win

    # macOS
    electronize build /target osx

    # Linux
    electronize build /target linux
    ```

15. **Create Installers**

    - Windows: `.exe` installer with NSIS
    - macOS: `.dmg` installer
    - Linux: `.AppImage`, `.deb`, or `.rpm`

16. **Code Signing** (for production)
    - Sign Windows executables
    - Sign macOS app bundle
    - Required for distribution outside development

## Phase 6: Polish & Optimization

17. **Auto-Update Implementation**

    - Use Electron's auto-updater
    - Check for updates on startup
    - Download and install updates in background

18. **Error Handling**

    - Add crash reporter
    - Log to file in user data directory
    - Graceful error messages for desktop context

19. **Performance**

    - Optimize bundle size
    - Lazy load components
    - Consider SQLite connection pooling

20. **Multi-Instance Prevention**
    - Ensure only one instance runs at a time
    - Focus existing window if already running

## Key Files Modified

- `Program.cs` - Add Electron initialization
- `appsettings.json` - Database paths, URLs
- `electron.manifest.json` - App metadata, build config
- `ApplicationDbContext.cs` - Dynamic database path
- `ScheduledTaskService.cs` - May need wake/sleep handling

## Testing Checklist

- [ ] Database migrations on fresh install
- [ ] All CRUD operations work
- [ ] Background services run correctly
- [ ] File uploads/downloads work
- [ ] PDF generation works
- [ ] Multi-window behavior
- [ ] App restart after updates
- [ ] Database backup/restore

## Estimated Effort

- **Basic conversion**: 2-4 hours
- **Authentication adjustments**: 1-2 hours
- **Desktop polish**: 2-4 hours
- **Build/distribution setup**: 2-3 hours
- **Total**: ~1-2 days for full conversion

## Architecture Notes

### Why Electron.NET?

This ASP.NET Core Blazor Server application is well-suited for Electron.NET conversion because:

- **Minimal code changes required** - Keep existing Blazor Server architecture
- **SQLite database** - Already file-based and portable
- **All services stay the same** - PropertyManagementService, UserContextService, etc.
- **Background services work** - ScheduledTaskService continues to function
- **Just add packaging** - Electron wraps the app, Kestrel runs internally

### Key Considerations

**Database:**

- SQLite `app.db` is already portable - perfect for desktop apps
- Move to Electron's user data directory for proper desktop behavior
- Handle migrations automatically on app startup

**Authentication:**

- Current multi-tenant system may need adjustment for desktop
- Consider auto-login for single-user desktop scenario
- Or keep multi-user with login screen for shared computers

**Services:**

- Background services like `ScheduledTaskService` work fine
- May need to handle system sleep/wake events

**File Paths:**

- Use Electron's app data directory for database location
- Update DocumentService to use proper desktop paths
- Ensure cross-platform path handling (Windows vs macOS vs Linux)

**Distribution:**

- Larger app size (~150-200MB due to bundled .NET runtime + Chromium)
- Need to handle database migrations on app updates
- Auto-update mechanism recommended for production
- Code signing required for distribution

### Single-User vs Multi-Tenant

**Current State:** Multi-tenant with OrganizationId-based data partitioning

**Desktop Options:**

1. **Single-User Mode** (Recommended for simplicity)

   - Auto-login on app start
   - One organization per installation
   - Simpler user experience
   - No login screen needed

2. **Multi-User Mode** (Keep current architecture)
   - Keep login screen
   - Multiple users can share one computer
   - Useful for property management offices
   - More complex but more flexible

Choose based on primary use case.
