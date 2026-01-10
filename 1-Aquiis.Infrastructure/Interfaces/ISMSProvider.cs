namespace Aquiis.Infrastructure.Interfaces;

/// <summary>
/// Interface for SMS provider implementations (Twilio, etc.)
/// Used by SMSSettingsService to manage provider-specific configuration.
/// </summary>
public interface ISMSProvider
{
    /// <summary>
    /// Verify that credentials are valid and have proper permissions
    /// </summary>
    Task<bool> VerifyCredentialsAsync(string accountSid, string authToken);

    /// <summary>
    /// Encrypt credentials for secure storage
    /// </summary>
    string EncryptCredential(string credential);

    /// <summary>
    /// Decrypt credentials for use
    /// </summary>
    string DecryptCredential(string encryptedCredential);

    /// <summary>
    /// Send an SMS using this provider
    /// </summary>
    Task SendSMSAsync(string to, string message);

    /// <summary>
    /// Validate a phone number format
    /// </summary>
    Task<bool> ValidatePhoneNumberAsync(string phoneNumber);
}
