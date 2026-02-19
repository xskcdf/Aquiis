using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Aquiis.Core.Validation;

namespace Aquiis.Core.Entities
{
    /// <summary>
    /// Stores SendGrid email configuration per organization.
    /// Each organization manages their own SendGrid account.
    /// </summary>
    public class OrganizationEmailSettings : BaseModel
    {
        public string ProviderName { get; set; } = "SMTP";

        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; } = 587;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool EnableSsl { get; set; } = true;

        // SendGrid Configuration
        public bool IsEmailEnabled { get; set; }

        /// <summary>
        /// Encrypted SendGrid API key using Data Protection API
        /// </summary>
        [StringLength(1000)]
        public string? SendGridApiKeyEncrypted { get; set; }

        [StringLength(200)]
        [EmailAddress]
        public string? FromEmail { get; set; }

        [StringLength(200)]
        public string? FromName { get; set; }

        // Email Usage Tracking (local cache)
        public int EmailsSentToday { get; set; }
        public int EmailsSentThisMonth { get; set; }
        public DateTime? LastEmailSentOn { get; set; }
        public DateTime? StatsLastUpdatedOn { get; set; }
        public DateTime? DailyCountResetOn { get; set; }
        public DateTime? MonthlyCountResetOn { get; set; }

        // SendGrid Account Info (cached from API)
        public int? DailyLimit { get; set; }
        public int? MonthlyLimit { get; set; }

        [StringLength(100)]
        public string? PlanType { get; set; } // Free, Essentials, Pro, etc.

        // Verification Status
        public bool IsVerified { get; set; }
        public DateTime? LastVerifiedOn { get; set; }

        /// <summary>
        /// Last error encountered when sending email or verifying API key
        /// </summary>
        [StringLength(1000)]
        public string? LastError { get; set; }

        public DateTime? LastErrorOn { get; set; }

        // Navigation
        [ForeignKey(nameof(OrganizationId))]
        public virtual Organization? Organization { get; set; }
    }
}