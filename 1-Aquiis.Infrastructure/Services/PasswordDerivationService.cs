using System.Security.Cryptography;
using System.Text;

namespace Aquiis.Infrastructure.Services;

/// <summary>
/// Service for deriving encryption keys from user passwords using PBKDF2.
/// Provides portable encryption - same password produces same key on any device.
/// </summary>
public class PasswordDerivationService
{
    private const int SaltSize = 32; // 256 bits
    private const int KeySize = 32; // 256 bits for AES-256
    private const int Iterations = 600000; // OWASP recommendation for PBKDF2-SHA256 (2023+)
    
    /// <summary>
    /// Generate a random salt for key derivation
    /// </summary>
    public byte[] GenerateSalt()
    {
        var salt = new byte[SaltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        return salt;
    }
    
    /// <summary>
    /// Derive an encryption key from a password and salt
    /// </summary>
    /// <param name="password">User's master password</param>
    /// <param name="salt">Random salt (should be stored with database)</param>
    /// <returns>256-bit encryption key</returns>
    public byte[] DeriveKey(string password, byte[] salt)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password cannot be null or empty", nameof(password));
        
        if (salt == null || salt.Length != SaltSize)
            throw new ArgumentException($"Salt must be {SaltSize} bytes", nameof(salt));
        
        return Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            KeySize);
    }
    
    /// <summary>
    /// Derive a key and convert to hex string (for SQLCipher connection string)
    /// </summary>
    /// <param name="password">User's master password</param>
    /// <param name="salt">Random salt</param>
    /// <returns>Hex-encoded encryption key</returns>
    public string DeriveKeyAsHex(string password, byte[] salt)
    {
        var key = DeriveKey(password, salt);
        return Convert.ToHexString(key).ToLower();
    }
    
    /// <summary>
    /// Validate password strength
    /// </summary>
    public (bool IsValid, string ErrorMessage) ValidatePasswordStrength(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return (false, "Password cannot be empty");
        
        if (password.Length < 12)
            return (false, "Password must be at least 12 characters long");
        
        bool hasUpper = password.Any(char.IsUpper);
        bool hasLower = password.Any(char.IsLower);
        bool hasDigit = password.Any(char.IsDigit);
        bool hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));
        
        int categories = (hasUpper ? 1 : 0) + (hasLower ? 1 : 0) + 
                        (hasDigit ? 1 : 0) + (hasSpecial ? 1 : 0);
        
        if (categories < 3)
            return (false, "Password must contain at least 3 of: uppercase, lowercase, numbers, special characters");
        
        return (true, string.Empty);
    }
    
    /// <summary>
    /// Convert salt to base64 for storage
    /// </summary>
    public string SaltToString(byte[] salt)
    {
        return Convert.ToBase64String(salt);
    }
    
    /// <summary>
    /// Convert base64 string back to salt bytes
    /// </summary>
    public byte[] StringToSalt(string saltString)
    {
        return Convert.FromBase64String(saltString);
    }
}
