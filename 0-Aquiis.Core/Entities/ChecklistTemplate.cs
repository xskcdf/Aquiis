using System.ComponentModel.DataAnnotations;
using Aquiis.Core.Validation;

namespace Aquiis.Core.Entities
{
    public class ChecklistTemplate : BaseModel
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Template Name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Category")]
        public string Category { get; set; } = string.Empty;

        [Display(Name = "Is System Template")]
        public bool IsSystemTemplate { get; set; } = false;

        // Navigation properties
        public virtual ICollection<ChecklistTemplateItem> Items { get; set; } = new List<ChecklistTemplateItem>();
        public virtual ICollection<Checklist> Checklists { get; set; } = new List<Checklist>();
    }
}
