using System.ComponentModel.DataAnnotations;

namespace Aquiis.SimpleStart.Features.PropertyManagement.Repairs;

/// <summary>
/// Form model for repair create/edit operations.
/// Contains only user-provided fields, not tracking fields.
/// </summary>
public class RepairFormModel
{
    [Required(ErrorMessage = "Property is required")]
    public Guid PropertyId { get; set; }

    [Required(ErrorMessage = "Description is required")]
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Repair Type is required")]
    [StringLength(50)]
    public string RepairType { get; set; } = string.Empty;

    [StringLength(200)]
    public string ContractorName { get; set; } = string.Empty;

    [StringLength(200)]
    public string ContactPerson { get; set; } = string.Empty;

    [StringLength(50)]
    public string ContractorPhone { get; set; } = string.Empty;

    public DateTime? CompletedOn { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Cost cannot be negative")]
    public decimal Cost { get; set; } = 0;

    [Range(0, int.MaxValue, ErrorMessage = "Duration cannot be negative")]
    public int DurationMinutes { get; set; } = 0;

    [StringLength(500)]
    public string PartsReplaced { get; set; } = string.Empty;

    [StringLength(2000)]
    public string Notes { get; set; } = string.Empty;

    public bool WarrantyApplies { get; set; } = false;

    public DateTime? WarrantyExpiresOn { get; set; }

    public Guid? MaintenanceRequestId { get; set; }

    public Guid? LeaseId { get; set; }
}
