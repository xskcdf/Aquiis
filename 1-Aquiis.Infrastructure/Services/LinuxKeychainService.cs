using System.Runtime.InteropServices;
using System.Text;

namespace Aquiis.Infrastructure.Services;

/// <summary>
/// Service for storing and retrieving encryption keys from Linux Secret Service (libsecret).
/// Provides convenient auto-decryption on trusted devices.
/// </summary>
public class LinuxKeychainService
{
    private const string Schema = "org.aquiis.database";
    private const string KeyAttribute = "key-type";
    private const string KeyValue = "database-encryption";
    
    /// <summary>
    /// Store encryption key in Linux keychain (libsecret)
    /// </summary>
    /// <param name="keyHex">Hex-encoded encryption key</param>
    /// <param name="label">Human-readable label for the key</param>
    /// <returns>True if stored successfully</returns>
    public bool StoreKey(string keyHex, string label = "Aquiis Database Encryption Key")
    {
        if (!OperatingSystem.IsLinux())
            return false;
        
        try
        {
            // Use secret-tool command line utility (part of libsecret)
            // This is more reliable than P/Invoke for cross-distribution compatibility
            var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "secret-tool",
                    Arguments = $"store --label=\"{label}\" {KeyAttribute} {KeyValue}",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            
            process.Start();
            process.StandardInput.WriteLine(keyHex);
            process.StandardInput.Close();
            process.WaitForExit(5000);
            
            return process.ExitCode == 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to store key in keychain: {ex.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Retrieve encryption key from Linux keychain
    /// </summary>
    /// <returns>Hex-encoded encryption key, or null if not found</returns>
    public string? RetrieveKey()
    {
        if (!OperatingSystem.IsLinux())
            return null;
        
        try
        {
            var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "secret-tool",
                    Arguments = $"lookup {KeyAttribute} {KeyValue}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            
            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit(5000);
            
            if (process.ExitCode == 0 && !string.IsNullOrWhiteSpace(output))
            {
                return output.Trim();
            }
            
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to retrieve key from keychain: {ex.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// Remove encryption key from Linux keychain
    /// </summary>
    /// <returns>True if removed successfully</returns>
    public bool RemoveKey()
    {
        if (!OperatingSystem.IsLinux())
            return false;
        
        try
        {
            var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "secret-tool",
                    Arguments = $"clear {KeyAttribute} {KeyValue}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            
            process.Start();
            process.WaitForExit(5000);
            
            return process.ExitCode == 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to remove key from keychain: {ex.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Check if secret-tool is available on the system
    /// </summary>
    public bool IsAvailable()
    {
        if (!OperatingSystem.IsLinux())
            return false;
        
        try
        {
            var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "which",
                    Arguments = "secret-tool",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            
            process.Start();
            process.WaitForExit(2000);
            
            return process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }
}
