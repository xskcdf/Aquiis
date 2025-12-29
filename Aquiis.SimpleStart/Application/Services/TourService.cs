using Aquiis.SimpleStart.Core.Constants;
using Aquiis.SimpleStart.Core.Entities;
using Aquiis.SimpleStart.Core.Interfaces;
using Aquiis.SimpleStart.Core.Services;
using Aquiis.SimpleStart.Infrastructure.Data;
using Aquiis.SimpleStart.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Aquiis.SimpleStart.Application.Services
{
    /// <summary>
    /// Service for managing property tours with business logic for scheduling,
    /// prospect tracking, and checklist integration.
    /// </summary>
    public class TourService : BaseService<Tour>
    {
        private readonly ICalendarEventService _calendarEventService;
        private readonly ChecklistService _checklistService;

        public TourService(
            ApplicationDbContext context,
            ILogger<TourService> logger,
            UserContextService userContext,
            IOptions<ApplicationSettings> settings,
            ICalendarEventService calendarEventService,
            ChecklistService checklistService)
            : base(context, logger, userContext, settings)
        {
            _calendarEventService = calendarEventService;
            _checklistService = checklistService;
        }

        #region Helper Methods

        protected async Task<string> GetUserIdAsync()
        {
            var userId = await _userContext.GetUserIdAsync();
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            return userId;
        }

        protected async Task<Guid> GetActiveOrganizationIdAsync()
        {
            var organizationId = await _userContext.GetActiveOrganizationIdAsync();
            if (!organizationId.HasValue)
            {
                throw new UnauthorizedAccessException("No active organization.");
            }
            return organizationId.Value;
        }

        #endregion

        /// <summary>
        /// Validates tour business rules.
        /// </summary>
        protected override async Task ValidateEntityAsync(Tour entity)
        {
            var errors = new List<string>();

            // Required fields
            if (entity.ProspectiveTenantId == Guid.Empty)
            {
                errors.Add("Prospective tenant is required");
            }

            if (entity.PropertyId == Guid.Empty)
            {
                errors.Add("Property is required");
            }

            if (entity.ScheduledOn == default)
            {
                errors.Add("Scheduled date/time is required");
            }

            if (entity.DurationMinutes <= 0)
            {
                errors.Add("Duration must be greater than 0");
            }

            if (errors.Any())
            {
                throw new InvalidOperationException(string.Join("; ", errors));
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// Gets all tours for the active organization.
        /// </summary>
        public override async Task<List<Tour>> GetAllAsync()
        {
            var organizationId = await GetActiveOrganizationIdAsync();

            return await _context.Tours
                .Include(t => t.ProspectiveTenant)
                .Include(t => t.Property)
                .Include(t => t.Checklist)
                .Where(t => !t.IsDeleted && t.OrganizationId == organizationId)
                .OrderBy(t => t.ScheduledOn)
                .ToListAsync();
        }

        /// <summary>
        /// Gets tours by prospective tenant ID.
        /// </summary>
        public async Task<List<Tour>> GetByProspectiveIdAsync(Guid prospectiveTenantId)
        {
            var organizationId = await GetActiveOrganizationIdAsync();

            return await _context.Tours
                .Include(t => t.ProspectiveTenant)
                .Include(t => t.Property)
                .Include(t => t.Checklist)
                .Where(t => t.ProspectiveTenantId == prospectiveTenantId && 
                           !t.IsDeleted && 
                           t.OrganizationId == organizationId)
                .OrderBy(t => t.ScheduledOn)
                .ToListAsync();
        }

        /// <summary>
        /// Gets a single tour by ID with related data.
        /// </summary>
        public override async Task<Tour?> GetByIdAsync(Guid id)
        {
            var organizationId = await GetActiveOrganizationIdAsync();

            return await _context.Tours
                .Include(t => t.ProspectiveTenant)
                .Include(t => t.Property)
                .Include(t => t.Checklist)
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted && t.OrganizationId == organizationId);
        }

        /// <summary>
        /// Creates a new tour with optional checklist from template.
        /// </summary>
        public async Task<Tour> CreateAsync(Tour tour, Guid? checklistTemplateId = null)
        {
            await ValidateEntityAsync(tour);

            var userId = await GetUserIdAsync();
            var organizationId = await GetActiveOrganizationIdAsync();

            tour.Id = Guid.NewGuid();
            tour.OrganizationId = organizationId;
            tour.CreatedBy = userId;
            tour.CreatedOn = DateTime.UtcNow;
            tour.Status = ApplicationConstants.TourStatuses.Scheduled;

            // Get prospect information for checklist
            var prospective = await _context.ProspectiveTenants
                .Include(p => p.InterestedProperty)
                .FirstOrDefaultAsync(p => p.Id == tour.ProspectiveTenantId);

            // Create checklist if template specified
            if (checklistTemplateId.HasValue || prospective != null)
            {
                await CreateTourChecklistAsync(tour, prospective, checklistTemplateId);
            }

            await _context.Tours.AddAsync(tour);
            await _context.SaveChangesAsync();

            // Create calendar event for the tour
            await _calendarEventService.CreateOrUpdateEventAsync(tour);

            // Update prospective tenant status if needed
            if (prospective != null && prospective.Status == ApplicationConstants.ProspectiveStatuses.Lead)
            {
                prospective.Status = ApplicationConstants.ProspectiveStatuses.TourScheduled;
                prospective.LastModifiedOn = DateTime.UtcNow;
                prospective.LastModifiedBy = userId;
                await _context.SaveChangesAsync();
            }

            _logger.LogInformation("Created tour {TourId} for prospect {ProspectId}", 
                tour.Id, tour.ProspectiveTenantId);

            return tour;
        }

        /// <summary>
        /// Creates a tour using the base CreateAsync (without template parameter).
        /// </summary>
        public override async Task<Tour> CreateAsync(Tour tour)
        {
            return await CreateAsync(tour, checklistTemplateId: null);
        }

        /// <summary>
        /// Updates an existing tour.
        /// </summary>
        public override async Task<Tour> UpdateAsync(Tour tour)
        {
            await ValidateEntityAsync(tour);

            var userId = await GetUserIdAsync();
            var organizationId = await GetActiveOrganizationIdAsync();

            // Security: Verify tour belongs to active organization
            var existing = await _context.Tours
                .FirstOrDefaultAsync(t => t.Id == tour.Id && t.OrganizationId == organizationId);

            if (existing == null)
            {
                throw new UnauthorizedAccessException($"Tour {tour.Id} not found in active organization.");
            }

            // Set tracking fields
            tour.LastModifiedBy = userId;
            tour.LastModifiedOn = DateTime.UtcNow;
            tour.OrganizationId = organizationId; // Prevent org hijacking

            _context.Entry(existing).CurrentValues.SetValues(tour);
            await _context.SaveChangesAsync();

            // Update calendar event
            await _calendarEventService.CreateOrUpdateEventAsync(tour);

            _logger.LogInformation("Updated tour {TourId}", tour.Id);

            return tour;
        }

        /// <summary>
        /// Deletes a tour (soft delete).
        /// </summary>
        public override async Task<bool> DeleteAsync(Guid id)
        {
            var userId = await GetUserIdAsync();
            var organizationId = await GetActiveOrganizationIdAsync();

            var tour = await _context.Tours
                .FirstOrDefaultAsync(t => t.Id == id && t.OrganizationId == organizationId);

            if (tour == null)
            {
                throw new KeyNotFoundException($"Tour {id} not found.");
            }

            tour.IsDeleted = true;
            tour.LastModifiedBy = userId;
            tour.LastModifiedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // TODO: Delete associated calendar event when interface method is available
            // await _calendarEventService.DeleteEventBySourceAsync(id, nameof(Tour));

            _logger.LogInformation("Deleted tour {TourId}", id);

            return true;
        }

        /// <summary>
        /// Completes a tour with feedback and interest level.
        /// </summary>
        public async Task<bool> CompleteTourAsync(Guid tourId, string? feedback = null, string? interestLevel = null)
        {
            var userId = await GetUserIdAsync();
            var organizationId = await GetActiveOrganizationIdAsync();

            var tour = await GetByIdAsync(tourId);
            if (tour == null) return false;

            // Update tour status and feedback
            tour.Status = ApplicationConstants.TourStatuses.Completed;
            tour.Feedback = feedback;
            tour.InterestLevel = interestLevel;
            tour.ConductedBy = userId;
            tour.LastModifiedOn = DateTime.UtcNow;
            tour.LastModifiedBy = userId;

            await _context.SaveChangesAsync();

            // Update calendar event
            await _calendarEventService.CreateOrUpdateEventAsync(tour);

            // Update prospective tenant status if highly interested
            if (interestLevel == ApplicationConstants.TourInterestLevels.VeryInterested)
            {
                var prospect = await _context.ProspectiveTenants
                    .FirstOrDefaultAsync(p => p.Id == tour.ProspectiveTenantId);
                
                if (prospect != null && prospect.Status == ApplicationConstants.ProspectiveStatuses.TourScheduled)
                {
                    prospect.Status = ApplicationConstants.ProspectiveStatuses.Applied;
                    prospect.LastModifiedOn = DateTime.UtcNow;
                    prospect.LastModifiedBy = userId;
                    await _context.SaveChangesAsync();
                }
            }

            _logger.LogInformation("Completed tour {TourId} with interest level {InterestLevel}", 
                tourId, interestLevel);

            return true;
        }

        /// <summary>
        /// Creates a checklist for a tour from a template.
        /// </summary>
        private async Task CreateTourChecklistAsync(Tour tour, ProspectiveTenant? prospective, Guid? templateId)
        {
            var organizationId = await GetActiveOrganizationIdAsync();

            // Find the specified template, or fall back to default "Property Tour" template
            ChecklistTemplate? tourTemplate = null;

            if (templateId.HasValue)
            {
                tourTemplate = await _context.ChecklistTemplates
                    .FirstOrDefaultAsync(t => t.Id == templateId.Value &&
                        (t.OrganizationId == organizationId || t.IsSystemTemplate) &&
                        !t.IsDeleted);
            }

            // Fall back to default "Property Tour" template if not specified or not found
            if (tourTemplate == null)
            {
                tourTemplate = await _context.ChecklistTemplates
                    .FirstOrDefaultAsync(t => t.Name == "Property Tour" &&
                        (t.OrganizationId == organizationId || t.IsSystemTemplate) &&
                        !t.IsDeleted);
            }

            if (tourTemplate != null && prospective != null)
            {
                // Create checklist from template
                var checklist = await _checklistService.CreateChecklistFromTemplateAsync(tourTemplate.Id);

                // Customize checklist with prospect information
                checklist.Name = $"Property Tour - {prospective.FullName}";
                checklist.PropertyId = tour.PropertyId;
                checklist.GeneralNotes = $"Prospect: {prospective.FullName}\n" +
                                        $"Email: {prospective.Email}\n" +
                                        $"Phone: {prospective.Phone}\n" +
                                        $"Scheduled: {tour.ScheduledOn:MMM dd, yyyy h:mm tt}";

                // Link tour to checklist
                tour.ChecklistId = checklist.Id;
            }
        }

        /// <summary>
        /// Marks a tour as no-show and updates the associated calendar event.
        /// </summary>
        public async Task<bool> MarkTourAsNoShowAsync(Guid tourId)
        {
            try
            {
                var userId = await GetUserIdAsync();
                var organizationId = await GetActiveOrganizationIdAsync();
                
                var tour = await GetByIdAsync(tourId);
                if (tour == null) return false;

                if (tour.OrganizationId != organizationId)
                {
                    throw new UnauthorizedAccessException("User is not authorized to update this tour.");
                }

                // Update tour status to NoShow
                tour.Status = "NoShow";
                tour.LastModifiedOn = DateTime.UtcNow;
                tour.LastModifiedBy = userId;

                // Update calendar event status
                if (tour.CalendarEventId.HasValue)
                {
                    var calendarEvent = await _context.CalendarEvents
                        .FirstOrDefaultAsync(e => e.Id == tour.CalendarEventId.Value);
                    if (calendarEvent != null)
                    {
                        calendarEvent.Status = "NoShow";
                        calendarEvent.LastModifiedBy = userId;
                        calendarEvent.LastModifiedOn = DateTime.UtcNow;
                    }
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Tour {TourId} marked as no-show by user {UserId}", tourId, userId);
                return true;
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, "MarkTourAsNoShow");
                throw;
            }
        }

        /// <summary>
        /// Cancels a tour and updates related prospect status.
        /// </summary>
        public async Task<bool> CancelTourAsync(Guid tourId)
        {
            try
            {
                var userId = await GetUserIdAsync();
                var organizationId = await GetActiveOrganizationIdAsync();
                var tour = await GetByIdAsync(tourId);

                if (tour == null)
                {
                    throw new InvalidOperationException("Tour not found.");
                }

                if (tour.OrganizationId != organizationId)
                {
                    throw new UnauthorizedAccessException("Unauthorized access to tour.");
                }

                // Update tour status to cancelled
                tour.Status = ApplicationConstants.TourStatuses.Cancelled;
                tour.LastModifiedOn = DateTime.UtcNow;
                tour.LastModifiedBy = userId;
                await _context.SaveChangesAsync();

                // Update calendar event status
                await _calendarEventService.CreateOrUpdateEventAsync(tour);

                // Check if prospect has any other scheduled tours
                var prospective = await _context.ProspectiveTenants.FindAsync(tour.ProspectiveTenantId);
                if (prospective != null && prospective.Status == ApplicationConstants.ProspectiveStatuses.TourScheduled)
                {
                    var hasOtherScheduledTours = await _context.Tours
                        .AnyAsync(s => s.ProspectiveTenantId == tour.ProspectiveTenantId
                            && s.Id != tourId
                            && !s.IsDeleted
                            && s.Status == ApplicationConstants.TourStatuses.Scheduled);

                    // If no other scheduled tours, revert prospect status to Lead
                    if (!hasOtherScheduledTours)
                    {
                        prospective.Status = ApplicationConstants.ProspectiveStatuses.Lead;
                        prospective.LastModifiedOn = DateTime.UtcNow;
                        prospective.LastModifiedBy = userId;
                        await _context.SaveChangesAsync();
                    }
                }

                _logger.LogInformation("Tour {TourId} cancelled by user {UserId}", tourId, userId);
                return true;
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, "CancelTour");
                throw;
            }
        }

        /// <summary>
        /// Gets upcoming tours within specified number of days.
        /// </summary>
        public async Task<List<Tour>> GetUpcomingToursAsync(int days = 7)
        {
            try
            {
                var organizationId = await GetActiveOrganizationIdAsync();
                var startDate = DateTime.UtcNow;
                var endDate = startDate.AddDays(days);

                return await _context.Tours
                    .Where(s => s.OrganizationId == organizationId
                        && !s.IsDeleted
                        && s.Status == ApplicationConstants.TourStatuses.Scheduled
                        && s.ScheduledOn >= startDate
                        && s.ScheduledOn <= endDate)
                    .Include(s => s.ProspectiveTenant)
                    .Include(s => s.Property)
                    .Include(s => s.Checklist)
                    .OrderBy(s => s.ScheduledOn)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, "GetUpcomingTours");
                throw;
            }
        }
    }
}
