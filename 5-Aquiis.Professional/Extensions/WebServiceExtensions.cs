using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore;
using Aquiis.Core.Interfaces;
using Aquiis.Core.Interfaces.Services;
using Aquiis.Application;  // ✅ Application facade
using Aquiis.Application.Services;
using Aquiis.Professional.Data;
using Aquiis.Professional.Entities;
using Aquiis.Professional.Services;

namespace Aquiis.Professional.Extensions;

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
        
        // Get connection string from configuration
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        // ✅ Register Application layer (includes Infrastructure internally)
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

        // Configure Identity with Web-specific settings
        services.AddIdentity<ApplicationUser, IdentityRole>(options => {
            // For web app, require confirmed email
            options.SignIn.RequireConfirmedAccount = true;
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
        })
        .AddEntityFrameworkStores<ProfessionalDbContext>()
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
