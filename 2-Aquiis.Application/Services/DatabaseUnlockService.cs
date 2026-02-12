using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Aquiis.Infrastructure.Services;

namespace Aquiis.Application.Services;

/// <summary>
/// Service for unlocking encrypted databases by prompting user for password
/// and verifying it against the database
/// </summary>
public class DatabaseUnlockService
{
    private readonly LinuxKeychainService _keychain;
    private readonly ILogger<DatabaseUnlockService> _logger;
    
    public DatabaseUnlockService(
        LinuxKeychainService keychain,
        ILogger<DatabaseUnlockService> logger)
    {
        _keychain = keychain;
        _logger = logger;
    }
    
    /// <summary>
    /// Verify password can decrypt the database and store in keychain
    /// </summary>
    /// <param name="connectionString">Database connection string</param>
    /// <param name="password">User-provided password to verify</param>
    /// <returns>(Success, ErrorMessage)</returns>
    public async Task<(bool Success, string? ErrorMessage)> UnlockDatabaseAsync(
        string connectionString, 
        string password)
    {
        try
        {
            _logger.LogInformation("Attempting database unlock verification");
            
            // Try to open database with password
            using var conn = new SqliteConnection($"{connectionString};Password={password}");
            await conn.OpenAsync();
            
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM sqlite_master;";
            await cmd.ExecuteScalarAsync();
            
            _logger.LogInformation("Password verification successful");
            
            // Store password in keychain for future use
            var stored = _keychain.StoreKey(password, "Aquiis Database Encryption Password");
            if (stored)
            {
                _logger.LogInformation("Password stored in keychain successfully");
            }
            else
            {
                _logger.LogWarning("Failed to store password in keychain");
            }
            
            return (true, null);
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == 26)
        {
            _logger.LogWarning("Incorrect password provided");
            return (false, "Incorrect password. Please try again.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unlocking database");
            return (false, $"Error unlocking database: {ex.Message}");
        }
    }
}
