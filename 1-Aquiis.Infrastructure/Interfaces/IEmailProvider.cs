namespace Aquiis.Infrastructure.Interfaces;

/// <summary>
/// Interface for email provider implementations (SendGrid, SMTP, etc.)
/// Used by EmailSettingsService to manage provider-specific configuration.
/// </summary>
public interface IEmailProvider
{
    /// <summary>
    /// Verify that an API key is valid and has proper permissions
    /// </summary>
    Task<bool> VerifyApiKeyAsync(string apiKey);

    /// <summary>
    /// Encrypt an API key for secure storage
    /// </summary>
    string EncryptApiKey(string apiKey);

    /// <summary>
    /// Decrypt an API key for use
    /// </summary>
    string DecryptApiKey(string encryptedApiKey);

    /// <summary>
    /// Send an email using this provider
    /// </summary>
    Task SendEmailAsync(string to, string subject, string body, string? fromName = null);

    /// <summary>
    /// Send a templated email using this provider
    /// </summary>
    Task SendTemplateEmailAsync(string to, string templateId, Dictionary<string, string> templateData);

    /// <summary>
    /// Validate an email address format
    /// </summary>
    Task<bool> ValidateEmailAddressAsync(string email);
}
