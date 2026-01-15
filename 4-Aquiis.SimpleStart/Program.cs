using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Aquiis.Core.Constants;
using Aquiis.Core.Interfaces;
using Aquiis.Core.Interfaces.Services;
using Aquiis.SimpleStart.Shared.Services;
using Aquiis.SimpleStart.Shared.Authorization;
using Aquiis.SimpleStart.Extensions;
using Aquiis.Application.Services;
using Aquiis.Application.Services.Workflows;
using Aquiis.SimpleStart.Data;
using Aquiis.SimpleStart.Entities;
using ElectronNET.API;
using Microsoft.Extensions.Options;
using Aquiis.Application.Services.PdfGenerators;
using Aquiis.SimpleStart.Shared.Components.Account;


var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddScoped<FinancialReportService>();
builder.Services.AddScoped<FinancialReportPdfGenerator>();
builder.Services.AddScoped<ChecklistPdfGenerator>();
builder.Services.AddScoped<DatabaseBackupService>();
builder.Services.AddScoped<SchemaValidationService>();
builder.Services.AddScoped<LeaseWorkflowService>();

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
            var webConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrEmpty(webConnectionString))
            {
                var dbPath = webConnectionString
                    .Replace("Data Source=", "")
                    .Replace("DataSource=", "")
                    .Split(';')[0]
                    .Trim();
                
                if (!Path.IsPathRooted(dbPath))
                {
                    dbPath = Path.Combine(Directory.GetCurrentDirectory(), dbPath);
                }
                
                var stagedRestorePath = $"{dbPath}.restore_pending";
                
                // Check if there's a staged restore waiting
                if (File.Exists(stagedRestorePath))
                {
                    app.Logger.LogInformation("Found staged restore file for web mode, applying it now");
                    
                    // Clear SQLite connection pool
                    Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
                    
                    // Wait for connections to close
                    await Task.Delay(500);
                    
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
                    app.Logger.LogInformation("Staged restore applied successfully for web mode");
                }
            }
            
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

// Only use HTTPS redirection in web mode, not in Electron
if (!HybridSupport.IsElectronActive)
{
    app.UseHttpsRedirection();
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
await app.StartAsync();

// Open Electron window
if (HybridSupport.IsElectronActive)
{
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
