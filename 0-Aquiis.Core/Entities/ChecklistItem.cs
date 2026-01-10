using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Aquiis.Core.Validation;

namespace Aquiis.Core.Entities
{
    public class ChecklistItem : BaseModel
    {

        [RequiredGuid]
        [Display(Name = "Organization ID")]
        public Guid OrganizationId { get; set; } = Guid.Empty;

        [RequiredGuid]
        [Display(Name = "Checklist ID")]
        public Guid ChecklistId { get; set; }

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

        [Display(Name = "Requires Value")]
        public bool RequiresValue { get; set; } = false;

        [StringLength(200)]
        [Display(Name = "Value")]
        public string? Value { get; set; }

        [StringLength(1000)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        [StringLength(500)]
        [Display(Name = "Photo URL")]
        public string? PhotoUrl { get; set; }

        [Display(Name = "Is Checked")]
        public bool IsChecked { get; set; } = false;

        // Navigation properties
        [ForeignKey(nameof(ChecklistId))]
        public virtual Checklist? Checklist { get; set; }
    }
}
