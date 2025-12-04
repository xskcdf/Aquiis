using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Aquiis.SimpleStart.Core.Entities
{
    public class RentalApplication : BaseModel
    {
        [Required]
        [JsonInclude]
        [StringLength(100)]
        [DataType(DataType.Text)]
        [Display(Name = "Organization ID")]
        public string OrganizationId { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Prospective Tenant")]
        public int ProspectiveTenantId { get; set; }

        [Required]
        [Display(Name = "Property")]
        public int PropertyId { get; set; }

        [Required]
        [Display(Name = "Applied On")]
        public DateTime AppliedOn { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Status")]
        public string Status { get; set; } = string.Empty; // Submitted, UnderReview, Screening, Approved, Denied

        // Current Address
        [Required]
        [StringLength(200)]
        [Display(Name = "Current Address")]
        public string CurrentAddress { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "City")]
        public string CurrentCity { get; set; } = string.Empty;

        [Required]
        [StringLength(2)]
        [Display(Name = "State")]
        public string CurrentState { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        [Display(Name = "Zip Code")]
        public string CurrentZipCode { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Current Rent")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentRent { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Landlord Name")]
        public string LandlordName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Phone]
        [Display(Name = "Landlord Phone")]
        public string LandlordPhone { get; set; } = string.Empty;

        // Employment
        [Required]
        [StringLength(200)]
        [Display(Name = "Employer Name")]
        public string EmployerName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Job Title")]
        public string JobTitle { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Monthly Income")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MonthlyIncome { get; set; }

        [Required]
        [Display(Name = "Employment Length (Months)")]
        public int EmploymentLengthMonths { get; set; }

        // References
        [Required]
        [StringLength(200)]
        [Display(Name = "Reference 1 - Name")]
        public string Reference1Name { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Phone]
        [Display(Name = "Reference 1 - Phone")]
        public string Reference1Phone { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Reference 1 - Relationship")]
        public string Reference1Relationship { get; set; } = string.Empty;

        [StringLength(200)]
        [Display(Name = "Reference 2 - Name")]
        public string? Reference2Name { get; set; }

        [StringLength(20)]
        [Phone]
        [Display(Name = "Reference 2 - Phone")]
        public string? Reference2Phone { get; set; }

        [StringLength(100)]
        [Display(Name = "Reference 2 - Relationship")]
        public string? Reference2Relationship { get; set; }

        // Fees
        [Required]
        [Display(Name = "Application Fee")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ApplicationFee { get; set; }

        [Display(Name = "Application Fee Paid")]
        public bool ApplicationFeePaid { get; set; }

        [Display(Name = "Fee Paid On")]
        public DateTime? ApplicationFeePaidOn { get; set; }

        [StringLength(50)]
        [Display(Name = "Payment Method")]
        public string? ApplicationFeePaymentMethod { get; set; }

        [Display(Name = "Expires On")]
        public DateTime? ExpiresOn { get; set; }

        // Decision
        [StringLength(1000)]
        [Display(Name = "Denial Reason")]
        public string? DenialReason { get; set; }

        [Display(Name = "Decided On")]
        public DateTime? DecidedOn { get; set; }

        [StringLength(100)]
        [Display(Name = "Decision By")]
        public string? DecisionBy { get; set; } // UserId

        
        // Navigation properties
        [ForeignKey(nameof(ProspectiveTenantId))]
        public virtual ProspectiveTenant? ProspectiveTenant { get; set; }

        [ForeignKey(nameof(PropertyId))]
        public virtual Property? Property { get; set; }

        public virtual ApplicationScreening? Screening { get; set; }
    }
}
