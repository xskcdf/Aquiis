using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Aquiis.Core.Constants;
using Aquiis.Core.Entities;
using Aquiis.Core.Interfaces;
using Aquiis.Core.Interfaces.Services;
using Aquiis.SimpleStart.Extensions;
using Aquiis.SimpleStart.Shared.Services;
using Aquiis.SimpleStart.Shared.Authorization;
using Aquiis.Application.Services;
using Aquiis.Application.Services.Workflows;
using Aquiis.SimpleStart.Data;
using Aquiis.SimpleStart.Entities;
using ElectronNET.API;
using Microsoft.Extensions.Options;
using Aquiis.Application.Services.PdfGenerators;
using Aquiis.SimpleStart.Shared.Components.Account;
using Aquiis.Infrastructure.Services;

// Initialize SQLCipher before any database operations
SQLitePCL.Batteries_V2.Init();
SQLitePCL.raw.sqlite3_initialize();

var builder = WebApplication.CreateBuilder(args);

// CRITICAL: Handle .restore_pending BEFORE any DbContext registration
// This ensures encrypted database detection happens on the correct file
HandlePendingRestore(builder.Configuration);

// Configure for Electron
builder.WebHost.UseElectron(args);

// Configure URLs - use specific port for Electron
if (HybridSupport.IsElectronActive)
{
    builder.WebHost.UseUrls("http://localhost:8888");
}



// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add antiforgery services with options for Blazor
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    // Allow cookies over HTTP for Electron/Development
    if (HybridSupport.IsElectronActive || builder.Environment.IsDevelopment())
    {
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    }
});


    //Added for session state
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
    

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

// Add platform-specific infrastructure services (Database, Identity, Path services)
if (HybridSupport.IsElectronActive)
{
    builder.Services.AddElectronServices(builder.Configuration);
}
else
{
    builder.Services.AddWebServices(builder.Configuration);
}

// Configure organization-based authorization
builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, OrganizationPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, OrganizationRoleAuthorizationHandler>();

builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("ApplicationSettings"));
builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

// Configure cookie authentication events (cookie lifetime already configured in extension methods)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnSignedIn = async context =>
    {
        // Track user login
        if (context.Principal != null)
        {
            var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
            var user = await userManager.GetUserAsync(context.Principal);
            if (user != null)
            {
                user.PreviousLoginDate = user.LastLoginDate;
                user.LastLoginDate = DateTime.UtcNow;
                user.LoginCount++;
                user.LastLoginIP = context.HttpContext.Connection.RemoteIpAddress?.ToString();
                await userManager.UpdateAsync(user);
            }
        }
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        // Check if user is locked out and redirect to lockout page
        if (context.HttpContext.User.Identity?.IsAuthenticated == true)
        {
            var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
            var user = userManager.GetUserAsync(context.HttpContext.User).Result;
            if (user != null && userManager.IsLockedOutAsync(user).Result)
            {
                context.Response.Redirect("/Account/Lockout");
                return Task.CompletedTask;
            }
        }
        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };
});

builder.Services.AddScoped<PropertyManagementService>();
builder.Services.AddScoped<PropertyService>(); // New refactored service
builder.Services.AddScoped<TenantService>(); // New refactored service
builder.Services.AddScoped<LeaseService>(); // New refactored service
builder.Services.AddScoped<DocumentService>(); // New refactored service
builder.Services.AddScoped<InvoiceService>(); // New refactored service
builder.Services.AddScoped<PaymentService>(); // New refactored service
builder.Services.AddScoped<MaintenanceService>(); // New refactored service
builder.Services.AddScoped<InspectionService>(); // New refactored service
builder.Services.AddScoped<TourService>(); // New refactored service
builder.Services.AddScoped<ProspectiveTenantService>(); // New refactored service
builder.Services.AddScoped<RentalApplicationService>(); // New refactored service
builder.Services.AddScoped<ScreeningService>(); // New refactored service
builder.Services.AddScoped<LeaseOfferService>(); // New refactored service
builder.Services.AddScoped<ChecklistService>();
builder.Services.AddScoped<ApplicationService>();
builder.Services.AddScoped<CalendarSettingsService>();
builder.Services.AddScoped<CalendarEventService>(); // Concrete class for services that need it
builder.Services.AddScoped<ICalendarEventService>(sp => sp.GetRequiredService<CalendarEventService>()); // Interface alias
builder.Services.AddScoped<TenantConversionService>();
builder.Services.AddScoped<UserContextService>(); // Concrete class for components that need it
builder.Services.AddScoped<IUserContextService>(sp => sp.GetRequiredService<UserContextService>()); // Interface alias
builder.Services.AddScoped<NoteService>();
// Add to service registration section
builder.Services.AddScoped<NotificationService>();

// Phase 2.4: Notification Infrastructure
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISMSService, SMSService>();

// Phase 2.5: Email/SMS Integration
builder.Services.AddScoped<EmailSettingsService>();
builder.Services.AddScoped<SMSSettingsService>();
// SendGridEmailService and TwilioSMSService registered in extension methods

// Workflow services
builder.Services.AddScoped<ApplicationWorkflowService>();
builder.Services.AddScoped<SecurityDepositService>();
builder.Services.AddScoped<OrganizationService>();
builder.Services.AddSingleton<ToastService>();
builder.Services.AddSingleton<ThemeService>();
builder.Services.AddScoped<LeaseRenewalPdfGenerator>();
builder.Services.AddScoped<FinancialReportService>(); // Professional edition uses MaintenanceRequests
builder.Services.AddScoped<SimpleStartFinancialReportService>(); // SimpleStart uses Repairs
builder.Services.AddScoped<FinancialReportPdfGenerator>();
builder.Services.AddScoped<ChecklistPdfGenerator>();
builder.Services.AddScoped<DatabaseBackupService>();
builder.Services.AddScoped<SchemaValidationService>();
builder.Services.AddScoped<LeaseWorkflowService>();

// Database encryption services
builder.Services.AddScoped<PasswordDerivationService>();
builder.Services.AddScoped<LinuxKeychainService>();
builder.Services.AddScoped<DatabaseEncryptionService>();
builder.Services.AddScoped<DatabasePasswordService>();

// Configure and register session timeout service
builder.Services.AddScoped<SessionTimeoutService>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var service = new SessionTimeoutService();
    
    // Load configuration
    var timeoutMinutes = config.GetValue<int>("SessionTimeout:InactivityTimeoutMinutes", 30);
    var warningMinutes = config.GetValue<int>("SessionTimeout:WarningDurationMinutes", 2);
    var enabled = config.GetValue<bool>("SessionTimeout:Enabled", true);
    
    // Disable for Electron in development, or use longer timeout
    if (HybridSupport.IsElectronActive)
    {
        timeoutMinutes = 120; // 2 hours for desktop app
        enabled = false; // Typically disabled for desktop
    }
    
    service.InactivityTimeout = TimeSpan.FromMinutes(timeoutMinutes);
    service.WarningDuration = TimeSpan.FromMinutes(warningMinutes);
    service.IsEnabled = enabled;
    
    return service;
});

// Register background service for scheduled tasks
builder.Services.AddHostedService<ScheduledTaskService>();

var app = builder.Build();

// Ensure database is created and migrations are applied
using (var scope = app.Services.CreateScope())
{
    // Get services
    var dbService = scope.ServiceProvider.GetRequiredService<IDatabaseService>();
    var identityContext = scope.ServiceProvider.GetRequiredService<SimpleStartDbContext>();
    var backupService = scope.ServiceProvider.GetRequiredService<DatabaseBackupService>();
    
    // For Electron, handle database initialization and migrations
    if (HybridSupport.IsElectronActive)
    {
        try
        {
            var pathService = scope.ServiceProvider.GetRequiredService<IPathService>();
            var dbPath = await pathService.GetDatabasePathAsync();
            
            // ✅ v1.1.0: Automatic migration from old Electron folder to new Aquiis folder
            var basePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
            {
                basePath = Environment.GetEnvironmentVariable("HOME")!;
                basePath = OperatingSystem.IsLinux() 
                    ? Path.Combine(basePath, ".config") 
                    : Path.Combine(basePath, "Library/Application Support");
            }
            
            var dbFileName = Path.GetFileName(dbPath);
            var oldDbPath = Path.Combine(basePath, "Electron", dbFileName);
            var oldBackupPath = Path.Combine(basePath, "Electron", "Backups");
            var newBackupPath = Path.Combine(Path.GetDirectoryName(dbPath)!, "Backups");
            
            // One-time migration: copy database and backups if old location exists and new doesn't
            if (File.Exists(oldDbPath) && !File.Exists(dbPath))
            {
                app.Logger.LogInformation("Migrating database from Electron folder to Aquiis folder");
                app.Logger.LogInformation("Old path: {OldPath}", oldDbPath);
                app.Logger.LogInformation("New path: {NewPath}", dbPath);
                
                // Ensure destination directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
                
                // Copy database file
                File.Copy(oldDbPath, dbPath);
                app.Logger.LogInformation("Database file migrated successfully");
                
                // Copy backups folder if it exists
                if (Directory.Exists(oldBackupPath))
                {
                    app.Logger.LogInformation("Migrating backups folder");
                    Directory.CreateDirectory(newBackupPath);
                    
                    var backupFiles = Directory.GetFiles(oldBackupPath);
                    foreach (var backupFile in backupFiles)
                    {
                        var destFile = Path.Combine(newBackupPath, Path.GetFileName(backupFile));
                        File.Copy(backupFile, destFile);
                    }
                    
                    app.Logger.LogInformation("Migrated {Count} backup files", backupFiles.Length);
                }
                
                app.Logger.LogInformation("Database migration from Electron to Aquiis folder completed successfully");
            }
            
            var stagedRestorePath = $"{dbPath}.restore_pending";
            
            // Check if there's a staged restore waiting
            if (File.Exists(stagedRestorePath))
            {
                app.Logger.LogInformation("Found staged restore file, applying it now");
                
                // Backup current database if it exists
                if (File.Exists(dbPath))
                {
                    var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    var beforeRestorePath = $"{dbPath}.beforeRestore.{timestamp}";
                    File.Move(dbPath, beforeRestorePath);
                    app.Logger.LogInformation("Current database backed up to: {Path}", beforeRestorePath);
                }
                
                // Move staged restore into place
                File.Move(stagedRestorePath, dbPath);
                app.Logger.LogInformation("Staged restore applied successfully");
            }
            
            var dbExists = File.Exists(dbPath);
            
            // Check database health if it exists
            if (dbExists)
            {
                var (isHealthy, healthMessage) = await backupService.ValidateDatabaseHealthAsync();
                if (!isHealthy)
                {
                    app.Logger.LogWarning("Database health check failed: {Message}", healthMessage);
                    app.Logger.LogWarning("Attempting automatic recovery from corruption");
                    
                    var (recovered, recoveryMessage) = await backupService.AutoRecoverFromCorruptionAsync();
                    if (recovered)
                    {
                        app.Logger.LogInformation("Database recovered successfully: {Message}", recoveryMessage);
                    }
                    else
                    {
                        app.Logger.LogError("Database recovery failed: {Message}", recoveryMessage);
                        
                        // Instead of throwing, rename corrupted database and create new one
                        var corruptedPath = $"{dbPath}.corrupted.{DateTime.Now:yyyyMMddHHmmss}";
                        File.Move(dbPath, corruptedPath);
                        app.Logger.LogWarning("Corrupted database moved to: {CorruptedPath}", corruptedPath);
                        app.Logger.LogInformation("Creating new database...");
                        
                        dbExists = false; // Treat as new installation
                    }
                }
            }
            
            if (dbExists)
            {
                // Existing installation - apply any pending migrations
                app.Logger.LogInformation("Checking for migrations on existing database at {DbPath}", dbPath);
                
                // Check pending migrations for both contexts
                var businessPendingCount = await dbService.GetPendingMigrationsCountAsync();
                var identityPendingCount = await dbService.GetIdentityPendingMigrationsCountAsync();
                
                if (businessPendingCount > 0 || identityPendingCount > 0)
                {
                    var totalCount = businessPendingCount + identityPendingCount;
                    app.Logger.LogInformation("Found {Count} pending migrations ({BusinessCount} business, {IdentityCount} identity)", 
                        totalCount, businessPendingCount, identityPendingCount);
                    
                    // Create backup before migration using the backup service
                    var backupPath = await backupService.CreatePreMigrationBackupAsync();
                    if (backupPath != null)
                    {
                        app.Logger.LogInformation("Database backed up to {BackupPath}", backupPath);
                    }
                    
                    try
                    {
                        // Apply migrations using DatabaseService
                        await dbService.InitializeAsync();
                        
                        app.Logger.LogInformation("Migrations applied successfully");
                        
                        // Verify database health after migration
                        var (isHealthy, healthMessage) = await backupService.ValidateDatabaseHealthAsync();
                        if (!isHealthy)
                        {
                            app.Logger.LogError("Database corrupted after migration: {Message}", healthMessage);
                            
                            if (backupPath != null)
                            {
                                app.Logger.LogInformation("Rolling back to pre-migration backup");
                                await backupService.RestoreFromBackupAsync(backupPath);
                            }
                            
                            throw new Exception($"Migration caused database corruption: {healthMessage}");
                        }
                    }
                    catch (Exception migrationEx)
                    {
                        app.Logger.LogError(migrationEx, "Migration failed, attempting to restore from backup");
                        
                        if (backupPath != null)
                        {
                            var restored = await backupService.RestoreFromBackupAsync(backupPath);
                            if (restored)
                            {
                                app.Logger.LogInformation("Database restored from pre-migration backup");
                            }
                        }
                        
                        throw;
                    }
                }
                else
                {
                    app.Logger.LogInformation("Database is up to date");
                }
            }
            else
            {
                // New installation - create database with migrations
                app.Logger.LogInformation("Creating new database for Electron app at {DbPath}", dbPath);
                
                // Apply migrations using DatabaseService
                await dbService.InitializeAsync();
                
                app.Logger.LogInformation("Database created successfully");
                
                // Create initial backup after database creation
                await backupService.CreateBackupAsync("InitialSetup");
            }
            
            // Update DatabaseSettings.DatabaseEncryptionEnabled flag to match actual encryption status
            var encryptionDetection = scope.ServiceProvider.GetRequiredService<EncryptionDetectionResult>();
            var currentSettings = await dbService.GetDatabaseSettingsAsync();
            
            if (currentSettings.DatabaseEncryptionEnabled != encryptionDetection.IsEncrypted)
            {
                app.Logger.LogInformation(
                    "Updating DatabaseSettings.DatabaseEncryptionEnabled from {Old} to {New} (detected actual status)",
                    currentSettings.DatabaseEncryptionEnabled,
                    encryptionDetection.IsEncrypted);
                await dbService.SetDatabaseEncryptionAsync(encryptionDetection.IsEncrypted, "System-AutoDetect");
            }
            else
            {
                app.Logger.LogInformation(
                    "DatabaseSettings.DatabaseEncryptionEnabled already matches actual encryption status: {Status}",
                    encryptionDetection.IsEncrypted);
            }
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "Failed to initialize database for Electron");
            throw;
        }
    }
    else
    {
        // Web mode - ensure migrations are applied
        try
        {
            app.Logger.LogInformation("Applying database migrations for web mode");
            
            // Get database path for web mode
            // REMOVED: .restore_pending handling now happens BEFORE service registration
            // This ensures encrypted database detection works correctly
            // See HandlePendingRestore() called before AddWebServices()
            
            // Check if there are pending migrations for both contexts
            var businessPendingCount = await dbService.GetPendingMigrationsCountAsync();
            var identityPendingCount = await dbService.GetIdentityPendingMigrationsCountAsync();
            
            var isNewDatabase = businessPendingCount == 0 && identityPendingCount == 0;
            
            if (businessPendingCount > 0 || identityPendingCount > 0)
            {
                var totalCount = businessPendingCount + identityPendingCount;
                app.Logger.LogInformation("Found {Count} pending migrations ({BusinessCount} business, {IdentityCount} identity)", 
                    totalCount, businessPendingCount, identityPendingCount);
                
                // Create backup before migration
                var backupPath = await backupService.CreatePreMigrationBackupAsync();
                if (backupPath != null)
                {
                    app.Logger.LogInformation("Database backed up to {BackupPath}", backupPath);
                }
            }
            
            // Apply migrations to both contexts
            if (identityPendingCount > 0 || businessPendingCount > 0)
            {
                app.Logger.LogInformation("Applying migrations ({Identity} identity, {Business} business)", 
                    identityPendingCount, businessPendingCount);
                await dbService.InitializeAsync();
            }
            
            app.Logger.LogInformation("Database migrations applied successfully");
            
            // Update DatabaseSettings.DatabaseEncryptionEnabled flag to match actual encryption status
            var encryptionDetection = scope.ServiceProvider.GetRequiredService<EncryptionDetectionResult>();
            var currentSettings = await dbService.GetDatabaseSettingsAsync();
            
            if (currentSettings.DatabaseEncryptionEnabled != encryptionDetection.IsEncrypted)
            {
                app.Logger.LogInformation(
                    "Updating DatabaseSettings.DatabaseEncryptionEnabled from {Old} to {New} (detected actual status)",
                    currentSettings.DatabaseEncryptionEnabled,
                    encryptionDetection.IsEncrypted);
                await dbService.SetDatabaseEncryptionAsync(encryptionDetection.IsEncrypted, "System-AutoDetect");
            }
            else
            {
                app.Logger.LogInformation(
                    "DatabaseSettings.DatabaseEncryptionEnabled already matches actual encryption status: {Status}",
                    encryptionDetection.IsEncrypted);
            }
            
            // Create initial backup after creating a new database
            if (isNewDatabase)
            {
                app.Logger.LogInformation("New database created, creating initial backup");
                await backupService.CreateBackupAsync("InitialSetup");
            }
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "Failed to apply database migrations");
            throw;
        }
    }

    // Validate and update schema version
    var schemaService = scope.ServiceProvider.GetRequiredService<SchemaValidationService>();
    var appSettings = scope.ServiceProvider.GetRequiredService<IOptions<ApplicationSettings>>().Value;
    
    app.Logger.LogInformation("Checking schema version...");
    var currentDbVersion = await schemaService.GetCurrentSchemaVersionAsync();
    app.Logger.LogInformation("Current database schema version: {Version}", currentDbVersion ?? "null");
    
    if (currentDbVersion == null)
    {
        // New database or table exists but empty - set initial schema version
        app.Logger.LogInformation("Setting initial schema version to {Version}", appSettings.SchemaVersion);
        await schemaService.UpdateSchemaVersionAsync(appSettings.SchemaVersion, "Initial schema version");
        app.Logger.LogInformation("Schema version initialized successfully");
    }
    else if (currentDbVersion != appSettings.SchemaVersion)
    {
        // Schema version mismatch - log warning but allow startup
        app.Logger.LogWarning("Schema version mismatch! Database: {DbVersion}, Application: {AppVersion}", 
            currentDbVersion, appSettings.SchemaVersion);
    }
    else
    {
        app.Logger.LogInformation("Schema version validated: {Version}", currentDbVersion);
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession();

// ✅ SECURITY: Content Security Policy and security headers
app.UseSecurityHeaders();

// ✅ SECURITY: HTTPS enforcement for production web mode
if (!HybridSupport.IsElectronActive)
{
    if (!app.Environment.IsDevelopment())
    {
        // Production: MUST use HTTPS
        app.UseHttpsRedirection();
        app.UseHsts();

        // Validate HTTPS is actually configured
        var httpsUrl = builder.Configuration["Kestrel:Endpoints:Https:Url"];
        if (string.IsNullOrEmpty(httpsUrl))
        {
            app.Logger.LogWarning(
                "HTTPS not configured in production. " +
                "Configure Kestrel:Endpoints:Https in appsettings.Production.json or set ASPNETCORE_URLS environment variable.");
        }
    }
    else
    {
        // Development: Optional HTTPS (for testing)
        var useHttps = builder.Configuration.GetValue<bool>("Development:UseHttps", false);
        if (useHttps)
        {
            app.UseHttpsRedirection();
            app.Logger.LogInformation("HTTPS enabled for development");
        }
        else
        {
            app.Logger.LogInformation("Running in development without HTTPS");
        }
    }
}

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<Aquiis.SimpleStart.Shared.App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

// Add session refresh endpoint for session timeout feature
app.MapPost("/api/session/refresh", async (HttpContext context) =>
{
    // Simply accessing the session refreshes it
    context.Session.SetString("LastRefresh", DateTime.UtcNow.ToString("O"));
    await Task.CompletedTask;
    return Results.Ok(new { success = true, timestamp = DateTime.UtcNow });
}).RequireAuthorization();

// Create system service account for background jobs
// Clear connection pool to ensure all connections use proper encryption interceptor
Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    
    var systemUser = await userManager.FindByIdAsync(ApplicationConstants.SystemUser.Id);
    if (systemUser == null)
    {
        systemUser = new ApplicationUser
        {
            Id = ApplicationConstants.SystemUser.Id,
            UserName = ApplicationConstants.SystemUser.Email, // UserName = Email in this system
            NormalizedUserName = ApplicationConstants.SystemUser.Email.ToUpperInvariant(),
            Email = ApplicationConstants.SystemUser.Email,
            NormalizedEmail = ApplicationConstants.SystemUser.Email.ToUpperInvariant(),
            EmailConfirmed = true,
            FirstName = ApplicationConstants.SystemUser.FirstName,
            LastName = ApplicationConstants.SystemUser.LastName,
            LockoutEnabled = true,  // CRITICAL: Account is locked by default
            LockoutEnd = DateTimeOffset.MaxValue,  // Locked until end of time
            AccessFailedCount = 0
        };
        
        // Create without password - cannot be used for login
        var result = await userManager.CreateAsync(systemUser);
        
        if (!result.Succeeded)
        {
            throw new Exception($"Failed to create system user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
        
        // DO NOT assign to any organization - service account is org-agnostic
        // DO NOT create OrganizationUsers entries
        // DO NOT set ActiveOrganizationId
    }
}

// Start the app for Electron
try
{
    app.Logger.LogInformation("Starting ASP.NET Core server...");
    await app.StartAsync();
    app.Logger.LogInformation("ASP.NET Core server started successfully");
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "FATAL: Failed to start ASP.NET Core server");
    Console.WriteLine($"FATAL ERROR: {ex.Message}");
    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
    }
    Environment.Exit(1);
}

// Open Electron window
if (HybridSupport.IsElectronActive)
{
    // Verify backend is responding before showing window
    var backendUrl = "http://localhost:8888";
    var isBackendReady = false;
    
    try
    {
        using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
        var response = await httpClient.GetAsync(backendUrl);
        isBackendReady = response.IsSuccessStatusCode;
        app.Logger.LogInformation("Backend health check: {Status}", isBackendReady ? "OK" : "Failed");
    }
    catch (Exception ex)
    {
        app.Logger.LogWarning(ex, "Backend health check failed, will show offline page");
    }
    
    var window = await Electron.WindowManager.CreateWindowAsync(new ElectronNET.API.Entities.BrowserWindowOptions
    {
        Width = 1400,
        Height = 900,
        MinWidth = 800,
        MinHeight = 600,
        Show = false
    });

    window.OnReadyToShow += () => window.Show();
    window.SetTitle("Aquiis Property Management");
    
    // Load appropriate page based on backend availability
    if (!isBackendReady)
    {
        app.Logger.LogWarning("Loading offline page due to backend unavailability");
        window.LoadURL($"{backendUrl}/offline.html");
    }
    
    // Open DevTools in development mode for debugging
    if (app.Environment.IsDevelopment())
    {
        window.WebContents.OpenDevTools();
        app.Logger.LogInformation("DevTools opened for debugging");
    }
    
    // Gracefully shutdown when window is closed
    window.OnClosed += () =>
    {
        app.Logger.LogInformation("Electron window closed, shutting down application");
        Electron.App.Quit();
    };
}

await app.WaitForShutdownAsync();

// Local function to handle .restore_pending before service registration
static void HandlePendingRestore(IConfiguration configuration)
{
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrEmpty(connectionString))
    {
        // Can't proceed without connection string
        return;
    }
    
    // Extract database path
    var builder = new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder(connectionString);
    var dbPath = builder.DataSource;
    
    if (!Path.IsPathRooted(dbPath))
    {
        dbPath = Path.Combine(Directory.GetCurrentDirectory(), dbPath);
    }
    
    var stagedRestorePath = $"{dbPath}.restore_pending";
    
    // Check if there's a staged restore waiting
    if (File.Exists(stagedRestorePath))
    {
        Console.WriteLine("Found staged restore file, applying it now...");
        
        // Clear SQLite connection pool
        Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
        
        // Wait for connections to close
        Thread.Sleep(500);
        
        // Backup current database if it exists
        if (File.Exists(dbPath))
        {
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            var beforeRestorePath = $"{dbPath}.beforeRestore.{timestamp}";
            File.Move(dbPath, beforeRestorePath);
            Console.WriteLine($"Current database backed up to: {beforeRestorePath}");
        }
        
        // Move staged restore into place
        File.Move(stagedRestorePath, dbPath);
        Console.WriteLine("Staged restore applied successfully");
        
        // Delete orphaned WAL/SHM files if they exist
        var walPath = $"{dbPath}-wal";
        var shmPath = $"{dbPath}-shm";
        if (File.Exists(walPath)) File.Delete(walPath);
        if (File.Exists(shmPath)) File.Delete(shmPath);
    }
}
