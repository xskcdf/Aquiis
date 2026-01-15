using System.ComponentModel.DataAnnotations;
using Aquiis.Core.Validation;

namespace Aquiis.Core.Entities
{
    public class Organization
    {
        [RequiredGuid]
        [Display(Name = "Organization ID")]
        public Guid Id { get; set; } = Guid.Empty;
        
        /// <summary>
        /// UserId of the account owner who created this organization
        /// </summary>
        public string OwnerId { get; set; } = string.Empty;
        
        /// <summary>
        /// Full organization name (e.g., "California Properties LLC")
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Short display name for UI (e.g., "CA Properties")
        /// </summary>
        public string? DisplayName { get; set; }
        
        /// <summary>
        /// US state code (CA, TX, FL, etc.) - determines applicable regulations
        /// </summary>
        public string? State { get; set; }
        
        /// <summary>
        /// Active/inactive flag for soft delete
        /// </summary>
        public bool IsActive { get; set; } = true;

        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public string? LastModifiedBy { get; set; } = string.Empty;
        public DateTime? LastModifiedOn { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public virtual ICollection<OrganizationUser> OrganizationUsers { get; set; } = new List<OrganizationUser>();
        public virtual ICollection<Property> Properties { get; set; } = new List<Property>();
        public virtual ICollection<Tenant> Tenants { get; set; } = new List<Tenant>();
        public virtual ICollection<Lease> Leases { get; set; } = new List<Lease>();
    }
}
