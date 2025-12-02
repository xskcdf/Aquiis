using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aquiis.SimpleStart.Core.Entities
{
    public class ChecklistTemplateItem : BaseModel
    {
        [Required]
        [Display(Name = "Checklist Template ID")]
        public int ChecklistTemplateId { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "Item Text")]
        public string ItemText { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Item Order")]
        public int ItemOrder { get; set; }

        [StringLength(100)]
        [Display(Name = "Category Section")]
        public string? CategorySection { get; set; }

        [Display(Name = "Section Order")]
        public int SectionOrder { get; set; } = 0;

        [Display(Name = "Is Required")]
        public bool IsRequired { get; set; } = false;

        [Display(Name = "Requires Value")]
        public bool RequiresValue { get; set; } = false;

        [Display(Name = "Allows Notes")]
        public bool AllowsNotes { get; set; } = true;

        [Required]
        [StringLength(100)]
        [Display(Name = "Organization ID")]
        public string OrganizationId { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey(nameof(ChecklistTemplateId))]
        public virtual ChecklistTemplate? ChecklistTemplate { get; set; }
    }
}
