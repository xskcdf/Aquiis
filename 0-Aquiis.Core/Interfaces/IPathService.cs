namespace Aquiis.Core.Interfaces;

/// <summary>
/// Platform-agnostic interface for managing application paths and connection strings.
/// Implementations exist for Electron, Web, and future mobile platforms.
/// </summary>
public interface IPathService
{
    /// <summary>
    /// Gets the connection string for the database, using platform-specific paths.
    /// </summary>
    Task<string> GetConnectionStringAsync(object configuration);

    /// <summary>
    /// Gets the platform-specific database file path.
    /// </summary>
    Task<string> GetDatabasePathAsync();

    /// <summary>
    /// Gets the platform-specific user data directory path.
    /// </summary>
    Task<string> GetUserDataPathAsync();

    /// <summary>
    /// Checks if the application is running in the current platform context.
    /// </summary>
    bool IsActive { get; }
}
