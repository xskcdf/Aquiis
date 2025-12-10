using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Aquiis.SimpleStart.Core.Validation;

namespace Aquiis.SimpleStart.Core.Entities
{
    public class ApplicationScreening : BaseModel
    {
        [RequiredGuid]
        [Display(Name = "Organization ID")]
        public Guid OrganizationId { get; set; } = Guid.Empty;

        [RequiredGuid]
        [Display(Name = "Rental Application")]
        public Guid RentalApplicationId { get; set; }

        // Background Check
        [Display(Name = "Background Check Requested")]
        public bool BackgroundCheckRequested { get; set; }

        [Display(Name = "Background Check Requested Date")]
        public DateTime? BackgroundCheckRequestedOn { get; set; }

        [Display(Name = "Background Check Passed")]
        public bool? BackgroundCheckPassed { get; set; }

        [Display(Name = "Background Check Completed Date")]
        public DateTime? BackgroundCheckCompletedOn { get; set; }

        [StringLength(1000)]
        [Display(Name = "Background Check Notes")]
        public string? BackgroundCheckNotes { get; set; }

        // Credit Check
        [Display(Name = "Credit Check Requested")]
        public bool CreditCheckRequested { get; set; }

        [Display(Name = "Credit Check Requested Date")]
        public DateTime? CreditCheckRequestedOn { get; set; }

        [Display(Name = "Credit Score")]
        public int? CreditScore { get; set; }

        [Display(Name = "Credit Check Passed")]
        public bool? CreditCheckPassed { get; set; }

        [Display(Name = "Credit Check Completed Date")]
        public DateTime? CreditCheckCompletedOn { get; set; }

        [StringLength(1000)]
        [Display(Name = "Credit Check Notes")]
        public string? CreditCheckNotes { get; set; }

        // Overall Result
        [Required]
        [StringLength(50)]
        [Display(Name = "Overall Result")]
        public string OverallResult { get; set; } = string.Empty; // Pending, Passed, Failed, ConditionalPass

        [StringLength(2000)]
        [Display(Name = "Result Notes")]
        public string? ResultNotes { get; set; }

        // Navigation properties
        [ForeignKey(nameof(RentalApplicationId))]
        public virtual RentalApplication? RentalApplication { get; set; }
    }
}
