using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Aquiis.SimpleStart.Core.Validation;

namespace Aquiis.SimpleStart.Core.Entities
{
    public class NotificationPreferences : BaseModel
    {
        [RequiredGuid]
        public Guid OrganizationId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        // In-App Notification Preferences
        public bool EnableInAppNotifications { get; set; } = true;

        // Email Preferences
        public bool EnableEmailNotifications { get; set; } = true;

        [StringLength(200)]
        public string? EmailAddress { get; set; }

        public bool EmailLeaseExpiring { get; set; } = true;
        public bool EmailPaymentDue { get; set; } = true;
        public bool EmailPaymentReceived { get; set; } = true;
        public bool EmailApplicationStatusChange { get; set; } = true;
        public bool EmailMaintenanceUpdate { get; set; } = true;
        public bool EmailInspectionScheduled { get; set; } = true;

        // SMS Preferences
        public bool EnableSMSNotifications { get; set; } = false;

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        public bool SMSPaymentDue { get; set; } = false;
        public bool SMSMaintenanceEmergency { get; set; } = true;
        public bool SMSLeaseExpiringUrgent { get; set; } = false; // 30 days or less

        // Digest Preferences
        public bool EnableDailyDigest { get; set; } = false;
        public TimeSpan DailyDigestTime { get; set; } = new TimeSpan(9, 0, 0); // 9 AM

        public bool EnableWeeklyDigest { get; set; } = false;
        public DayOfWeek WeeklyDigestDay { get; set; } = DayOfWeek.Monday;

        // Navigation
        [ForeignKey(nameof(OrganizationId))]
        public virtual Organization? Organization { get; set; }
    }
}