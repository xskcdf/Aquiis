using System.ComponentModel.DataAnnotations;

namespace Aquiis.SimpleStart.Core.Entities;

public class CalendarSettings : BaseModel
{
    [Required]
    [StringLength(100)]
    [Display(Name = "Organization ID")]
    public string OrganizationId { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public bool AutoCreateEvents { get; set; } = true;
    public bool ShowOnCalendar { get; set; } = true;
    public string? DefaultColor { get; set; }
    public string? DefaultIcon { get; set; }
    public int DisplayOrder { get; set; }
}
