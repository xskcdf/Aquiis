namespace Aquiis.SimpleStart.Core.Entities
{
    /// <summary>
    /// Interface for entities that can be scheduled on the calendar.
    /// Provides a contract for automatic calendar event creation and synchronization.
    /// </summary>
    public interface ISchedulableEntity
    {
        /// <summary>
        /// Entity ID
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        /// Organization ID
        /// </summary>
        Guid OrganizationId { get; set; }

        /// <summary>
        /// Created By User ID
        /// </summary>
        string CreatedBy { get; set; }

        /// <summary>
        /// Link to the associated CalendarEvent
        /// </summary>
        Guid? CalendarEventId { get; set; }

        /// <summary>
        /// Get the title to display on the calendar
        /// </summary>
        string GetEventTitle();

        /// <summary>
        /// Get the start date/time of the event
        /// </summary>
        DateTime GetEventStart();

        /// <summary>
        /// Get the duration of the event in minutes
        /// </summary>
        int GetEventDuration();

        /// <summary>
        /// Get the event type (from CalendarEventTypes constants)
        /// </summary>
        string GetEventType();

        /// <summary>
        /// Get the associated property ID (if applicable)
        /// </summary>
        Guid? GetPropertyId();

        /// <summary>
        /// Get the description/details for the event
        /// </summary>
        string GetEventDescription();

        /// <summary>
        /// Get the current status of the event
        /// </summary>
        string GetEventStatus();
    }
}
