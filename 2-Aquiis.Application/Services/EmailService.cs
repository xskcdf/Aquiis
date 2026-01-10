using System.Net;
using System.Net.Mail;
using Aquiis.Core.Interfaces.Services;
namespace Aquiis.Application.Services{

    public class EmailService : IEmailService
    {
        private readonly EmailSettingsService _emailSettingsService;

        public EmailService(EmailSettingsService emailSettingsService)
        {
            _emailSettingsService = emailSettingsService;
        }

        public async Task<EmailStats> GetEmailStatsAsync()
        {
            var settings = await _emailSettingsService.GetOrCreateSettingsAsync();
            if (settings == null)
            {
                return new EmailStats { IsConfigured = false };
            }

            // Example logic to get email stats
            var stats = new EmailStats
            {
                PlanType = settings.PlanType!,
                Provider = settings.ProviderName,
                LastEmailSentOn = settings.LastEmailSentOn,
                EmailsSentToday = settings.EmailsSentToday,
                DailyLimit = settings.DailyLimit!.Value,
                EmailsSentThisMonth = settings.EmailsSentThisMonth,
                MonthlyLimit = settings.MonthlyLimit!.Value,
                IsConfigured = settings.IsEmailEnabled
            };

            return stats;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var settings = await _emailSettingsService.GetOrCreateSettingsAsync();
            if (settings == null)
            {
                throw new InvalidOperationException("Email settings are not configured.");
            }

            // Implement email sending logic here using the configured settings
            // Example using SMTP client
            switch (settings.ProviderName)
            {
                case "SendGrid":
                    // Implement SendGrid email sending logic here
                    break;
                case "SMTP":
                    using (var client = new SmtpClient(settings.SmtpServer, settings.SmtpPort))
                    {
                        client.Credentials = new NetworkCredential(settings.Username, settings.Password);
                        client.EnableSsl = settings.EnableSsl;

                        var mailMessage = new MailMessage
                        {
                            From = new MailAddress(settings.FromEmail!, settings.FromName),
                            Subject = subject,
                            Body = body,
                            IsBodyHtml = true
                        };
                        mailMessage.To.Add(to);

                        await client.SendMailAsync(mailMessage);
                    }
                    break;
                default:
                    throw new NotSupportedException($"Email provider '{settings.ProviderName}' is not supported.");
            }
            
        }

        public Task SendEmailAsync(string to, string subject, string body, string? fromName = null)
        {
            throw new NotImplementedException();
        }

        public Task SendTemplateEmailAsync(string to, string templateId, Dictionary<string, string> templateData)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidateEmailAddressAsync(string emailAddress)
        {
            throw new NotImplementedException();
        }
    }

    public class EmailStats
    {
        public string PlanType { get; set; } = "string.Empty";
        public string Provider { get; set; } = string.Empty;

        public DateTime? LastEmailSentOn { get; set; }
        public int EmailsSentToday { get; set; }
        public int DailyLimit { get; set; }
        public int EmailsSentThisMonth { get; set; }
        public int MonthlyLimit { get; set; }
        public bool IsConfigured { get; set; }
        public int DailyPercentUsed => DailyLimit == 0 ? 0 : (int)((double)EmailsSentToday / DailyLimit * 100);
        public int MonthlyPercentUsed => MonthlyLimit == 0 ? 0 : (int)((double)EmailsSentThisMonth / MonthlyLimit * 100);
    }
}