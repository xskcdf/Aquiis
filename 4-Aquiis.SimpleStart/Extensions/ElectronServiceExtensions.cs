using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore;
using Aquiis.Core.Interfaces;
using Aquiis.Core.Interfaces.Services;
using Aquiis.Application;  // ✅ Application facade
using Aquiis.Application.Services;
using Aquiis.Infrastructure.Data;  // For SqlCipherConnectionInterceptor
using Aquiis.SimpleStart.Data;
using Aquiis.SimpleStart.Entities;
using Aquiis.SimpleStart.Services;
using Microsoft.Data.Sqlite;

namespace Aquiis.SimpleStart.Extensions;

/// <summary>
/// Extension methods for configuring Electron-specific services for SimpleStart.
/// </summary>
public static class ElectronServiceExtensions
{
    // Toggle for verbose logging (useful for troubleshooting encryption setup)
    private const bool EnableVerboseLogging = true;
    
    /// <summary>
    /// Adds all Electron-specific infrastructure services including database, identity, and path services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddElectronServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Register path service
        services.AddScoped<IPathService, ElectronPathService>();

        // Get connection string using the path service (synchronous to avoid startup deadlock)
        var connectionString = GetElectronConnectionString(configuration);

        // Check if database is encrypted and retrieve password if needed
        var encryptionPassword = WebServiceExtensions.GetEncryptionPasswordIfNeeded(connectionString);

        // Register encryption status as singleton for use during startup
        services.AddSingleton(new EncryptionDetectionResult
        {
            IsEncrypted = !string.IsNullOrEmpty(encryptionPassword)
        });

        // CRITICAL: Create interceptor instance BEFORE any DbContext registration
        // This single instance will be used by all DbContexts
        SqlCipherConnectionInterceptor? interceptor = null;
        if (!string.IsNullOrEmpty(encryptionPassword))
        {
            interceptor = new SqlCipherConnectionInterceptor(encryptionPassword);
            
            // Clear connection pools to ensure no connections bypass the interceptor
            SqliteConnection.ClearAllPools();
            if (EnableVerboseLogging)
                Console.WriteLine("[ElectronServiceExtensions] Encryption interceptor created and connection pools cleared");
        }

        // ✅ Register Application layer (includes Infrastructure internally) with encryption interceptor
        services.AddApplication(connectionString, encryptionPassword, interceptor);

        // Register Identity database context (SimpleStart-specific) with encryption interceptor
        services.AddDbContext<SimpleStartDbContext>((serviceProvider, options) =>
        {
            options.UseSqlite(connectionString);
            if (interceptor != null)
            {
                options.AddInterceptors(interceptor);
            }
        });
        
        // CRITICAL: Clear connection pools again after DbContext registration
        if (!string.IsNullOrEmpty(encryptionPassword))
        {
            SqliteConnection.ClearAllPools();
            if (EnableVerboseLogging)
                Console.WriteLine("[ElectronServiceExtensions] Connection pools cleared after DbContext registration");
        }

        // Register DatabaseService now that both contexts are available
        services.AddScoped<IDatabaseService>(sp => 
            new DatabaseService(
                sp.GetRequiredService<Aquiis.Infrastructure.Data.ApplicationDbContext>(),
                sp.GetRequiredService<SimpleStartDbContext>(),
                sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<DatabaseService>>()));

        services.AddDatabaseDeveloperPageExceptionFilter();

        // Configure Identity with Electron-specific settings
        services.AddIdentity<ApplicationUser, IdentityRole>(options => {
            // For desktop app, simplify registration (email confirmation can be enabled later via settings)
            options.SignIn.RequireConfirmedAccount = false; // Electron mode
            
            // ✅ SECURITY: Strong password policy (12+ chars, special characters required)
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 12;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequiredUniqueChars = 4; // Prevent patterns like "aaa111!!!"
        })
        .AddEntityFrameworkStores<SimpleStartDbContext>()
        .AddDefaultTokenProviders();

        // Configure cookie authentication for Electron
        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/Account/Logout";
            options.AccessDeniedPath = "/Account/AccessDenied";
            
            // For Electron desktop app, use longer cookie lifetime
            options.ExpireTimeSpan = TimeSpan.FromDays(30);
            options.SlidingExpiration = true;
            
            // Ensure cookie is persisted (not session-only)
            options.Cookie.MaxAge = TimeSpan.FromDays(30);
            options.Cookie.IsEssential = true;
            
            // For localhost Electron app, allow non-HTTPS cookies
            options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest;
            options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
        });

        return services;
    }

    /// <summary>
    /// Gets the connection string for Electron mode using the path service synchronously.
    /// This avoids deadlocks during service registration before Electron is fully initialized.
    /// </summary>
    private static string GetElectronConnectionString(IConfiguration configuration)
    {
        var pathService = new ElectronPathService(configuration);
        var dbPath = pathService.GetDatabasePathSync();
        return $"DataSource={dbPath};Cache=Shared";
    }
}
