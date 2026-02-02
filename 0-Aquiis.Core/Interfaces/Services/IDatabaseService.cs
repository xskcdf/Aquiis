using Aquiis.Core.Entities;

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
    
    /// <summary>
    /// Get database settings (creates default if not exists)
    /// </summary>
    Task<DatabaseSettings> GetDatabaseSettingsAsync();
    
    /// <summary>
    /// Set database encryption status
    /// </summary>
    Task SetDatabaseEncryptionAsync(bool enabled, string modifiedBy = "System");
    
    /// <summary>
    /// Check if database encryption is currently enabled
    /// </summary>
    Task<bool> IsDatabaseEncryptionEnabledAsync();
}
