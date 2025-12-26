using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Aquiis.SimpleStart.Shared.Components.Account;
using Aquiis.SimpleStart.Infrastructure.Data;
using Aquiis.SimpleStart.Features.PropertyManagement;
using Aquiis.SimpleStart.Core.Constants;
using Aquiis.SimpleStart.Application.Services;
using Aquiis.SimpleStart.Application.Services.PdfGenerators;
using Aquiis.SimpleStart.Shared.Services;
using Aquiis.SimpleStart.Shared.Authorization;
using ElectronNET.API;
using Microsoft.Extensions.Options;
using Aquiis.SimpleStart.Application.Services.Workflows;

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

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

// Get database connection string (uses Electron user data path when running as desktop app)
var connectionString = HybridSupport.IsElectronActive 
    ? await ElectronPathService.GetConnectionStringAsync(builder.Configuration)
    : builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString), ServiceLifetime.Scoped);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => {

    // For desktop app, simplify registration (email confirmation can be enabled later via settings)
    options.SignIn.RequireConfirmedAccount = !HybridSupport.IsElectronActive;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

// Configure organization-based authorization
builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, OrganizationPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, OrganizationRoleAuthorizationHandler>();

builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("ApplicationSettings"));
builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();



// Configure cookie authentication
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    
    // For Electron desktop app, we can use longer cookie lifetime
    if (HybridSupport.IsElectronActive)
    {
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
    }
    
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
builder.Services.AddScoped<ChecklistService>();
builder.Services.AddScoped<ApplicationService>();
builder.Services.AddScoped<CalendarSettingsService>();
builder.Services.AddScoped<CalendarEventService>();
builder.Services.AddScoped<TenantConversionService>();
builder.Services.AddScoped<UserContextService>();
builder.Services.AddScoped<DocumentService>();
builder.Services.AddScoped<NoteService>();

// Workflow services
builder.Services.AddScoped<Aquiis.SimpleStart.Application.Services.Workflows.ApplicationWorkflowService>();
builder.Services.AddScoped<SecurityDepositService>();
builder.Services.AddScoped<OrganizationService>();
builder.Services.AddScoped<ElectronPathService>();
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
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var backupService = scope.ServiceProvider.GetRequiredService<DatabaseBackupService>();
    
    // For Electron, handle database initialization and migrations
    if (HybridSupport.IsElectronActive)
    {
        try
        {
            var pathService = scope.ServiceProvider.GetRequiredService<ElectronPathService>();
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
                
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    app.Logger.LogInformation("Found {Count} pending migrations", pendingMigrations.Count());
                    
                    // Create backup before migration using the backup service
                    var backupPath = await backupService.CreatePreMigrationBackupAsync();
                    if (backupPath != null)
                    {
                        app.Logger.LogInformation("Database backed up to {BackupPath}", backupPath);
                    }
                    
                    try
                    {
                        // Apply migrations
                        await context.Database.MigrateAsync();
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
                await context.Database.MigrateAsync();
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
                    
                    // Close all database connections
                    await context.Database.CloseConnectionAsync();
                    
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
            
            // Check if there are pending migrations
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            var isNewDatabase = !pendingMigrations.Any() && !(await context.Database.GetAppliedMigrationsAsync()).Any();
            
            if (pendingMigrations.Any())
            {
                // Create backup before migration
                var backupPath = await backupService.CreatePreMigrationBackupAsync();
                if (backupPath != null)
                {
                    app.Logger.LogInformation("Database backed up to {BackupPath}", backupPath);
                }
            }
            
            await context.Database.MigrateAsync();
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
        // DO NOT create UserOrganizations entries
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
