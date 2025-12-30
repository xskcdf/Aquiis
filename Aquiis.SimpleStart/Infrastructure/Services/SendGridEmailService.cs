using System;
using System.Linq;
using System.Threading.Tasks;
using Aquiis.SimpleStart.Core.Entities;
using Aquiis.SimpleStart.Core.Interfaces.Services;
using Aquiis.SimpleStart.Infrastructure.Data;
using Aquiis.SimpleStart.Shared.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Aquiis.SimpleStart.Infrastructure.Services
{
    public class SendGridEmailService : IEmailService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserContextService _userContext;
        private readonly ILogger<SendGridEmailService> _logger;
        private readonly IDataProtectionProvider _dataProtection;

        private const string PROTECTION_PURPOSE = "SendGridApiKey";

        public SendGridEmailService(
            ApplicationDbContext context,
            UserContextService userContext,
            ILogger<SendGridEmailService> logger,
            IDataProtectionProvider dataProtection)
        {
            _context = context;
            _userContext = userContext;
            _logger = logger;
            _dataProtection = dataProtection;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var orgId = await _userContext.GetActiveOrganizationIdAsync();
            if (orgId == null)
            {
                _logger.LogWarning("Cannot send email - no active organization");
                return;
            }

            var settings = await GetEmailSettingsAsync(orgId.Value);

            if (!settings.IsEmailEnabled || string.IsNullOrEmpty(settings.SendGridApiKeyEncrypted))
            {
                _logger.LogInformation("Email disabled for organization {OrgId}", orgId);
                return; // Graceful degradation - don't throw
            }

            try
            {
                var apiKey = DecryptApiKey(settings.SendGridApiKeyEncrypted);
                var client = new SendGridClient(apiKey);

                var from = new EmailAddress(settings.FromEmail, settings.FromName);
                var toAddress = new EmailAddress(to);
                var msg = MailHelper.CreateSingleEmail(from, toAddress, subject, body, body);

                var response = await client.SendEmailAsync(msg);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Email sent successfully to {To}", to);
                    await UpdateUsageStatsAsync(settings);
                }
                else
                {
                    var error = await response.Body.ReadAsStringAsync();
                    _logger.LogError("SendGrid error {StatusCode}: {Error}", response.StatusCode, error);
                    settings.LastError = $"HTTP {response.StatusCode}: {error}";
                    await _context.SaveChangesAsync();
                    throw new Exception($"SendGrid returned {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email via SendGrid for org {OrgId}", orgId);
                settings.LastError = ex.Message;
                settings.LastErrorOn = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                throw;
            }
        }

        public async Task SendEmailAsync(string to, string subject, string body, string? fromName = null)
        {
            // Override from name if provided
            var orgId = await _userContext.GetActiveOrganizationIdAsync();
            var settings = await GetEmailSettingsAsync(orgId!.Value);

            var originalFromName = settings.FromName;
            if (!string.IsNullOrEmpty(fromName))
            {
                settings.FromName = fromName;
            }

            await SendEmailAsync(to, subject, body);

            settings.FromName = originalFromName;
        }

        public async Task SendTemplateEmailAsync(string to, string templateId, Dictionary<string, string> templateData)
        {
            var orgId = await _userContext.GetActiveOrganizationIdAsync();
            var settings = await GetEmailSettingsAsync(orgId!.Value);

            if (!settings.IsEmailEnabled)
            {
                _logger.LogInformation("Email disabled for organization {OrgId}", orgId);
                return;
            }

            try
            {
                var apiKey = DecryptApiKey(settings.SendGridApiKeyEncrypted!);
                var client = new SendGridClient(apiKey);

                var msg = new SendGridMessage();
                msg.SetFrom(new EmailAddress(settings.FromEmail, settings.FromName));
                msg.AddTo(new EmailAddress(to));
                msg.SetTemplateId(templateId);
                msg.SetTemplateData(templateData);

                var response = await client.SendEmailAsync(msg);

                if (response.IsSuccessStatusCode)
                {
                    await UpdateUsageStatsAsync(settings);
                }
                else
                {
                    var error = await response.Body.ReadAsStringAsync();
                    _logger.LogError("SendGrid template error: {Error}", error);
                    throw new Exception(error);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send template email via SendGrid");
                throw;
            }
        }

        public async Task<bool> ValidateEmailAddressAsync(string email)
        {
            await Task.CompletedTask;
            return !string.IsNullOrWhiteSpace(email) &&
                   email.Contains("@") &&
                   email.Contains(".");
        }

        public async Task<bool> VerifyApiKeyAsync(string apiKey)
        {
            try
            {
                var client = new SendGridClient(apiKey);

                // Test API key by fetching user profile
                var response = await client.RequestAsync(
                    method: SendGridClient.Method.GET,
                    urlPath: "user/profile");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "SendGrid API key verification failed");
                return false;
            }
        }

        public async Task<SendGridStats> GetSendGridStatsAsync()
        {
            var orgId = await _userContext.GetActiveOrganizationIdAsync();
            var settings = await GetEmailSettingsAsync(orgId!.Value);

            if (!settings.IsEmailEnabled)
            {
                return new SendGridStats { IsConfigured = false };
            }

            // Optionally refresh stats from SendGrid API
            // await RefreshStatsFromSendGridAsync(settings);

            return new SendGridStats
            {
                IsConfigured = true,
                EmailsSentToday = settings.EmailsSentToday,
                EmailsSentThisMonth = settings.EmailsSentThisMonth,
                DailyLimit = settings.DailyLimit ?? 100,
                MonthlyLimit = settings.MonthlyLimit ?? 40000,
                LastEmailSentOn = settings.LastEmailSentOn,
                LastVerifiedOn = settings.LastVerifiedOn,
                PlanType = settings.PlanType ?? "Free",
                DailyPercentUsed = settings.DailyLimit.HasValue
                    ? (int)((settings.EmailsSentToday / (double)settings.DailyLimit.Value) * 100)
                    : 0,
                MonthlyPercentUsed = settings.MonthlyLimit.HasValue
                    ? (int)((settings.EmailsSentThisMonth / (double)settings.MonthlyLimit.Value) * 100)
                    : 0
            };
        }

        private async Task<OrganizationEmailSettings> GetEmailSettingsAsync(Guid organizationId)
        {
            var settings = await _context.OrganizationEmailSettings
                .FirstOrDefaultAsync(s => s.OrganizationId == organizationId && !s.IsDeleted);

            if (settings == null)
            {
                throw new InvalidOperationException(
                    $"Email settings not found for organization {organizationId}");
            }

            return settings;
        }

        private async Task UpdateUsageStatsAsync(OrganizationEmailSettings settings)
        {
            var now = DateTime.UtcNow;
            var today = now.Date;

            // Reset daily counter if needed
            if (settings.DailyCountResetOn?.Date != today)
            {
                settings.EmailsSentToday = 0;
                settings.DailyCountResetOn = today;
            }

            // Reset monthly counter if needed (first of month)
            if (settings.MonthlyCountResetOn?.Month != now.Month ||
                settings.MonthlyCountResetOn?.Year != now.Year)
            {
                settings.EmailsSentThisMonth = 0;
                settings.MonthlyCountResetOn = new DateTime(now.Year, now.Month, 1);
            }

            settings.EmailsSentToday++;
            settings.EmailsSentThisMonth++;
            settings.LastEmailSentOn = now;
            settings.StatsLastUpdatedOn = now;

            await _context.SaveChangesAsync();
        }

        private string DecryptApiKey(string encrypted)
        {
            var protector = _dataProtection.CreateProtector(PROTECTION_PURPOSE);
            return protector.Unprotect(encrypted);
        }

        public string EncryptApiKey(string apiKey)
        {
            var protector = _dataProtection.CreateProtector(PROTECTION_PURPOSE);
            return protector.Protect(apiKey);
        }
    }

    public class SendGridStats
    {
        public bool IsConfigured { get; set; }
        public int EmailsSentToday { get; set; }
        public int EmailsSentThisMonth { get; set; }
        public int DailyLimit { get; set; }
        public int MonthlyLimit { get; set; }
        public int DailyPercentUsed { get; set; }
        public int MonthlyPercentUsed { get; set; }
        public DateTime? LastEmailSentOn { get; set; }
        public DateTime? LastVerifiedOn { get; set; }
        public string? PlanType { get; set; }
    }
}