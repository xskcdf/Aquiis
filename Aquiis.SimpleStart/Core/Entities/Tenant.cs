using System.ComponentModel.DataAnnotations;
using Aquiis.SimpleStart.Core.Validation;

namespace Aquiis.SimpleStart.Core.Entities {

    public class Tenant : BaseModel
    {

        [RequiredGuid]
        public Guid OrganizationId { get; set; } = Guid.Empty;

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string IdentificationNumber { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        public bool IsActive { get; set; } = true;

        [StringLength(200)]
        public string EmergencyContactName { get; set; } = string.Empty;

        [Phone]
        [StringLength(20)]
        public string? EmergencyContactPhone { get; set; }

        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;

        // Link back to prospect for audit trail
        public Guid? ProspectiveTenantId { get; set; }

        // Navigation properties
        public virtual ICollection<Lease> Leases { get; set; } = new List<Lease>();

        // Computed property
        public string FullName => $"{FirstName} {LastName}";
    }
}