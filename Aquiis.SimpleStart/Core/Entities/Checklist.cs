using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Aquiis.SimpleStart.Core.Validation;

namespace Aquiis.SimpleStart.Core.Entities
{
    public class Checklist : BaseModel
    {
        [RequiredGuid]
        [Display(Name = "Organization ID")]
        public Guid OrganizationId { get; set; } = Guid.Empty;

        [Display(Name = "Property ID")]
        public Guid? PropertyId { get; set; }

        [Display(Name = "Lease ID")]
        public Guid? LeaseId { get; set; }

        [RequiredGuid]
        [Display(Name = "Checklist Template ID")]
        public Guid ChecklistTemplateId { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Checklist Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Checklist Type")]
        public string ChecklistType { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Status")]
        public string Status { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Completed By")]
        public string? CompletedBy { get; set; }

        [Display(Name = "Completed On")]
        public DateTime? CompletedOn { get; set; }

        [Display(Name = "Document ID")]
        public Guid? DocumentId { get; set; }

        [StringLength(2000)]
        [Display(Name = "General Notes")]
        public string? GeneralNotes { get; set; }

        // Navigation properties
        [ForeignKey(nameof(PropertyId))]
        public virtual Property? Property { get; set; }

        [ForeignKey(nameof(LeaseId))]
        public virtual Lease? Lease { get; set; }

        [ForeignKey(nameof(ChecklistTemplateId))]
        public virtual ChecklistTemplate? ChecklistTemplate { get; set; }

        [ForeignKey(nameof(DocumentId))]
        public virtual Document? Document { get; set; }

        public virtual ICollection<ChecklistItem> Items { get; set; } = new List<ChecklistItem>();
    }
}
