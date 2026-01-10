using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore;
using Aquiis.Core.Interfaces;
using Aquiis.Core.Interfaces.Services;
using Aquiis.Application;
using Aquiis.Application.Services;
using Aquiis.Professional.Data;
using Aquiis.Professional.Entities;
using Aquiis.Professional.Services;

namespace Aquiis.Professional.Extensions;

/// <summary>
/// Extension methods for configuring Electron-specific services for Professional.
/// </summary>
public static class ElectronServiceExtensions
{
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

        // Get connection string using the path service
        var connectionString = GetElectronConnectionString(configuration).GetAwaiter().GetResult();

        // Register Application layer (includes Infrastructure internally)
        services.AddApplication(connectionString);

        // Register Identity database context (Professional-specific)
        services.AddDbContext<ProfessionalDbContext>(options =>
            options.UseSqlite(connectionString));

        // Register DatabaseService now that both contexts are available
        services.AddScoped<IDatabaseService>(sp => 
            new DatabaseService(
                sp.GetRequiredService<Aquiis.Infrastructure.Data.ApplicationDbContext>(),
                sp.GetRequiredService<ProfessionalDbContext>(),
                sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<DatabaseService>>()));

        services.AddDatabaseDeveloperPageExceptionFilter();

        // Configure Identity with Electron-specific settings
        services.AddIdentity<ApplicationUser, IdentityRole>(options => {
            // For desktop app, simplify registration (email confirmation can be enabled later via settings)
            options.SignIn.RequireConfirmedAccount = false; // Electron mode
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
        })
        .AddEntityFrameworkStores<ProfessionalDbContext>()
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
        });

        return services;
    }

    /// <summary>
    /// Gets the connection string for Electron mode using the path service.
    /// </summary>
    private static async Task<string> GetElectronConnectionString(IConfiguration configuration)
    {
        var pathService = new ElectronPathService(configuration);
        return await pathService.GetConnectionStringAsync(configuration);
    }
}
