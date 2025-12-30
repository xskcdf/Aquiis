using System;
using System.Threading.Tasks;
using Aquiis.SimpleStart.Core.Constants;
using Aquiis.SimpleStart.Core.Entities;
using Aquiis.SimpleStart.Core.Services;
using Aquiis.SimpleStart.Infrastructure.Data;
using Aquiis.SimpleStart.Infrastructure.Services;
using Aquiis.SimpleStart.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aquiis.SimpleStart.Application.Services
{
    public class SMSSettingsService : BaseService<OrganizationSMSSettings>
    {
        private readonly TwilioSMSService _smsService;

        public SMSSettingsService(
            ApplicationDbContext context,
            ILogger<SMSSettingsService> logger,
            UserContextService userContext,
            IOptions<ApplicationSettings> settings,
            TwilioSMSService smsService)
            : base(context, logger, userContext, settings)
        {
            _smsService = smsService;
        }

        public async Task<OrganizationSMSSettings> GetOrCreateSettingsAsync()
        {
            var orgId = await _userContext.GetActiveOrganizationIdAsync();
            if (orgId == null)
            {
                throw new UnauthorizedAccessException("No active organization");
            }

            var settings = await _dbSet
                .FirstOrDefaultAsync(s => s.OrganizationId == orgId && !s.IsDeleted);

            if (settings == null)
            {
                settings = new OrganizationSMSSettings
                {
                    Id = Guid.NewGuid(),
                    OrganizationId = orgId.Value,
                    IsSMSEnabled = false,
                    CostPerSMS = 0.0075m, // Approximate US cost
                    CreatedBy = await _userContext.GetUserIdAsync() ?? string.Empty,
                    CreatedOn = DateTime.UtcNow
                };
                await CreateAsync(settings);
            }

            return settings;
        }

        public async Task<OperationResult> UpdateTwilioConfigAsync(
            string accountSid,
            string authToken,
            string phoneNumber)
        {
            // Verify credentials work before saving
            if (!await _smsService.VerifyTwilioCredentialsAsync(accountSid, authToken, phoneNumber))
            {
                return OperationResult.FailureResult(
                    "Invalid Twilio credentials or phone number. Please verify your Account SID, Auth Token, and phone number.");
            }

            var settings = await GetOrCreateSettingsAsync();

            settings.TwilioAccountSidEncrypted = _smsService.EncryptAccountSid(accountSid);
            settings.TwilioAuthTokenEncrypted = _smsService.EncryptAuthToken(authToken);
            settings.TwilioPhoneNumber = phoneNumber;
            settings.IsSMSEnabled = true;
            settings.IsVerified = true;
            settings.LastVerifiedOn = DateTime.UtcNow;
            settings.LastError = null;

            await UpdateAsync(settings);

            return OperationResult.SuccessResult("Twilio configuration saved successfully");
        }

        public async Task<OperationResult> DisableSMSAsync()
        {
            var settings = await GetOrCreateSettingsAsync();
            settings.IsSMSEnabled = false;
            await UpdateAsync(settings);

            return OperationResult.SuccessResult("SMS notifications disabled");
        }

        public async Task<OperationResult> EnableSMSAsync()
        {
            var settings = await GetOrCreateSettingsAsync();

            if (string.IsNullOrEmpty(settings.TwilioAccountSidEncrypted))
            {
                return OperationResult.FailureResult(
                    "Twilio credentials not configured. Please configure Twilio first.");
            }

            settings.IsSMSEnabled = true;
            await UpdateAsync(settings);

            return OperationResult.SuccessResult("SMS notifications enabled");
        }

        public async Task<OperationResult> TestSMSConfigurationAsync(string testPhoneNumber)
        {
            try
            {
                await _smsService.SendSMSAsync(
                    testPhoneNumber,
                    "Aquiis SMS Configuration Test: This message confirms your Twilio integration is working correctly.");

                return OperationResult.SuccessResult("Test SMS sent successfully! Check your phone.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Test SMS failed");
                return OperationResult.FailureResult($"Failed to send test SMS: {ex.Message}");
            }
        }
    }
}