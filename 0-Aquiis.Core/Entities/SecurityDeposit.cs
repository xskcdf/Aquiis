using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Aquiis.Core.Entities
{
    /// <summary>
    /// Security deposit tracking for each lease with complete lifecycle management.
    /// Tracks deposit collection, investment pool participation, and refund disposition.
    /// </summary>
    public class SecurityDeposit : BaseModel
    {
        [Required]
        [JsonInclude]
        public Guid LeaseId { get; set; }

        [Required]
        [JsonInclude]
        public Guid TenantId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Deposit amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime DateReceived { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = string.Empty; // Check, Cash, Bank Transfer, etc.

        [StringLength(100)]
        public string? TransactionReference { get; set; } // Check number, transfer ID, etc.

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Held"; // Held, Released, Refunded, Forfeited, PartiallyRefunded

        /// <summary>
        /// Tracks whether this deposit is included in the investment pool for dividend calculation.
        /// Set to true when lease becomes active and deposit is added to pool.
        /// </summary>
        public bool InInvestmentPool { get; set; } = false;

        /// <summary>
        /// Date when deposit was added to investment pool (typically lease start date).
        /// Used for pro-rating dividend calculations for mid-year move-ins.
        /// </summary>
        public DateTime? PoolEntryDate { get; set; }

        /// <summary>
        /// Date when deposit was removed from investment pool (typically lease end date).
        /// Used to stop dividend accrual.
        /// </summary>
        public DateTime? PoolExitDate { get; set; }

        // Refund Tracking
        public DateTime? RefundProcessedDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? RefundAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? DeductionsAmount { get; set; }

        [StringLength(1000)]
        public string? DeductionsReason { get; set; }

        [StringLength(50)]
        public string? RefundMethod { get; set; } // Check, Bank Transfer, Applied to Balance

        [StringLength(100)]
        public string? RefundReference { get; set; } // Check number, transfer ID

        [StringLength(500)]
        public string? Notes { get; set; }

        // Navigation properties
        [ForeignKey("LeaseId")]
        public virtual Lease Lease { get; set; } = null!;

        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; } = null!;

        public virtual ICollection<SecurityDepositDividend> Dividends { get; set; } = new List<SecurityDepositDividend>();

        // Computed properties
        public bool IsRefunded => Status == "Refunded" || Status == "PartiallyRefunded";
        public bool IsActive => Status == "Held" && InInvestmentPool;
        public decimal TotalDividendsEarned => Dividends.Sum(d => d.DividendAmount);
        public decimal NetRefundDue => Amount + TotalDividendsEarned - (DeductionsAmount ?? 0);
    }
}
