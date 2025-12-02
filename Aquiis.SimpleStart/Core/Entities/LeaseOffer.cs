using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Aquiis.SimpleStart.Core.Entities;
using Aquiis.SimpleStart.Core.Entities;

namespace Aquiis.SimpleStart.Core.Entities
{
    public class LeaseOffer : BaseModel
    {
        [Required]
        public Guid OrganizationId { get; set; }

        [Required]
        public int RentalApplicationId { get; set; }

        [Required]
        public int PropertyId { get; set; }

        [Required]
        public int ProspectiveTenantId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MonthlyRent { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SecurityDeposit { get; set; }

        [Required]
        [StringLength(2000)]
        public string Terms { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Notes { get; set; } = string.Empty;

        [Required]
        public DateTime OfferedOn { get; set; }

        [Required]
        public DateTime ExpiresOn { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Accepted, Declined, Expired, Withdrawn

        public DateTime? RespondedOn { get; set; }

        [StringLength(500)]
        public string? ResponseNotes { get; set; }

        public int? ConvertedLeaseId { get; set; } // Set when offer is accepted and converted to lease

        // Navigation properties
        [ForeignKey("RentalApplicationId")]
        public virtual RentalApplication RentalApplication { get; set; } = null!;

        [ForeignKey("PropertyId")]
        public virtual Property Property { get; set; } = null!;

        [ForeignKey("ProspectiveTenantId")]
        public virtual ProspectiveTenant ProspectiveTenant { get; set; } = null!;
    }
}
