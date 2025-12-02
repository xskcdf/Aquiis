using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Aquiis.SimpleStart.Core.Entities;

namespace Aquiis.SimpleStart.Core.Entities
{
    /// <summary>
    /// Annual investment pool performance tracking.
    /// All security deposits are pooled and invested, with annual earnings distributed as dividends.
    /// </summary>
    public class SecurityDepositInvestmentPool : BaseModel
    {
        [Required]
        public string OrganizationId { get; set; } = string.Empty;

        [Required]
        public int Year { get; set; }

        /// <summary>
        /// Total security deposit amount in pool at start of year.
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal StartingBalance { get; set; }

        /// <summary>
        /// Total security deposit amount in pool at end of year.
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal EndingBalance { get; set; }

        /// <summary>
        /// Total investment earnings for the year (can be negative for losses).
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalEarnings { get; set; }

        /// <summary>
        /// Rate of return for the year (as decimal, e.g., 0.05 = 5%).
        /// Calculated as TotalEarnings / StartingBalance.
        /// </summary>
        [Column(TypeName = "decimal(18,6)")]
        public decimal ReturnRate { get; set; }

        /// <summary>
        /// Organization's share percentage (default 20%).
        /// Configurable per organization via OrganizationSettings.
        /// </summary>
        [Required]
        [Range(0, 1)]
        [Column(TypeName = "decimal(18,6)")]
        public decimal OrganizationSharePercentage { get; set; } = 0.20m;

        /// <summary>
        /// Amount retained by organization (TotalEarnings * OrganizationSharePercentage).
        /// Only applies if TotalEarnings > 0 (losses absorbed by organization).
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal OrganizationShare { get; set; }

        /// <summary>
        /// Amount available for distribution to tenants (TotalEarnings - OrganizationShare).
        /// Zero if TotalEarnings <= 0 (no negative dividends).
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal TenantShareTotal { get; set; }

        /// <summary>
        /// Number of active leases in the pool for the year.
        /// Used to calculate per-lease dividend (TenantShareTotal / ActiveLeaseCount).
        /// </summary>
        [Required]
        public int ActiveLeaseCount { get; set; }

        /// <summary>
        /// Dividend amount per active lease (TenantShareTotal / ActiveLeaseCount).
        /// Pro-rated for mid-year move-ins.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal DividendPerLease { get; set; }

        /// <summary>
        /// Date when dividends were calculated.
        /// </summary>
        public DateTime? DividendsCalculatedOn { get; set; }

        /// <summary>
        /// Date when dividends were distributed to tenants.
        /// </summary>
        public DateTime? DividendsDistributedOn { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Open"; // Open, Calculated, Distributed, Closed

        [StringLength(1000)]
        public string? Notes { get; set; }

        // Navigation properties
        public virtual ICollection<SecurityDepositDividend> Dividends { get; set; } = new List<SecurityDepositDividend>();

        // Computed properties
        public bool HasEarnings => TotalEarnings > 0;
        public bool HasLosses => TotalEarnings < 0;
        public decimal AbsorbedLosses => TotalEarnings < 0 ? Math.Abs(TotalEarnings) : 0;
    }
}
