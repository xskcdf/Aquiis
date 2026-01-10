namespace Aquiis.Core.Interfaces.Services;

/// <summary>
/// Service for database initialization and management operations.
/// Abstracts database access from product layers.
/// </summary>
public interface IDatabaseService
{
    /// <summary>
    /// Initialize database (apply pending migrations for both business and identity contexts)
    /// </summary>
    Task InitializeAsync();
    
    /// <summary>
    /// Check if database can be connected to
    /// </summary>
    Task<bool> CanConnectAsync();
    
    /// <summary>
    /// Get count of pending migrations for business context
    /// </summary>
    Task<int> GetPendingMigrationsCountAsync();
    
    /// <summary>
    /// Get count of pending migrations for identity context
    /// </summary>
    Task<int> GetIdentityPendingMigrationsCountAsync();
}
