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
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Aquiis.SimpleStart.Infrastructure.Services
{
    public class TwilioSMSService : ISMSService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserContextService _userContext;
        private readonly ILogger<TwilioSMSService> _logger;
        private readonly IDataProtectionProvider _dataProtection;

        private const string ACCOUNT_SID_PURPOSE = "TwilioAccountSid";
        private const string AUTH_TOKEN_PURPOSE = "TwilioAuthToken";

        public TwilioSMSService(
            ApplicationDbContext context,
            UserContextService userContext,
            ILogger<TwilioSMSService> logger,
            IDataProtectionProvider dataProtection)
        {
            _context = context;
            _userContext = userContext;
            _logger = logger;
            _dataProtection = dataProtection;
        }

        public async Task SendSMSAsync(string phoneNumber, string message)
        {
            var orgId = await _userContext.GetActiveOrganizationIdAsync();
            if (orgId == null)
            {
                _logger.LogWarning("Cannot send SMS - no active organization");
                return;
            }

            var settings = await GetSMSSettingsAsync(orgId.Value);

            if (!settings.IsSMSEnabled ||
                string.IsNullOrEmpty(settings.TwilioAccountSidEncrypted) ||
                string.IsNullOrEmpty(settings.TwilioAuthTokenEncrypted))
            {
                _logger.LogInformation("SMS disabled for organization {OrgId}", orgId);
                return; // Graceful degradation
            }

            try
            {
                var accountSid = DecryptAccountSid(settings.TwilioAccountSidEncrypted);
                var authToken = DecryptAuthToken(settings.TwilioAuthTokenEncrypted);

                TwilioClient.Init(accountSid, authToken);

                var messageResource = await MessageResource.CreateAsync(
                    body: message,
                    from: new PhoneNumber(settings.TwilioPhoneNumber),
                    to: new PhoneNumber(phoneNumber));

                if (messageResource.Status == MessageResource.StatusEnum.Queued ||
                    messageResource.Status == MessageResource.StatusEnum.Sent)
                {
                    _logger.LogInformation("SMS sent successfully to {PhoneNumber}", phoneNumber);
                    await UpdateUsageStatsAsync(settings);
                }
                else
                {
                    _logger.LogError("Twilio SMS status: {Status}", messageResource.Status);
                    settings.LastError = $"Status: {messageResource.Status}";
                    await _context.SaveChangesAsync();
                    throw new Exception($"SMS send failed with status: {messageResource.Status}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send SMS via Twilio for org {OrgId}", orgId);
                settings.LastError = ex.Message;
                await _context.SaveChangesAsync();
                throw;
            }
        }

        public async Task<bool> ValidatePhoneNumberAsync(string phoneNumber)
        {
            // Basic validation
            var digits = new string(phoneNumber.Where(char.IsDigit).ToArray());
            return await Task.FromResult(digits.Length >= 10);
        }

        public async Task<bool> VerifyTwilioCredentialsAsync(string accountSid, string authToken, string phoneNumber)
        {
            try
            {
                TwilioClient.Init(accountSid, authToken);

                // Verify by fetching the incoming phone number
                var incomingPhoneNumber = await IncomingPhoneNumberResource.ReadAsync(
                    phoneNumber: new PhoneNumber(phoneNumber));

                return incomingPhoneNumber.Any();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Twilio credentials verification failed");
                return false;
            }
        }

        public async Task<TwilioStats> GetTwilioStatsAsync()
        {
            var orgId = await _userContext.GetActiveOrganizationIdAsync();
            var settings = await GetSMSSettingsAsync(orgId!.Value);

            if (!settings.IsSMSEnabled)
            {
                return new TwilioStats { IsConfigured = false };
            }

            return new TwilioStats
            {
                IsConfigured = true,
                SMSSentToday = settings.SMSSentToday,
                SMSSentThisMonth = settings.SMSSentThisMonth,
                AccountBalance = settings.AccountBalance ?? 0,
                CostPerSMS = settings.CostPerSMS ?? 0.0075m,
                EstimatedMonthlyCost = settings.SMSSentThisMonth * (settings.CostPerSMS ?? 0.0075m),
                LastSMSSentOn = settings.LastSMSSentOn,
                LastVerifiedOn = settings.LastVerifiedOn,
                AccountType = settings.AccountType ?? "Unknown"
            };
        }

        private async Task<OrganizationSMSSettings> GetSMSSettingsAsync(Guid organizationId)
        {
            var settings = await _context.OrganizationSMSSettings
                .FirstOrDefaultAsync(s => s.OrganizationId == organizationId && !s.IsDeleted);

            if (settings == null)
            {
                throw new InvalidOperationException(
                    $"SMS settings not found for organization {organizationId}");
            }

            return settings;
        }

        private async Task UpdateUsageStatsAsync(OrganizationSMSSettings settings)
        {
            var now = DateTime.UtcNow;
            var today = now.Date;

            // Reset daily counter if needed
            if (settings.DailyCountResetOn?.Date != today)
            {
                settings.SMSSentToday = 0;
                settings.DailyCountResetOn = today;
            }

            // Reset monthly counter if needed
            if (settings.MonthlyCountResetOn?.Month != now.Month ||
                settings.MonthlyCountResetOn?.Year != now.Year)
            {
                settings.SMSSentThisMonth = 0;
                settings.MonthlyCountResetOn = new DateTime(now.Year, now.Month, 1);
            }

            settings.SMSSentToday++;
            settings.SMSSentThisMonth++;
            settings.LastSMSSentOn = now;
            settings.StatsLastUpdatedOn = now;

            await _context.SaveChangesAsync();
        }

        private string DecryptAccountSid(string encrypted)
        {
            var protector = _dataProtection.CreateProtector(ACCOUNT_SID_PURPOSE);
            return protector.Unprotect(encrypted);
        }

        private string DecryptAuthToken(string encrypted)
        {
            var protector = _dataProtection.CreateProtector(AUTH_TOKEN_PURPOSE);
            return protector.Unprotect(encrypted);
        }

        public string EncryptAccountSid(string accountSid)
        {
            var protector = _dataProtection.CreateProtector(ACCOUNT_SID_PURPOSE);
            return protector.Protect(accountSid);
        }

        public string EncryptAuthToken(string authToken)
        {
            var protector = _dataProtection.CreateProtector(AUTH_TOKEN_PURPOSE);
            return protector.Protect(authToken);
        }
    }

    public class TwilioStats
    {
        public bool IsConfigured { get; set; }
        public int SMSSentToday { get; set; }
        public int SMSSentThisMonth { get; set; }
        public decimal AccountBalance { get; set; }
        public decimal CostPerSMS { get; set; }
        public decimal EstimatedMonthlyCost { get; set; }
        public DateTime? LastSMSSentOn { get; set; }
        public DateTime? LastVerifiedOn { get; set; }
        public string AccountType { get; set; } = string.Empty;
    }
}