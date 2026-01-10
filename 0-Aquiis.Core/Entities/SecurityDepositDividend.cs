using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Aquiis.Core.Entities
{
    /// <summary>
    /// Individual dividend payment tracking for each lease's security deposit.
    /// Dividends are calculated annually and distributed based on tenant's choice.
    /// </summary>
    public class SecurityDepositDividend : BaseModel
    {
        [Required]
        [JsonInclude]
        [StringLength(100)]
        [Display(Name = "Organization ID")]
        public Guid OrganizationId { get; set; } = Guid.Empty;

        [Required]
        public Guid SecurityDepositId { get; set; }

        [Required]
        public Guid InvestmentPoolId { get; set; }

        [Required]
        public Guid LeaseId { get; set; }

        [Required]
        public Guid TenantId { get; set; }

        [Required]
        public int Year { get; set; }

        /// <summary>
        /// Base dividend amount (TenantShareTotal / ActiveLeaseCount from pool).
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BaseDividendAmount { get; set; }

        /// <summary>
        /// Pro-ration factor for mid-year move-ins (0.0 to 1.0).
        /// Example: Moved in July 1 = 0.5 (6 months of 12).
        /// </summary>
        [Required]
        [Range(0, 1)]
        [Column(TypeName = "decimal(18,6)")]
        public decimal ProrationFactor { get; set; } = 1.0m;

        /// <summary>
        /// Actual dividend amount after pro-ration (BaseDividendAmount * ProrationFactor).
        /// This is the amount paid to the tenant.
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DividendAmount { get; set; }

        /// <summary>
        /// Tenant's choice for dividend payment.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = "Pending"; // Pending, LeaseCredit, Check

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, ChoiceMade, Applied, Paid

        /// <summary>
        /// Date when tenant made their payment method choice.
        /// </summary>
        public DateTime? ChoiceMadeOn { get; set; }

        /// <summary>
        /// Date when dividend was applied as lease credit or check was issued.
        /// </summary>
        public DateTime? PaymentProcessedOn { get; set; }

        [StringLength(100)]
        public string? PaymentReference { get; set; } // Check number, invoice ID

        /// <summary>
        /// Mailing address if tenant chose check and has moved out.
        /// </summary>
        [StringLength(500)]
        public string? MailingAddress { get; set; }

        /// <summary>
        /// Number of months deposit was in pool during the year (for pro-ration calculation).
        /// </summary>
        public int MonthsInPool { get; set; } = 12;

        [StringLength(500)]
        public string? Notes { get; set; }

        // Navigation properties
        [ForeignKey("SecurityDepositId")]
        public virtual SecurityDeposit SecurityDeposit { get; set; } = null!;

        [ForeignKey("InvestmentPoolId")]
        public virtual SecurityDepositInvestmentPool InvestmentPool { get; set; } = null!;

        [ForeignKey("LeaseId")]
        public virtual Lease Lease { get; set; } = null!;

        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; } = null!;

        // Computed properties
        public bool IsPending => Status == "Pending";
        public bool IsProcessed => Status == "Applied" || Status == "Paid";
        public bool TenantHasChosen => !string.IsNullOrEmpty(PaymentMethod) && PaymentMethod != "Pending";
    }
}
