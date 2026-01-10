using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aquiis.Core.Entities {

    public class Payment : BaseModel
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Organization ID")]
        public Guid OrganizationId { get; set; } = Guid.Empty;

        [Required]
        public Guid InvoiceId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime PaidOn { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [StringLength(50)]
        public string PaymentMethod { get; set; } = string.Empty; // e.g., Cash, Check, CreditCard, BankTransfer

        [StringLength(1000)]
        public string Notes { get; set; } = string.Empty;

        // Document Tracking
        public Guid? DocumentId { get; set; }

        // Navigation properties
        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoice { get; set; } = null!;

        [ForeignKey("DocumentId")]
        public virtual Document? Document { get; set; }

    }
}