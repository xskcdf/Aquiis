using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Aquiis.SimpleStart.Core.Entities;
using Aquiis.SimpleStart.Core.Entities;
using Aquiis.SimpleStart.Core.Entities;
using Aquiis.SimpleStart.Core.Entities;
using Aquiis.SimpleStart.Core.Entities;
using Aquiis.SimpleStart.Core.Entities;

namespace Aquiis.SimpleStart.Core.Entities {

    public class Payment : BaseModel
    {

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int InvoiceId { get; set; }

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
        public int? DocumentId { get; set; }

        // Navigation properties
        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoice { get; set; } = null!;

        [ForeignKey("DocumentId")]
        public virtual Document? Document { get; set; }

    }
}