using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Aquiis.SimpleStart.Components.PropertyManagement.Documents;
using Aquiis.SimpleStart.Components.PropertyManagement.Leases;
using Aquiis.SimpleStart.Components.PropertyManagement.Payments;
using Aquiis.SimpleStart.Models;

namespace Aquiis.SimpleStart.Components.PropertyManagement.Invoices {

    public class Invoice : BaseModel
    {

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int LeaseId { get; set; }

        [Required]
        [StringLength(50)]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime InvoiceDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; } = string.Empty;

        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Paid, Overdue, Cancelled

        public DateTime? PaidOn { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountPaid { get; set; }

        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;

        // Late Fee Properties
        [Column(TypeName = "decimal(18,2)")]
        public decimal? LateFeeAmount { get; set; }

        public bool? LateFeeApplied { get; set; }

        public DateTime? LateFeeAppliedDate { get; set; }

        // Reminder Properties
        public bool? ReminderSent { get; set; }

        public DateTime? ReminderSentDate { get; set; }

    // Document Tracking
    public int? DocumentId { get; set; }

        // Navigation properties
    [ForeignKey("LeaseId")]
    public virtual Lease Lease { get; set; } = null!;

    [ForeignKey("DocumentId")]
    public virtual Document? Document { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

        // Computed properties
        public decimal BalanceDue => Amount - AmountPaid;
        public bool IsOverdue => Status != "Paid" && DueDate < DateTime.Now;
        public int DaysOverdue => IsOverdue ? (DateTime.Now - DueDate).Days : 0;
    }
}