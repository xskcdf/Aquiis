using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Aquiis.SimpleStart.Components.PropertyManagement.Documents;
using Aquiis.SimpleStart.Components.PropertyManagement.Invoices;
using Aquiis.SimpleStart.Components.PropertyManagement.Leases;
using Aquiis.SimpleStart.Components.PropertyManagement.Properties;
using Aquiis.SimpleStart.Components.PropertyManagement.Tenants;
using Aquiis.SimpleStart.Models;

namespace Aquiis.SimpleStart.Components.PropertyManagement.Payments {

    public class Payment : BaseModel
    {

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int InvoiceId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime PaymentDate { get; set; }

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