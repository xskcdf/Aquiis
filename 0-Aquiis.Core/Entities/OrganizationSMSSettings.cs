using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Aquiis.Core.Validation;

namespace Aquiis.Core.Entities
{
    /// <summary>
    /// Stores Twilio SMS configuration per organization.
    /// Each organization manages their own Twilio account.
    /// </summary>
    public class OrganizationSMSSettings : BaseModel
    {
        [RequiredGuid]
        public Guid OrganizationId { get; set; }

        // Twilio Configuration
        public bool IsSMSEnabled { get; set; }

        [StringLength(100)]
        public string? ProviderName { get; set; }

        /// <summary>
        /// Encrypted Twilio Account SID using Data Protection API
        /// </summary>
        [StringLength(1000)]
        public string? TwilioAccountSidEncrypted { get; set; }

        /// <summary>
        /// Encrypted Twilio Auth Token using Data Protection API
        /// </summary>
        [StringLength(1000)]
        public string? TwilioAuthTokenEncrypted { get; set; }

        [StringLength(20)]
        [Phone]
        public string? TwilioPhoneNumber { get; set; }

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

        [StringLength(100)]
        public string? AccountType { get; set; } // Trial, Paid

        // Verification Status
        public bool IsVerified { get; set; }
        public DateTime? LastVerifiedOn { get; set; }

        /// <summary>
        /// Last error encountered when sending SMS or verifying credentials
        /// </summary>
        [StringLength(1000)]
        public string? LastError { get; set; }

        // Navigation
        [ForeignKey(nameof(OrganizationId))]
        public virtual Organization? Organization { get; set; }
    }
}