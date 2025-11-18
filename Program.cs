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

// Register background service for scheduled tasks
builder.Services.AddHostedService<ScheduledTaskService>();

var app = builder.Build();

// Ensure database is created and migrations are applied
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    // For Electron, handle database initialization and migrations
    if (HybridSupport.IsElectronActive)
    {
        try
        {
            var dbPath = await ElectronPathService.GetDatabasePathAsync();
            var dbExists = File.Exists(dbPath);
            
            if (dbExists)
            {
                // Existing installation - apply any pending migrations
                app.Logger.LogInformation("Checking for migrations on existing database at {DbPath}", dbPath);
                
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    app.Logger.LogInformation("Found {Count} pending migrations", pendingMigrations.Count());
                    
                    // Backup database before migration
                    var backupPath = $"{dbPath}.backup.{DateTime.Now:yyyyMMddHHmmss}";
                    File.Copy(dbPath, backupPath);
                    app.Logger.LogInformation("Database backed up to {BackupPath}", backupPath);
                    
                    // Apply migrations
                    await context.Database.MigrateAsync();
                    app.Logger.LogInformation("Migrations applied successfully");
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
            }
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "Failed to initialize database for Electron");
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
    
    // Gracefully shutdown when window is closed
    window.OnClosed += () =>
    {
        app.Logger.LogInformation("Electron window closed, shutting down application");
        Electron.App.Quit();
    };
}

await app.WaitForShutdownAsync();
