using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aquiis.Core.Entities
{
    /// <summary>
    /// Represents a timeline note/comment that can be attached to any entity
    /// </summary>
    public class Note : BaseModel
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Organization ID")]
        public Guid OrganizationId { get; set; } = Guid.Empty;

        [Required]
        [StringLength(5000)]
        [Display(Name = "Content")]
        public string Content { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Entity Type")]
        public string EntityType { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Entity ID")]
        public Guid EntityId { get; set; }

        [StringLength(100)]
        [Display(Name = "User Full Name")]
        public string? UserFullName { get; set; }

        // CreatedBy (UserId) comes from BaseModel - no navigation property needed in Core
        // Individual projects can add navigation properties to their ApplicationUser if needed

            // public partial class Note
            // {
            //     [ForeignKey(nameof(CreatedBy))]
            //     public virtual ApplicationUser? User { get; set; }
            // }
    }
}
