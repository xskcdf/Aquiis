

using System.ComponentModel.DataAnnotations;
using Aquiis.Core.Validation;

namespace Aquiis.Core.Entities
{
    /// <summary>
    /// Junction table for multi-organization user assignments with role-based permissions
    /// </summary>
    public class OrganizationUser
    {

        [RequiredGuid]
        [Display(Name = "OrganizationUser ID")]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The user being granted access
        /// </summary>
        public string UserId { get; set; } = string.Empty;
        
        /// <summary>
        /// The organization they're being granted access to
        /// </summary>
        [RequiredGuid]
        public Guid OrganizationId { get; set; } = Guid.Empty;
        
        /// <summary>
        /// Role within this organization: "Owner", "Administrator", "PropertyManager", "User"
        /// </summary>
        public string Role { get; set; } = string.Empty;
        
        /// <summary>
        /// UserId of the user who granted this access
        /// </summary>
        public string GrantedBy { get; set; } = string.Empty;
        
        /// <summary>
        /// When access was granted
        /// </summary>
        public DateTime GrantedOn { get; set; }
        
        /// <summary>
        /// When access was revoked (NULL if still active)
        /// </summary>
        public DateTime? RevokedOn { get; set; }
        
        /// <summary>
        /// Active assignment flag
        /// </summary>
        public bool IsActive { get; set; } = true;

        public string CreatedBy { get; set; } = string.Empty;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public string? LastModifiedBy { get; set; } = string.Empty;

        public DateTime? LastModifiedOn { get; set; }

        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public virtual Organization Organization { get; set; } = null!;
    }
}
