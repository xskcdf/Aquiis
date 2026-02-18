using Aquiis.Core.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using Aquiis.Core.Entities;
using Aquiis.Core.Interfaces;

namespace Aquiis.Application.Services
{
    /// <summary>
    /// Service for managing calendar events and synchronizing with schedulable entities
    /// </summary>
    public class CalendarEventService : ICalendarEventService
    {
        private readonly ApplicationDbContext _context;
        private readonly CalendarSettingsService _settingsService;
        private readonly IUserContextService _userContextService;

        public CalendarEventService(ApplicationDbContext context, CalendarSettingsService settingsService, IUserContextService userContext)
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
            // Service sets tracking fields - UI should not set these
            var organizationId = await _userContextService.GetActiveOrganizationIdAsync();
            var userId = await _userContextService.GetUserIdAsync();
            
            calendarEvent.Id = Guid.NewGuid();
            calendarEvent.OrganizationId = organizationId ?? Guid.Empty;
            calendarEvent.CreatedBy = userId ?? string.Empty;
            calendarEvent.CreatedOn = DateTime.UtcNow;
            
            // Not linked to a source entity (user-created from calendar UI)
            calendarEvent.SourceEntityId = null;
            calendarEvent.SourceEntityType = null;
            
            // Set color and icon from event type if not already set
            if (string.IsNullOrEmpty(calendarEvent.Color))
            {
                calendarEvent.Color = CalendarEventTypes.GetColor(calendarEvent.EventType ?? CalendarEventTypes.Custom);
            }
            if (string.IsNullOrEmpty(calendarEvent.Icon))
            {
                calendarEvent.Icon = CalendarEventTypes.GetIcon(calendarEvent.EventType ?? CalendarEventTypes.Custom);
            }

            _context.CalendarEvents.Add(calendarEvent);
            await _context.SaveChangesAsync();

            return calendarEvent;
        }

        /// <summary>
        /// Update a custom calendar event
        /// </summary>
        public async Task<CalendarEvent?> UpdateCustomEventAsync(CalendarEvent calendarEvent)
        {
            var organizationId = await _userContextService.GetActiveOrganizationIdAsync();
            var userId = await _userContextService.GetUserIdAsync();
            
            var existing = await _context.CalendarEvents
                .FirstOrDefaultAsync(e => e.Id == calendarEvent.Id 
                    && e.OrganizationId == organizationId
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
            
            // Service sets tracking fields
            existing.LastModifiedBy = userId ?? string.Empty;
            existing.LastModifiedOn = DateTime.UtcNow;

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
            var propertyId = entity.GetPropertyId();
            
            // Get property address for Location field
            var property = propertyId.HasValue 
                ? _context.Properties.FirstOrDefault(p => p.Id == propertyId.Value) 
                : null;
            
            return new CalendarEvent
            {
                Id = Guid.NewGuid(),
                Title = entity.GetEventTitle(),
                StartOn = entity.GetEventStart(),
                DurationMinutes = entity.GetEventDuration(),
                EventType = eventType,
                Status = entity.GetEventStatus(),
                Description = entity.GetEventDescription(),
                PropertyId = propertyId,
                Location = property?.Address ?? string.Empty, // Set location to property address
                Color = CalendarEventTypes.GetColor(eventType),
                Icon = CalendarEventTypes.GetIcon(eventType),
                SourceEntityId = entity.Id,
                SourceEntityType = typeof(T).Name,
                OrganizationId = entity.OrganizationId,
                CreatedBy = entity.CreatedBy,
                CreatedOn = DateTime.UtcNow,
                IsSampleData = entity.IsSampleData // Inherit sample data flag from entity
            };
        }

        /// <summary>
        /// Update a CalendarEvent from a schedulable entity
        /// </summary>
        private void UpdateEventFromEntity<T>(CalendarEvent evt, T entity) 
            where T : BaseModel, ISchedulableEntity
        {
            var propertyId = entity.GetPropertyId();
            
            // Get property address for Location field
            var property = propertyId.HasValue 
                ? _context.Properties.FirstOrDefault(p => p.Id == propertyId.Value) 
                : null;
            
            evt.Title = entity.GetEventTitle();
            evt.StartOn = entity.GetEventStart();
            evt.DurationMinutes = entity.GetEventDuration();
            evt.EventType = entity.GetEventType();
            evt.Status = entity.GetEventStatus();
            evt.Description = entity.GetEventDescription();
            evt.PropertyId = propertyId;
            evt.Location = property?.Address ?? string.Empty; // Update location to property address
            evt.Color = CalendarEventTypes.GetColor(entity.GetEventType());
            evt.Icon = CalendarEventTypes.GetIcon(entity.GetEventType());
            evt.IsSampleData = entity.IsSampleData; // Inherit sample data flag from entity
        }
    }
}
