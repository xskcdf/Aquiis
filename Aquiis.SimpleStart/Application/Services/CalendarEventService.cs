using Microsoft.EntityFrameworkCore;
using Aquiis.SimpleStart.Infrastructure.Data;
using Aquiis.SimpleStart.Core.Entities;
using Aquiis.SimpleStart.Shared.Services;

namespace Aquiis.SimpleStart.Application.Services
{
    /// <summary>
    /// Service for managing calendar events and synchronizing with schedulable entities
    /// </summary>
    public class CalendarEventService
    {
        private readonly ApplicationDbContext _context;
        private readonly CalendarSettingsService _settingsService;
        private readonly UserContextService _userContextService;

        public CalendarEventService(ApplicationDbContext context, CalendarSettingsService settingsService, UserContextService userContext)
        {
            _context = context;
            _settingsService = settingsService;
            _userContextService = userContext;
        }

        /// <summary>
        /// Create or update a calendar event from a schedulable entity
        /// </summary>
        public async Task<CalendarEvent?> CreateOrUpdateEventAsync<T>(T entity) 
            where T : BaseModel, ISchedulableEntity
        {
            var entityType = entity.GetEventType();
            
            // Check if auto-creation is enabled for this entity type
            var isEnabled = await _settingsService.IsAutoCreateEnabledAsync(
                entity.OrganizationId, 
                entityType
            );
            
            if (!isEnabled)
            {
                // If disabled and event exists, delete it
                if (entity.CalendarEventId.HasValue)
                {
                    await DeleteEventAsync(entity.CalendarEventId);
                    entity.CalendarEventId = null;
                    await _context.SaveChangesAsync();
                }
                return null;
            }

            CalendarEvent? calendarEvent;

            if (entity.CalendarEventId.HasValue)
            {
                // Update existing event
                calendarEvent = await _context.CalendarEvents
                    .FindAsync(entity.CalendarEventId.Value);

                if (calendarEvent != null)
                {
                    UpdateEventFromEntity(calendarEvent, entity);
                }
                else
                {
                    // Event was deleted, create new one
                    calendarEvent = CreateEventFromEntity(entity);
                    _context.CalendarEvents.Add(calendarEvent);
                }
            }
            else
            {
                // Create new event
                calendarEvent = CreateEventFromEntity(entity);
                _context.CalendarEvents.Add(calendarEvent);
            }

            await _context.SaveChangesAsync();

            // Link back to entity if not already linked
            if (!entity.CalendarEventId.HasValue)
            {
                entity.CalendarEventId = calendarEvent.Id;
                await _context.SaveChangesAsync();
            }

            return calendarEvent;
        }

        /// <summary>
        /// Delete a calendar event
        /// </summary>
        public async Task DeleteEventAsync(Guid? calendarEventId)
        {
            if (!calendarEventId.HasValue) return;

            var evt = await _context.CalendarEvents.FindAsync(calendarEventId.Value);
            if (evt != null)
            {
                _context.CalendarEvents.Remove(evt);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Get calendar events for a date range with optional filtering
        /// </summary>
        public async Task<List<CalendarEvent>> GetEventsAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? eventTypes = null)
        {
            var organizationId = await _userContextService.GetActiveOrganizationIdAsync();
            var query = _context.CalendarEvents
                .Include(e => e.Property)
                .Where(e => e.OrganizationId == organizationId
                    && e.StartOn >= startDate
                    && e.StartOn <= endDate
                    && !e.IsDeleted);

            if (eventTypes?.Any() == true)
            {
                query = query.Where(e => eventTypes.Contains(e.EventType));
            }

            return await query.OrderBy(e => e.StartOn).ToListAsync();
        }

        /// <summary>
        /// Get a specific calendar event by ID
        /// </summary>
        public async Task<CalendarEvent?> GetEventByIdAsync(Guid eventId)
        {
            var organizationId = await _userContextService.GetActiveOrganizationIdAsync();
            return await _context.CalendarEvents
                .Include(e => e.Property)
                .FirstOrDefaultAsync(e => e.Id == eventId 
                    && e.OrganizationId == organizationId 
                    && !e.IsDeleted);
        }

        /// <summary>
        /// Create a custom calendar event (not linked to a domain entity)
        /// </summary>
        public async Task<CalendarEvent> CreateCustomEventAsync(CalendarEvent calendarEvent)
        {
            calendarEvent.EventType = CalendarEventTypes.Custom;
            calendarEvent.SourceEntityId = null;
            calendarEvent.SourceEntityType = null;
            calendarEvent.Color = CalendarEventTypes.GetColor(CalendarEventTypes.Custom);
            calendarEvent.Icon = CalendarEventTypes.GetIcon(CalendarEventTypes.Custom);
            calendarEvent.CreatedOn = DateTime.UtcNow;

            _context.CalendarEvents.Add(calendarEvent);
            await _context.SaveChangesAsync();

            return calendarEvent;
        }

        /// <summary>
        /// Update a custom calendar event
        /// </summary>
        public async Task<CalendarEvent?> UpdateCustomEventAsync(CalendarEvent calendarEvent)
        {
            var existing = await _context.CalendarEvents
                .FirstOrDefaultAsync(e => e.Id == calendarEvent.Id 
                    && e.OrganizationId == calendarEvent.OrganizationId 
                    && e.SourceEntityType == null
                    && !e.IsDeleted);

            if (existing == null) return null;

            existing.Title = calendarEvent.Title;
            existing.StartOn = calendarEvent.StartOn;
            existing.EndOn = calendarEvent.EndOn;
            existing.DurationMinutes = calendarEvent.DurationMinutes;
            existing.Description = calendarEvent.Description;
            existing.PropertyId = calendarEvent.PropertyId;
            existing.Location = calendarEvent.Location;
            existing.Status = calendarEvent.Status;
            existing.LastModifiedBy = calendarEvent.LastModifiedBy;
            existing.LastModifiedOn = calendarEvent.LastModifiedOn;

            await _context.SaveChangesAsync();

            return existing;
        }

        /// <summary>
        /// Get all calendar events for a specific property
        /// </summary>
        public async Task<List<CalendarEvent>> GetEventsByPropertyIdAsync(Guid propertyId)
        {
            var organizationId = await _userContextService.GetActiveOrganizationIdAsync();
            return await _context.CalendarEvents
                .Include(e => e.Property)
                .Where(e => e.PropertyId == propertyId 
                    && e.OrganizationId == organizationId 
                    && !e.IsDeleted)
                .OrderByDescending(e => e.StartOn)
                .ToListAsync();
        }

        /// <summary>
        /// Get upcoming events for the next N days
        /// </summary>
        public async Task<List<CalendarEvent>> GetUpcomingEventsAsync(
            int days = 7,
            List<string>? eventTypes = null)
        {
            var startDate = DateTime.Today;
            var endDate = DateTime.Today.AddDays(days);
            return await GetEventsAsync(startDate, endDate, eventTypes);
        }

        /// <summary>
        /// Create a CalendarEvent from a schedulable entity
        /// </summary>
        private CalendarEvent CreateEventFromEntity<T>(T entity) 
            where T : BaseModel, ISchedulableEntity
        {
            var eventType = entity.GetEventType();
            
            return new CalendarEvent
            {
                Id = Guid.NewGuid(),
                Title = entity.GetEventTitle(),
                StartOn = entity.GetEventStart(),
                DurationMinutes = entity.GetEventDuration(),
                EventType = eventType,
                Status = entity.GetEventStatus(),
                Description = entity.GetEventDescription(),
                PropertyId = entity.GetPropertyId(),
                Color = CalendarEventTypes.GetColor(eventType),
                Icon = CalendarEventTypes.GetIcon(eventType),
                SourceEntityId = entity.Id,
                SourceEntityType = typeof(T).Name,
                OrganizationId = entity.OrganizationId,
                CreatedBy = entity.CreatedBy,
                CreatedOn = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Update a CalendarEvent from a schedulable entity
        /// </summary>
        private void UpdateEventFromEntity<T>(CalendarEvent evt, T entity) 
            where T : ISchedulableEntity
        {
            evt.Title = entity.GetEventTitle();
            evt.StartOn = entity.GetEventStart();
            evt.DurationMinutes = entity.GetEventDuration();
            evt.EventType = entity.GetEventType();
            evt.Status = entity.GetEventStatus();
            evt.Description = entity.GetEventDescription();
            evt.PropertyId = entity.GetPropertyId();
            evt.Color = CalendarEventTypes.GetColor(entity.GetEventType());
            evt.Icon = CalendarEventTypes.GetIcon(entity.GetEventType());
        }
    }
}
