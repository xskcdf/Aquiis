using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace Aquiis.Infrastructure.Data;

/// <summary>
/// EF Core connection interceptor that sets SQLCipher encryption password after opening connections
/// </summary>
public class SqlCipherConnectionInterceptor : DbConnectionInterceptor
{
    private readonly string? _encryptionPassword;
    
    // Toggle for verbose logging (useful for troubleshooting encryption issues)
    private const bool EnableVerboseLogging = true;

    public SqlCipherConnectionInterceptor(string? encryptionPassword)
    {
        _encryptionPassword = encryptionPassword;
        if (EnableVerboseLogging)
            Console.WriteLine($"[SqlCipherConnectionInterceptor] Initialized with password: {(_encryptionPassword != null ? $"YES (length: {_encryptionPassword.Length})" : "NO")}");
    }

    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
    {
        if (EnableVerboseLogging)
            Console.WriteLine($"[SqlCipherConnectionInterceptor] ConnectionOpened called - Database: {connection.Database}");
        
        if (!string.IsNullOrEmpty(_encryptionPassword))
        {
            using (var cmd = connection.CreateCommand())
            {
                // CRITICAL: Set key FIRST, before any other PRAGMA commands
                if (EnableVerboseLogging)
                    Console.WriteLine("[SqlCipherConnectionInterceptor] Setting encryption key...");
                cmd.CommandText = $"PRAGMA key = '{_encryptionPassword}';";
                cmd.ExecuteNonQuery();
                
                // Now set SQLCipher 4 parameters
                if (EnableVerboseLogging)
                    Console.WriteLine("[SqlCipherConnectionInterceptor] Setting SQLCipher 4 parameters...");
                cmd.CommandText = "PRAGMA cipher_page_size = 4096;";
                cmd.ExecuteNonQuery();
                
                cmd.CommandText = "PRAGMA kdf_iter = 256000;";
                cmd.ExecuteNonQuery();
                
                cmd.CommandText = "PRAGMA cipher_hmac_algorithm = HMAC_SHA512;";
                cmd.ExecuteNonQuery();
                
                cmd.CommandText = "PRAGMA cipher_kdf_algorithm = PBKDF2_HMAC_SHA512;";
                cmd.ExecuteNonQuery();
                
                if (EnableVerboseLogging)
                    Console.WriteLine("[SqlCipherConnectionInterceptor] Encryption configured successfully");
            }
        }
        else if (EnableVerboseLogging)
        {
            Console.WriteLine("[SqlCipherConnectionInterceptor] No password provided, skipping encryption");
        }

        // Enable WAL mode for all connections (encrypted or not).
        // WAL mode is persistent in the database file — this is a no-op for databases
        // already in WAL mode, and upgrades fresh databases from the default DELETE journal.
        // NORMAL synchronous is safe with WAL and gives a meaningful performance boost.
        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = "PRAGMA journal_mode = WAL;";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "PRAGMA synchronous = NORMAL;";
            cmd.ExecuteNonQuery();
        }

        base.ConnectionOpened(connection, eventData);
    }

    public override async Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
    {
        if (EnableVerboseLogging)
            Console.WriteLine($"[SqlCipherConnectionInterceptor] ConnectionOpenedAsync called - Database: {connection.Database}");
        
        if (!string.IsNullOrEmpty(_encryptionPassword))
        {
            using (var cmd = connection.CreateCommand())
            {
                // CRITICAL: Set key FIRST, before any other PRAGMA commands
                if (EnableVerboseLogging)
                    Console.WriteLine("[SqlCipherConnectionInterceptor] Setting encryption key (async)...");
                cmd.CommandText = $"PRAGMA key = '{_encryptionPassword}';";
                await cmd.ExecuteNonQueryAsync(cancellationToken);
                
                // Now set SQLCipher 4 parameters
                if (EnableVerboseLogging)
                    Console.WriteLine("[SqlCipherConnectionInterceptor] Setting SQLCipher 4 parameters (async)...");
                cmd.CommandText = "PRAGMA cipher_page_size = 4096;";
                await cmd.ExecuteNonQueryAsync(cancellationToken);
                
                cmd.CommandText = "PRAGMA kdf_iter = 256000;";
                await cmd.ExecuteNonQueryAsync(cancellationToken);
                
                cmd.CommandText = "PRAGMA cipher_hmac_algorithm = HMAC_SHA512;";
                await cmd.ExecuteNonQueryAsync(cancellationToken);
                
                cmd.CommandText = "PRAGMA cipher_kdf_algorithm = PBKDF2_HMAC_SHA512;";
                await cmd.ExecuteNonQueryAsync(cancellationToken);
                
                if (EnableVerboseLogging)
                    Console.WriteLine("[SqlCipherConnectionInterceptor] Encryption configured successfully (async)");
            }
        }
        else if (EnableVerboseLogging)
        {
            Console.WriteLine("[SqlCipherConnectionInterceptor] No password provided, skipping encryption (async)");
        }

        // Enable WAL mode for all connections (encrypted or not).
        // WAL mode is persistent in the database file — this is a no-op for databases
        // already in WAL mode, and upgrades fresh databases from the default DELETE journal.
        // NORMAL synchronous is safe with WAL and gives a meaningful performance boost.
        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = "PRAGMA journal_mode = WAL;";
            await cmd.ExecuteNonQueryAsync(cancellationToken);
            cmd.CommandText = "PRAGMA synchronous = NORMAL;";
            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }

        await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
    }
}
