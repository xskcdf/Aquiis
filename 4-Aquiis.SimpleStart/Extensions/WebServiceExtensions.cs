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
using Aquiis.Infrastructure.Services;
using Microsoft.Data.Sqlite;

namespace Aquiis.SimpleStart.Extensions;

/// <summary>
/// Extension methods for configuring Web-specific services for SimpleStart.
/// </summary>
public static class WebServiceExtensions
{
    // Toggle for verbose logging (useful for troubleshooting encryption setup)
    private const bool EnableVerboseLogging = true;
    
    /// <summary>
    /// Adds all Web-specific infrastructure services including database and identity.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddWebServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Register path service
        services.AddScoped<IPathService, WebPathService>();
        
        // ✅ SECURITY: Get connection string from environment variable first (production),
        // then fall back to configuration (development)
        var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string not found. " +
                "Set DATABASE_CONNECTION_STRING environment variable or configure DefaultConnection in appsettings.json");

        // Check if database is encrypted and retrieve password if needed
        var encryptionPassword = GetEncryptionPasswordIfNeeded(connectionString);

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
                Console.WriteLine("Encryption interceptor created and connection pools cleared");
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
                Console.WriteLine("Connection pools cleared after DbContext registration");
        }

        // Register DatabaseService now that both contexts are available
        services.AddScoped<IDatabaseService>(sp => 
            new DatabaseService(
                sp.GetRequiredService<Aquiis.Infrastructure.Data.ApplicationDbContext>(),
                sp.GetRequiredService<SimpleStartDbContext>(),
                sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<DatabaseService>>()));

        services.AddDatabaseDeveloperPageExceptionFilter();

        // Configure Identity with Web-specific settings
        services.AddIdentity<ApplicationUser, IdentityRole>(options => {
            // For web app, require confirmed email
            options.SignIn.RequireConfirmedAccount = true;
            
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

        // Configure cookie authentication for Web
        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/Account/Logout";
            options.AccessDeniedPath = "/Account/AccessDenied";
        });

        return services;
    }

    /// <summary>
    /// Detects if database is encrypted and retrieves password from keychain if needed
    /// </summary>
    /// <returns>Encryption password, or null if database is not encrypted</returns>
    internal static string? GetEncryptionPasswordIfNeeded(string connectionString)
    {
        try
        {
            // Extract database path from connection string
            var builder = new SqliteConnectionStringBuilder(connectionString);
            var dbPath = builder.DataSource;

            if (!File.Exists(dbPath))
            {
                // Database doesn't exist yet, not encrypted
                return null;
            }

            // Try to open as plaintext
            try
            {
                using (var conn = new SqliteConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT COUNT(*) FROM sqlite_master;";
                        cmd.ExecuteScalar();
                    }
                }
                // Success - database is not encrypted
                return null;
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 26) // "file is not a database"
            {
                // Database is encrypted - try to get password from keychain
                if (EnableVerboseLogging)
                    Console.WriteLine("Detected encrypted database, retrieving password from keychain...");
                var keychain = new LinuxKeychainService();
                var password = keychain.RetrieveKey();

                if (string.IsNullOrEmpty(password))
                {
                    throw new InvalidOperationException(
                        "Database is encrypted but encryption password not found in keychain. " +
                        "Please restore from an unencrypted backup.");
                }

                if (EnableVerboseLogging)
                    Console.WriteLine($"Encryption password retrieved successfully (length: {password.Length} chars)");
                
                // CRITICAL: Clear connection pool to prevent reuse of unencrypted connections
                SqliteConnection.ClearAllPools();
                if (EnableVerboseLogging)
                    Console.WriteLine("Connection pool cleared to force encryption on all new connections");
                
                return password;
            }
        }
        catch (InvalidOperationException)
        {
            throw; // Re-throw our custom messages
        }
        catch (Exception ex)
        {
            // Log but don't fail - assume database is not encrypted
            Console.WriteLine($"Warning: Could not check database encryption status: {ex.Message}");
            return null;
        }
    }
}

/// <summary>
/// Tracks whether database encryption was detected during startup
/// </summary>
public class EncryptionDetectionResult
{
    public bool IsEncrypted { get; set; }
}
