using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Aquiis.SimpleStart.Components;
using Aquiis.SimpleStart.Components.Account;
using Aquiis.SimpleStart.Data;
using Aquiis.SimpleStart.Components.PropertyManagement;
using Aquiis.SimpleStart.Components.Administration.Application;
using Aquiis.SimpleStart.Services;
using ElectronNET.API;
using Microsoft.Extensions.Options;

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
    ? await ElectronPathService.GetConnectionStringAsync()
    : builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString, b => b.MigrationsAssembly("Aquiis.SimpleStart")));
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString, b => b.MigrationsAssembly("Aquiis.SimpleStart")), ServiceLifetime.Scoped);
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
builder.Services.AddScoped<ApplicationService>();
builder.Services.AddScoped<UserContextService>();
builder.Services.AddSingleton<ToastService>();
builder.Services.AddScoped<LeaseRenewalPdfGenerator>();
builder.Services.AddScoped<FinancialReportService>();
builder.Services.AddScoped<FinancialReportPdfGenerator>();
builder.Services.AddScoped<DatabaseBackupService>();
builder.Services.AddScoped<SchemaValidationService>();

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
            var dbPath = await ElectronPathService.GetDatabasePathAsync();
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
            
            // Check if there are pending migrations
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
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
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "Failed to apply database migrations");
            throw;
        }
    }

    // Seed roles
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = ApplicationConstants.DefaultRoles;
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Add Admin user
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var superAdminEmail = ApplicationConstants.SuperAdminEmail;
    var adminUser = await userManager.FindByEmailAsync(superAdminEmail);
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = superAdminEmail,
            Email = superAdminEmail,
            EmailConfirmed = true
        };
        await userManager.CreateAsync(adminUser, ApplicationConstants.DefaultSuperAdminPassword);
        await userManager.AddToRoleAsync(adminUser, ApplicationConstants.DefaultSuperAdminRole);
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
app.MapRazorComponents<Aquiis.SimpleStart.Components.App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

// Start the app for Electron
await app.StartAsync();

// Open Electron window
if (HybridSupport.IsElectronActive)
{
    var window = await Electron.WindowManager.CreateWindowAsync(new ElectronNET.API.Entities.BrowserWindowOptions
    {
        Width = 1400,
        Height = 900,
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
