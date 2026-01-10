using Aquiis.Infrastructure.Data;
using Aquiis.Infrastructure.Interfaces;
using Aquiis.Infrastructure.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Aquiis.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Register all Infrastructure services and data access.
    /// Called internally by Application layer - products should NOT call this directly.
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        // Register ApplicationDbContext (business data)
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString));
            
        // Register DbContext factory for services that need it (like FinancialReportService)
        // Use AddDbContextFactory instead of AddPooledDbContextFactory to avoid lifetime issues
        services.AddDbContextFactory<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString),
            ServiceLifetime.Scoped);

        // Register provider interfaces
        services.AddScoped<SendGridEmailService>();
        services.AddScoped<IEmailProvider>(sp => 
            sp.GetRequiredService<SendGridEmailService>());
            
        services.AddScoped<TwilioSMSService>();
        services.AddScoped<ISMSProvider>(sp => 
            sp.GetRequiredService<TwilioSMSService>());

        return services;
    }
}
