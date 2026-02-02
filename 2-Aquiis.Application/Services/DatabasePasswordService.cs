using Aquiis.Core.Interfaces;
using Aquiis.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace Aquiis.Application.Services;

/// <summary>
/// Service for detecting and handling encrypted databases on application startup.
/// Manages password retrieval from keychain or user prompt.
/// </summary>
public class DatabasePasswordService
{
    private readonly LinuxKeychainService _keychain;
    private readonly ILogger<DatabasePasswordService> _logger;
    
    public DatabasePasswordService(
        LinuxKeychainService keychain,
        ILogger<DatabasePasswordService> logger)
    {
        _keychain = keychain;
        _logger = logger;
    }
    
    /// <summary>
    /// Check if a database file is encrypted by attempting to open it
    /// </summary>
    public async Task<bool> IsDatabaseEncryptedAsync(string dbPath)
    {
        if (!File.Exists(dbPath))
            return false;
        
        try
        {
            // Try to open without password
            using (var conn = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={dbPath}"))
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT count(*) FROM sqlite_master;";
                    await cmd.ExecuteScalarAsync();
                }
                return false; // Opened successfully = not encrypted
            }
        }
        catch (Microsoft.Data.Sqlite.SqliteException ex)
        {
            // SQLCipher error codes indicate encryption
            if (ex.Message.Contains("file is not a database") || 
                ex.Message.Contains("file is encrypted") ||
                ex.SqliteErrorCode == 26) // SQLITE_NOTADB
            {
                _logger.LogInformation("Database is encrypted");
                return true;
            }
            
            // Some other error - rethrow
            throw;
        }
    }
    
    /// <summary>
    /// Try to get database password from keychain
    /// </summary>
    public string? TryGetPasswordFromKeychain()
    {
        if (!OperatingSystem.IsLinux())
            return null;
        
        var key = _keychain.RetrieveKey();
        if (key != null)
        {
            _logger.LogInformation("Retrieved encryption key from keychain");
        }
        return key;
    }
    
    /// <summary>
    /// Verify that a password can decrypt the database
    /// </summary>
    public async Task<bool> VerifyPasswordAsync(string dbPath, string passwordHex)
    {
        try
        {
            using (var conn = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={dbPath};Password={passwordHex}"))
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT count(*) FROM sqlite_master;";
                    await cmd.ExecuteScalarAsync();
                }
                return true;
            }
        }
        catch
        {
            return false;
        }
    }
}
