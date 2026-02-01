using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore;
using Aquiis.Core.Interfaces;
using Aquiis.Core.Interfaces.Services;
using Aquiis.Application;  // ✅ Application facade
using Aquiis.Application.Services;
using Aquiis.SimpleStart.Data;
using Aquiis.SimpleStart.Entities;
using Aquiis.SimpleStart.Services;

namespace Aquiis.SimpleStart.Extensions;

/// <summary>
/// Extension methods for configuring Web-specific services for SimpleStart.
/// </summary>
public static class WebServiceExtensions
{
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

        // ✅ Register Application layer (includes Infrastructure internally)
        services.AddApplication(connectionString);

        // Register Identity database context (SimpleStart-specific)
        services.AddDbContext<SimpleStartDbContext>(options =>
            options.UseSqlite(connectionString));

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
}
