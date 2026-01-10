using Aquiis.Core.Interfaces.Services;
using Aquiis.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Aquiis.Application.Services;

/// <summary>
/// Service for managing database initialization and migrations.
/// Handles both business (ApplicationDbContext) and product-specific Identity contexts.
/// </summary>
public class DatabaseService : IDatabaseService
{
    private readonly ApplicationDbContext _businessContext;
    private readonly DbContext _identityContext;  // Product-specific (SimpleStartDbContext or ProfessionalDbContext)
    private readonly ILogger<DatabaseService> _logger;

    public DatabaseService(
        ApplicationDbContext businessContext,
        DbContext identityContext,
        ILogger<DatabaseService> logger)
    {
        _businessContext = businessContext;
        _identityContext = identityContext;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        _logger.LogInformation("Checking for pending migrations...");
        
        // Check and apply identity migrations first
        var identityPending = await _identityContext.Database.GetPendingMigrationsAsync();
        if (identityPending.Any())
        {
            _logger.LogInformation($"Applying {identityPending.Count()} identity migrations...");
            await _identityContext.Database.MigrateAsync();
            _logger.LogInformation("Identity migrations applied successfully.");
        }
        else
        {
            _logger.LogInformation("No pending identity migrations.");
        }
        
        // Then check and apply business migrations
        var businessPending = await _businessContext.Database.GetPendingMigrationsAsync();
        if (businessPending.Any())
        {
            _logger.LogInformation($"Applying {businessPending.Count()} business migrations...");
            await _businessContext.Database.MigrateAsync();
            _logger.LogInformation("Business migrations applied successfully.");
        }
        else
        {
            _logger.LogInformation("No pending business migrations.");
        }
        
        _logger.LogInformation("Database initialization complete.");
    }

    public async Task<bool> CanConnectAsync()
    {
        try
        {
            var businessCanConnect = await _businessContext.Database.CanConnectAsync();
            var identityCanConnect = await _identityContext.Database.CanConnectAsync();
            
            return businessCanConnect && identityCanConnect;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to database");
            return false;
        }
    }

    public async Task<int> GetPendingMigrationsCountAsync()
    {
        var pending = await _businessContext.Database.GetPendingMigrationsAsync();
        return pending.Count();
    }

    public async Task<int> GetIdentityPendingMigrationsCountAsync()
    {
        var pending = await _identityContext.Database.GetPendingMigrationsAsync();
        return pending.Count();
    }
}
