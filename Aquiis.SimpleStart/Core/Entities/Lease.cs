using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aquiis.SimpleStart.Core.Entities
{
    
    public class Lease : BaseModel
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Organization ID")]
        public string OrganizationId { get; set; } = string.Empty;

        [Required]
        public int PropertyId { get; set; }

        [Required]
        public int TenantId { get; set; }

        // Reference to the lease offer if this lease was created from an accepted offer
        public int? LeaseOfferId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MonthlyRent { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SecurityDeposit { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Active"; // Active, Pending, Expired, Terminated

        [StringLength(1000)]
        public string Terms { get; set; } = string.Empty;

        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;

        // Lease Offer & Acceptance Tracking
        public DateTime? OfferedOn { get; set; }
        
        public DateTime? SignedOn { get; set; }
        
        public DateTime? DeclinedOn { get; set; }
        
        public DateTime? ExpiresOn { get; set; } // Lease offer expires 30 days from OfferedOn

        // Lease Renewal Tracking
        public bool? RenewalNotificationSent { get; set; }
        
        public DateTime? RenewalNotificationSentOn { get; set; }
        
        public DateTime? RenewalReminderSentOn { get; set; }
        
        [StringLength(50)]
        public string? RenewalStatus { get; set; } // NotRequired, Pending, Offered, Accepted, Declined, Expired
        
        public DateTime? RenewalOfferedOn { get; set; }
        
        public DateTime? RenewalResponseOn { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? ProposedRenewalRent { get; set; }
        
        [StringLength(1000)]
        public string? RenewalNotes { get; set; }

        // Lease Chain Tracking
        public int? PreviousLeaseId { get; set; }

        // Document Tracking
        public int? DocumentId { get; set; }

        // Navigation properties
        [ForeignKey("PropertyId")]
        public virtual Property Property { get; set; } = null!;

        [ForeignKey("TenantId")]
        public virtual Tenant? Tenant { get; set; }

        [ForeignKey("DocumentId")]
        public virtual Document? Document { get; set; }

        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

        // Computed properties
        public bool IsActive => Status == "Active" && DateTime.Now >= StartDate && DateTime.Now <= EndDate;
        public int DaysRemaining => EndDate > DateTime.Now ? (EndDate - DateTime.Now).Days : 0;
        public bool IsExpiringSoon => DaysRemaining > 0 && DaysRemaining <= 90;
        public bool IsExpired => DateTime.Now > EndDate;
    }
}