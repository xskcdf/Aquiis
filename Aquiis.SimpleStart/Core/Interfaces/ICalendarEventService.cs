using Aquiis.SimpleStart.Core.Entities;

namespace Aquiis.SimpleStart.Core.Interfaces
{
    /// <summary>
    /// Service interface for managing calendar events and synchronizing with schedulable entities
    /// </summary>
    public interface ICalendarEventService
    {
        /// <summary>
        /// Create or update a calendar event from a schedulable entity
        /// </summary>
        Task<CalendarEvent?> CreateOrUpdateEventAsync<T>(T entity) 
            where T : BaseModel, ISchedulableEntity;

        /// <summary>
        /// Delete a calendar event
        /// </summary>
        Task DeleteEventAsync(Guid? calendarEventId);
    }
}
