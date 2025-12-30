
using Aquiis.SimpleStart.Core.Interfaces.Services;

namespace Aquiis.SimpleStart.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IConfiguration _configuration;

    public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        // TODO: Implement with SendGrid/Mailgun in Task 2.5
        _logger.LogInformation($"[EMAIL] To: {to}, Subject: {subject}, Body: {body}");
        await Task.CompletedTask;
    }

    public async Task SendEmailAsync(string to, string subject, string body, string? fromName = null)
    {
        _logger.LogInformation($"[EMAIL] From: {fromName}, To: {to}, Subject: {subject}");
        await Task.CompletedTask;
    }

    public async Task SendTemplateEmailAsync(string to, string templateId, Dictionary<string, string> templateData)
    {
        _logger.LogInformation($"[EMAIL TEMPLATE] To: {to}, Template: {templateId}");
        await Task.CompletedTask;
    }

    public async Task<bool> ValidateEmailAddressAsync(string email)
    {
        // Basic validation
        return await Task.FromResult(!string.IsNullOrWhiteSpace(email) && email.Contains("@"));
    }
}