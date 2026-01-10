
namespace Aquiis.Core.Interfaces.Services;
public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendEmailAsync(string to, string subject, string body, string? fromName = null);
    Task SendTemplateEmailAsync(string to, string templateId, Dictionary<string, string> templateData);
    Task<bool> ValidateEmailAddressAsync(string email);
}