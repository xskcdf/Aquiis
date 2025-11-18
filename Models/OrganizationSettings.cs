using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aquiis.SimpleStart.Models
{
    /// <summary>
    /// Organization-specific settings for late fees, payment reminders, and other configurable features.
    /// Each organization can have different policies for their property management operations.
    /// </summary>
    public class OrganizationSettings : BaseModel
    {
        public Guid OrganizationId { get; set; }

        [MaxLength(200)]
        public string? Name { get; set; }

        #region Late Fee Settings

        [Display(Name = "Enable Late Fees")]
        public bool LateFeeEnabled { get; set; } = true;

        [Display(Name = "Auto-Apply Late Fees")]
        public bool LateFeeAutoApply { get; set; } = true;

        [Required]
        [Range(0, 30)]
        [Display(Name = "Grace Period (Days)")]
        public int LateFeeGracePeriodDays { get; set; } = 3;

        [Required]
        [Range(0, 1)]
        [Display(Name = "Late Fee Percentage")]
        public decimal LateFeePercentage { get; set; } = 0.05m;

        [Required]
        [Range(0, 10000)]
        [Display(Name = "Maximum Late Fee Amount")]
        public decimal MaxLateFeeAmount { get; set; } = 50.00m;

        #endregion

        #region Payment Reminder Settings

        [Display(Name = "Enable Payment Reminders")]
        public bool PaymentReminderEnabled { get; set; } = true;

        [Required]
        [Range(1, 30)]
        [Display(Name = "Send Reminder (Days Before Due)")]
        public int PaymentReminderDaysBefore { get; set; } = 3;

        #endregion

        // Future settings can be added here as new regions:
        // - Default lease terms
        // - Routine inspection intervals
        // - Document retention policies
        // - etc.
    }
}
