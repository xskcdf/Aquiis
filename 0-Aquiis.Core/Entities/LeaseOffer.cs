using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aquiis.Core.Entities
{
    public class LeaseOffer : BaseModel
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Organization ID")]
        public Guid OrganizationId { get; set; } = Guid.Empty;

        [Required]
        public Guid RentalApplicationId { get; set; }

        [Required]
        public Guid PropertyId { get; set; }

        [Required]
        public Guid ProspectiveTenantId { get; set; }

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

        public Guid? ConvertedLeaseId { get; set; } // Set when offer is accepted and converted to lease

        // Navigation properties
        [ForeignKey("RentalApplicationId")]
        public virtual RentalApplication RentalApplication { get; set; } = null!;

        [ForeignKey("PropertyId")]
        public virtual Property Property { get; set; } = null!;

        [ForeignKey("ProspectiveTenantId")]
        public virtual ProspectiveTenant ProspectiveTenant { get; set; } = null!;
    }
}
