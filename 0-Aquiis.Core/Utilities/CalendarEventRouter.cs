using Aquiis.Core.Entities;

namespace Aquiis.Core.Utilities
{
    /// <summary>
    /// Helper class for routing calendar event clicks to appropriate detail pages
    /// </summary>
    public static class CalendarEventRouter
    {
        /// <summary>
        /// Get the route URL for a calendar event based on its source entity type
        /// </summary>
        /// <param name="evt">The calendar event</param>
        /// <returns>The route URL or null if it's a custom event or routing not available</returns>
        public static string? GetRouteForEvent(CalendarEvent evt)
        {
            if (!evt.SourceEntityId.HasValue || string.IsNullOrEmpty(evt.SourceEntityType))
                return null;

            return evt.SourceEntityType switch
            {
                nameof(Tour) => $"/PropertyManagement/Tours/Details/{evt.SourceEntityId}",
                nameof(Inspection) => $"/PropertyManagement/Inspections/View/{evt.SourceEntityId}",
                nameof(MaintenanceRequest) => $"/PropertyManagement/Maintenance/View/{evt.SourceEntityId}",
                // Add new schedulable entity routes here as they are created
                _ => null
            };
        }

        /// <summary>
        /// Check if an event is routable (has a valid source entity and route)
        /// </summary>
        /// <param name="evt">The calendar event</param>
        /// <returns>True if the event can be routed to a detail page</returns>
        public static bool IsRoutable(CalendarEvent evt)
        {
            return !string.IsNullOrEmpty(GetRouteForEvent(evt));
        }

        /// <summary>
        /// Get a display label for the event type
        /// </summary>
        /// <param name="evt">The calendar event</param>
        /// <returns>User-friendly label for the event source</returns>
        public static string GetSourceLabel(CalendarEvent evt)
        {
            if (evt.IsCustomEvent)
                return "Custom Event";

            return evt.SourceEntityType switch
            {
                nameof(Tour) => "Property Tour",
                nameof(Inspection) => "Property Inspection",
                nameof(MaintenanceRequest) => "Maintenance Request",
                _ => evt.EventType
            };
        }
    }
}
