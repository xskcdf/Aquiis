using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Aquiis.SimpleStart.Components.PropertyManagement.Properties;
using Aquiis.SimpleStart.Components.PropertyManagement.Leases;
using Aquiis.SimpleStart.Components.PropertyManagement.Documents;

namespace Aquiis.SimpleStart.Models
{
    public class Checklist : BaseModel
    {
        [Display(Name = "Property ID")]
        public int? PropertyId { get; set; }

        [Display(Name = "Lease ID")]
        public int? LeaseId { get; set; }

        [Required]
        [Display(Name = "Checklist Template ID")]
        public int ChecklistTemplateId { get; set; }

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
        public int? DocumentId { get; set; }

        [StringLength(2000)]
        [Display(Name = "General Notes")]
        public string? GeneralNotes { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Organization ID")]
        public string OrganizationId { get; set; } = string.Empty;

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
