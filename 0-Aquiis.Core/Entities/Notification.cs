using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Aquiis.Core.Entities;
using Aquiis.Core.Validation;

public class Notification : BaseModel
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Message { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Type { get; set; } = string.Empty; // Info, Warning, Error, Success

    [Required]
    [StringLength(50)]
    public string Category { get; set; } = string.Empty; // Lease, Payment, Maintenance, Application

    [Required]
    public string RecipientUserId { get; set; } = string.Empty;

    [Required]
    public DateTime SentOn { get; set; }

    public DateTime? ReadOn { get; set; }

    public bool IsRead { get; set; }

    // Optional entity reference for "view details" link
    public Guid? RelatedEntityId { get; set; }

    [StringLength(50)]
    public string? RelatedEntityType { get; set; }

    // Delivery channels
    public bool SendInApp { get; set; } = true;
    public bool SendEmail { get; set; }
    public bool SendSMS { get; set; }

    // Delivery status
    public bool EmailSent { get; set; }
    public DateTime? EmailSentOn { get; set; }

    public bool SMSSent { get; set; }
    public DateTime? SMSSentOn { get; set; }

    [StringLength(500)]
    public string? EmailError { get; set; }

    [StringLength(500)]
    public string? SMSError { get; set; }

    // Navigation
    [ForeignKey(nameof(OrganizationId))]
    public virtual Organization? Organization { get; set; }
}