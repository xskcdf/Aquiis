using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aquiis.SimpleStart.Core.Entities
{
    public class ProspectiveTenant : BaseModel
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Organization ID")]
        public string OrganizationId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Phone]
        [Display(Name = "Phone")]
        public string Phone { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(100)]
        [Display(Name = "Identification Number")]
        public string? IdentificationNumber { get; set; }

        [StringLength(2)]
        [Display(Name = "Identification State")]
        public string? IdentificationState { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Status")]
        public string Status { get; set; } = string.Empty; // Lead, TourScheduled, Applied, Screening, Approved, Denied, ConvertedToTenant

        [StringLength(100)]
        [Display(Name = "Source")]
        public string? Source { get; set; } // Website, Referral, Walk-in, Zillow, etc.

        [StringLength(2000)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        [Display(Name = "Interested Property")]
        public int? InterestedPropertyId { get; set; }

        [Display(Name = "Desired Move-In Date")]
        public DateTime? DesiredMoveInDate { get; set; }

        [Display(Name = "First Contact Date")]
        public DateTime? FirstContactedOn { get; set; }

        

        // Computed Property
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        // Navigation properties
        [ForeignKey(nameof(InterestedPropertyId))]
        public virtual Property? InterestedProperty { get; set; }

        public virtual ICollection<Tour> Tours { get; set; } = new List<Tour>();

        public virtual RentalApplication? Application { get; set; }
    }
}
