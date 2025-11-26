namespace Aquiis.SimpleStart.Models
{
    /// <summary>
    /// Defines calendar event type constants and their visual properties
    /// </summary>
    public static class CalendarEventTypes
    {
        // Event Type Constants
        public const string Tour = "Tour";
        public const string Inspection = "Inspection";
        public const string Maintenance = "Maintenance";
        public const string LeaseExpiry = "LeaseExpiry";
        public const string RentDue = "RentDue";
        public const string Custom = "Custom";

        /// <summary>
        /// Configuration for each event type (color and icon)
        /// </summary>
        public static readonly Dictionary<string, EventTypeConfig> Config = new()
        {
            [Tour] = new EventTypeConfig("#0dcaf0", "bi-calendar-check", "Property Tour"),
            [Inspection] = new EventTypeConfig("#fd7e14", "bi-clipboard-check", "Property Inspection"),
            [Maintenance] = new EventTypeConfig("#dc3545", "bi-tools", "Maintenance Request"),
            [LeaseExpiry] = new EventTypeConfig("#ffc107", "bi-calendar-x", "Lease Expiry"),
            [RentDue] = new EventTypeConfig("#198754", "bi-cash-coin", "Rent Due"),
            [Custom] = new EventTypeConfig("#6c757d", "bi-calendar-event", "Custom Event")
        };

        /// <summary>
        /// Get the color for an event type
        /// </summary>
        public static string GetColor(string eventType)
        {
            return Config.TryGetValue(eventType, out var config) ? config.Color : Config[Custom].Color;
        }

        /// <summary>
        /// Get the icon for an event type
        /// </summary>
        public static string GetIcon(string eventType)
        {
            return Config.TryGetValue(eventType, out var config) ? config.Icon : Config[Custom].Icon;
        }

        /// <summary>
        /// Get the display name for an event type
        /// </summary>
        public static string GetDisplayName(string eventType)
        {
            return Config.TryGetValue(eventType, out var config) ? config.DisplayName : eventType;
        }

        /// <summary>
        /// Get all available event types
        /// </summary>
        public static List<string> GetAllTypes()
        {
            return Config.Keys.ToList();
        }
    }

    /// <summary>
    /// Configuration record for event type visual properties
    /// </summary>
    public record EventTypeConfig(string Color, string Icon, string DisplayName);
}
