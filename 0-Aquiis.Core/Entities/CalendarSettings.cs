using System.ComponentModel.DataAnnotations;
using Aquiis.Core.Validation;

namespace Aquiis.Core.Entities;

public class CalendarSettings : BaseModel
{
    public string EntityType { get; set; } = string.Empty;
    public bool AutoCreateEvents { get; set; } = true;
    public bool ShowOnCalendar { get; set; } = true;
    public string? DefaultColor { get; set; }
    public string? DefaultIcon { get; set; }
    public int DisplayOrder { get; set; }
}
