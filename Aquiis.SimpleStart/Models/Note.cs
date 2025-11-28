using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Aquiis.SimpleStart.Components.Account;

namespace Aquiis.SimpleStart.Models
{
    /// <summary>
    /// Represents a timeline note/comment that can be attached to any entity
    /// </summary>
    public class Note : BaseModel
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Organization ID")]
        public string OrganizationId { get; set; } = string.Empty;

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
        public int EntityId { get; set; }

        [StringLength(100)]
        [Display(Name = "User Full Name")]
        public string? UserFullName { get; set; }

        // Navigation to user who created the note
        [ForeignKey(nameof(CreatedBy))]
        public virtual ApplicationUser? User { get; set; }
    }
}
