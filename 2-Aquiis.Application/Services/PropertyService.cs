using Aquiis.Core.Interfaces.Services;
using System.ComponentModel.DataAnnotations;
using Aquiis.Core.Constants;
using Aquiis.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Aquiis.Application.Services
{
    /// <summary>
    /// Service for managing Property entities.
    /// Inherits common CRUD operations from BaseService and adds property-specific business logic.
    /// </summary>
    public class PropertyService : BaseService<Property>
    {
        private readonly CalendarEventService _calendarEventService;
        private readonly ApplicationSettings _appSettings;

        private readonly NotificationService _notificationService;

        public PropertyService(
            ApplicationDbContext context,
            ILogger<PropertyService> logger,
            IUserContextService userContext,
            IOptions<ApplicationSettings> settings,
            CalendarEventService calendarEventService, NotificationService notificationService)
            : base(context, logger, userContext, settings)
        {
            _calendarEventService = calendarEventService;
            _notificationService = notificationService;
            _appSettings = settings.Value;
        }

        #region Overrides with Property-Specific Logic

        /// <summary>
        /// Creates a new property with initial routine inspection scheduling.
        /// </summary>
        public override async Task<Property> CreateAsync(Property property)
        {
            // Set initial routine inspection due date to 30 days from creation
            property.NextRoutineInspectionDueDate = DateTime.Today.AddDays(30);

            // Call base create (handles audit fields, org assignment, validation)
            var createdProperty = await base.CreateAsync(property);

            // Create calendar event for the first routine inspection
            await CreateRoutineInspectionCalendarEventAsync(createdProperty);

            return createdProperty;
        }

        /// <summary>
        /// Retrieves a property by ID with related entities (Leases, Documents).
        /// </summary>
        public async Task<Property?> GetPropertyWithRelationsAsync(Guid propertyId)
        {
            try
            {
                var userId = await _userContext.GetUserIdAsync();
                if (string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException("User is not authenticated.");
                }

                var organizationId = await _userContext.GetActiveOrganizationIdAsync();

                return await _context.Properties
                    .Include(p => p.Leases)
                    .Include(p => p.Documents)
                    .FirstOrDefaultAsync(p => p.Id == propertyId && 
                                            p.OrganizationId == organizationId && 
                                            !p.IsDeleted);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, "GetPropertyWithRelations");
                throw;
            }
        }

        /// <summary>
        /// Retrieves all properties with related entities.
        /// </summary>
        public async Task<List<Property>> GetPropertiesWithRelationsAsync()
        {
            try
            {
                var userId = await _userContext.GetUserIdAsync();
                if (string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException("User is not authenticated.");
                }

                var organizationId = await _userContext.GetActiveOrganizationIdAsync();

                return await _context.Properties
                    .Include(p => p.Leases)
                    .Include(p => p.Documents)
                    .Where(p => !p.IsDeleted && p.OrganizationId == organizationId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, "GetPropertiesWithRelations");
                throw;
            }
        }

        /// <summary>
        /// Validates property data before create/update operations.
        /// </summary>
        protected override async Task ValidateEntityAsync(Property property)
        {
            // Validate required address
            if (string.IsNullOrWhiteSpace(property.Address))
            {
                throw new ValidationException("Property address is required.");
            }

            // Check for duplicate address in same organization
            var userId = await _userContext.GetUserIdAsync();
            var organizationId = await _userContext.GetActiveOrganizationIdAsync();

            var exists = await _context.Properties
                .AnyAsync(p => p.Address == property.Address && 
                             p.City == property.City &&
                             p.State == property.State &&
                             p.ZipCode == property.ZipCode &&
                             p.Id != property.Id && 
                             p.OrganizationId == organizationId &&
                             !p.IsDeleted);

            if (exists)
            {
                throw new ValidationException($"A property with address '{property.Address}' already exists in this location.");
            }

            await base.ValidateEntityAsync(property);
        }

        #endregion

        #region Business Logic Methods

        /// <summary>
        /// Searches properties by address, city, state, or zip code.
        /// </summary>
        public async Task<List<Property>> SearchPropertiesByAddressAsync(string searchTerm)
        {
            try
            {
                var userId = await _userContext.GetUserIdAsync();
                if (string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException("User is not authenticated.");
                }

                var organizationId = await _userContext.GetActiveOrganizationIdAsync();

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return await _context.Properties
                        .Where(p => !p.IsDeleted && p.OrganizationId == organizationId)
                        .OrderBy(p => p.Address)
                        .Take(20)
                        .ToListAsync();
                }

                return await _context.Properties
                    .Where(p => !p.IsDeleted &&
                               p.OrganizationId == organizationId &&
                               (p.Address.Contains(searchTerm) ||
                                p.City.Contains(searchTerm) ||
                                p.State.Contains(searchTerm) ||
                                p.ZipCode.Contains(searchTerm)))
                    .OrderBy(p => p.Address)
                    .Take(20)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, "SearchPropertiesByAddress");
                throw;
            }
        }

        /// <summary>
        /// Retrieves all vacant properties (no active leases).
        /// </summary>
        public async Task<List<Property>> GetVacantPropertiesAsync()
        {
            try
            {
                var userId = await _userContext.GetUserIdAsync();
                if (string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException("User is not authenticated.");
                }

                var organizationId = await _userContext.GetActiveOrganizationIdAsync();

                return await _context.Properties
                    .Where(p => !p.IsDeleted && 
                               p.IsAvailable && 
                               p.OrganizationId == organizationId)
                    .Where(p => !_context.Leases.Any(l =>
                        l.PropertyId == p.Id &&
                        l.Status == Core.Constants.ApplicationConstants.LeaseStatuses.Active &&
                        !l.IsDeleted))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, "GetVacantProperties");
                throw;
            }
        }

        /// <summary>
        /// Calculates the overall occupancy rate for the organization.
        /// </summary>
        public async Task<decimal> CalculateOccupancyRateAsync()
        {
            try
            {
                var userId = await _userContext.GetUserIdAsync();
                if (string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException("User is not authenticated.");
                }

                var organizationId = await _userContext.GetActiveOrganizationIdAsync();

                var totalProperties = await _context.Properties
                    .CountAsync(p => !p.IsDeleted && p.IsAvailable && p.OrganizationId == organizationId);

                if (totalProperties == 0)
                {
                    return 0;
                }

                var occupiedProperties = await _context.Properties
                    .CountAsync(p => !p.IsDeleted && 
                                    p.IsAvailable && 
                                    p.OrganizationId == organizationId &&
                                    _context.Leases.Any(l =>
                                        l.PropertyId == p.Id &&
                                        l.Status == Core.Constants.ApplicationConstants.LeaseStatuses.Active &&
                                        !l.IsDeleted));

                return (decimal)occupiedProperties / totalProperties * 100;
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, "CalculateOccupancyRate");
                throw;
            }
        }

        /// <summary>
        /// Retrieves properties that need routine inspection.
        /// </summary>
        public async Task<List<Property>> GetPropertiesDueForInspectionAsync(int daysAhead = 7)
        {
            try
            {
                var userId = await _userContext.GetUserIdAsync();
                if (string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException("User is not authenticated.");
                }

                var organizationId = await _userContext.GetActiveOrganizationIdAsync();
                var cutoffDate = DateTime.Today.AddDays(daysAhead);

                return await _context.Properties
                    .Where(p => !p.IsDeleted && 
                               p.OrganizationId == organizationId &&
                               p.NextRoutineInspectionDueDate.HasValue &&
                               p.NextRoutineInspectionDueDate.Value <= cutoffDate)
                    .OrderBy(p => p.NextRoutineInspectionDueDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, "GetPropertiesDueForInspection");
                throw;
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Creates a calendar event for routine property inspection.
        /// </summary>
        private async Task CreateRoutineInspectionCalendarEventAsync(Property property)
        {
            if (!property.NextRoutineInspectionDueDate.HasValue)
            {
                return;
            }

            var userId = await _userContext.GetUserIdAsync();
            var organizationId = await _userContext.GetActiveOrganizationIdAsync();

            var calendarEvent = new CalendarEvent
            {
                Id = Guid.NewGuid(),
                Title = $"Routine Inspection - {property.Address}",
                Description = $"Scheduled routine inspection for property at {property.Address}",
                StartOn = property.NextRoutineInspectionDueDate.Value,
                EndOn = property.NextRoutineInspectionDueDate.Value.AddHours(1),
                DurationMinutes = 60,
                Location = property.Address,
                SourceEntityType = nameof(Property),
                SourceEntityId = property.Id,
                PropertyId = property.Id,
                OrganizationId = organizationId!.Value,
                CreatedBy = userId!,
                CreatedOn = DateTime.UtcNow,
                EventType = "Inspection",
                Status = "Scheduled"
            };

            await _notificationService.CreateAsync(new Notification
            {
                Id = Guid.NewGuid(),
                Type = NotificationConstants.Types.Info,
                Category = NotificationConstants.Categories.CalendarEvent,
                Title = "Routine Inspection Scheduled",
                Message = $"A routine inspection has been scheduled for the property at {property.Address} on {calendarEvent.StartOn:d}.",
                RecipientUserId = userId!,
                RelatedEntityId = calendarEvent.PropertyId,
                RelatedEntityType = nameof(Property),
                SentOn = DateTime.UtcNow,
                OrganizationId = organizationId!.Value,
                CreatedBy = userId!,
                CreatedOn = DateTime.UtcNow
            });

            await _calendarEventService.CreateCustomEventAsync(calendarEvent);
        }

        /// <summary>
        /// Gets properties with overdue routine inspections.
        /// </summary>
        public async Task<List<Property>> GetPropertiesWithOverdueInspectionsAsync()
        {
            try
            {
                var organizationId = await _userContext.GetOrganizationIdAsync();
                
                return await _context.Properties
                    .Where(p => p.OrganizationId == organizationId && 
                               !p.IsDeleted &&
                               p.NextRoutineInspectionDueDate.HasValue &&
                               p.NextRoutineInspectionDueDate.Value < DateTime.Today)
                    .OrderBy(p => p.NextRoutineInspectionDueDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, "GetPropertiesWithOverdueInspections");
                throw;
            }
        }

        /// <summary>
        /// Gets properties with inspections due within specified days.
        /// </summary>
        public async Task<List<Property>> GetPropertiesWithInspectionsDueSoonAsync(int daysAhead = 30)
        {
            try
            {
                var organizationId = await _userContext.GetOrganizationIdAsync();
                var dueDate = DateTime.Today.AddDays(daysAhead);
                
                return await _context.Properties
                    .Where(p => p.OrganizationId == organizationId && 
                               !p.IsDeleted &&
                               p.NextRoutineInspectionDueDate.HasValue &&
                               p.NextRoutineInspectionDueDate.Value >= DateTime.Today &&
                               p.NextRoutineInspectionDueDate.Value <= dueDate)
                    .OrderBy(p => p.NextRoutineInspectionDueDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, "GetPropertiesWithInspectionsDueSoon");
                throw;
            }
        }

       

        #endregion
    }
}
