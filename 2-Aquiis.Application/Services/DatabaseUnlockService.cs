using Aquiis.Infrastructure.Interfaces;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace Aquiis.Application.Services;

/// <summary>
/// Service for unlocking encrypted databases by prompting user for password
/// and verifying it against the database
/// </summary>
public class DatabaseUnlockService
{
    private readonly IKeychainService _keychain;
    private readonly ILogger<DatabaseUnlockService> _logger;
    
    public DatabaseUnlockService(
        IKeychainService keychain,
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
    
    /// <summary>
    /// Archive encrypted database and create fresh database when password forgotten
    /// </summary>
    /// <param name="databasePath">Path to encrypted database</param>
    /// <returns>(Success, ArchivedPath, ErrorMessage)</returns>
    public async Task<(bool Success, string? ArchivedPath, string? ErrorMessage)> StartWithNewDatabaseAsync(
        string databasePath)
    {
        try
        {
            _logger.LogWarning("User requested new database - archiving encrypted database");

            // Create backups directory if it doesn't exist
            var dbDirectory = Path.GetDirectoryName(databasePath)!;
            var backupsDir = Path.Combine(dbDirectory, "Backups");
            Directory.CreateDirectory(backupsDir);

            // Generate archived filename with timestamp and .db extension for easy identification
            var dbFileNameWithoutExt = Path.GetFileNameWithoutExtension(databasePath);
            var timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var archivedPath = Path.Combine(backupsDir, $"{dbFileNameWithoutExt}.{timestamp}.encrypted.db");

            // Move encrypted database to backups
            File.Move(databasePath, archivedPath);
            _logger.LogInformation("Encrypted database archived to: {ArchivedPath}", archivedPath);

            // New unencrypted database will be created automatically on app restart
            // The app will detect no database exists and go through first-time setup
            return (true, archivedPath, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error archiving encrypted database");
            return (false, null, $"Error archiving database: {ex.Message}");
        }
    }
}
