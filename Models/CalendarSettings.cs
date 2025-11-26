namespace Aquiis.SimpleStart.Models;

public class CalendarSettings : BaseModel
{
    public Guid OrganizationId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public bool AutoCreateEvents { get; set; } = true;
    public bool ShowOnCalendar { get; set; } = true;
    public string? DefaultColor { get; set; }
    public string? DefaultIcon { get; set; }
    public int DisplayOrder { get; set; }
}
