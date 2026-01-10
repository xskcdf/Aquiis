
using Aquiis.Core.Interfaces.Services;

namespace Aquiis.Application.Services
{
    public class SMSService : ISMSService
    {
        private readonly SMSSettingsService _smsSettingsService;
        public SMSService(SMSSettingsService smsSettingsService)
        {
            _smsSettingsService = smsSettingsService;
        }

        public async Task SendSMSAsync(string phoneNumber, string message)
        {
            var settings = await _smsSettingsService.GetOrCreateSettingsAsync();
            if (settings == null)
            {
                throw new InvalidOperationException("SMS settings are not configured.");
            }

            // Implement SMS sending logic here using the configured settings
        }

        public Task<bool> ValidatePhoneNumberAsync(string phoneNumber)
        {
            throw new NotImplementedException();
        }

        public async Task<SMSStats> GetSMSStatsAsync()
        {
            var stats = await _smsSettingsService.GetOrCreateSettingsAsync();
            return new SMSStats
            {
                ProviderName = stats.ProviderName!,
                SMSSentToday = stats.SMSSentToday,
                SMSSentThisMonth = stats.SMSSentThisMonth,
                LastSMSSentOn = stats.LastSMSSentOn,
                StatsLastUpdatedOn = stats.StatsLastUpdatedOn,
                DailyCountResetOn = stats.DailyCountResetOn,
                MonthlyCountResetOn = stats.MonthlyCountResetOn,
                AccountBalance = stats.AccountBalance,
                CostPerSMS = stats.CostPerSMS
            };
        }
        public Task DisableSMSAsync()
        {
            throw new NotImplementedException();
        }

        public Task EnableSMSAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsSMSEnabledAsync()
        {
            throw new NotImplementedException();
        }
    }

    public class SMSStats
    {
        public string ProviderName { get; set; } = string.Empty;
        public int TotalMessages { get; set; }
        public int SentMessages { get; set; }
        public int FailedMessages { get; set; }
        public int PendingMessages { get; set; }

        // SMS Usage Tracking (local cache)
        public int SMSSentToday { get; set; }
        public int SMSSentThisMonth { get; set; }
        public DateTime? LastSMSSentOn { get; set; }
        public DateTime? StatsLastUpdatedOn { get; set; }
        public DateTime? DailyCountResetOn { get; set; }
        public DateTime? MonthlyCountResetOn { get; set; }

        // Twilio Account Info (cached from API)
        public decimal? AccountBalance { get; set; }
        public decimal? CostPerSMS { get; set; } // Approximate cost
    }
}