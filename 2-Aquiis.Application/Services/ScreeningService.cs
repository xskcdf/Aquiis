using Aquiis.Core.Interfaces.Services;
using System.ComponentModel.DataAnnotations;
using Aquiis.Core.Constants;
using Aquiis.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Aquiis.Application.Services;
using Microsoft.Extensions.Logging;

namespace Aquiis.Application.Services
{
    /// <summary>
    /// Service for managing ApplicationScreening entities.
    /// Inherits common CRUD operations from BaseService and adds screening-specific business logic.
    /// </summary>
    public class ScreeningService : BaseService<ApplicationScreening>
    {
        public ScreeningService(
            ApplicationDbContext context,
            ILogger<ScreeningService> logger,
            IUserContextService userContext,
            IOptions<ApplicationSettings> settings)
            : base(context, logger, userContext, settings)
        {
        }

        #region Overrides with Screening-Specific Logic

        /// <summary>
        /// Validates an application screening entity before create/update operations.
        /// </summary>
        protected override async Task ValidateEntityAsync(ApplicationScreening entity)
        {
            var errors = new List<string>();

            // Required field validation
            if (entity.RentalApplicationId == Guid.Empty)
            {
                errors.Add("RentalApplicationId is required");
            }

            if (errors.Any())
            {
                throw new ValidationException(string.Join("; ", errors));
            }

            await base.ValidateEntityAsync(entity);
        }

        /// <summary>
        /// Sets default values for create operations.
        /// </summary>
        protected override async Task<ApplicationScreening> SetCreateDefaultsAsync(ApplicationScreening entity)
        {
            entity = await base.SetCreateDefaultsAsync(entity);
            
            // Set default overall result if not already set
            if (string.IsNullOrWhiteSpace(entity.OverallResult))
            {
                entity.OverallResult = ApplicationConstants.ScreeningResults.Pending;
            }

            return entity;
        }

        /// <summary>
        /// Post-create hook to update related application and prospective tenant status.
        /// </summary>
        protected override async Task AfterCreateAsync(ApplicationScreening entity)
        {
            await base.AfterCreateAsync(entity);

            // Update application and prospective tenant status
            var application = await _context.RentalApplications.FindAsync(entity.RentalApplicationId);
            if (application != null)
            {
                application.Status = ApplicationConstants.ApplicationStatuses.Screening;
                application.LastModifiedOn = DateTime.UtcNow;

                var prospective = await _context.ProspectiveTenants.FindAsync(application.ProspectiveTenantId);
                if (prospective != null)
                {
                    prospective.Status = ApplicationConstants.ProspectiveStatuses.Screening;
                    prospective.LastModifiedOn = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
            }
        }

        #endregion

        #region Retrieval Methods

        /// <summary>
        /// Gets a screening with related rental application.
        /// </summary>
        public async Task<ApplicationScreening?> GetScreeningWithRelationsAsync(Guid screeningId)
        {
            try
            {
                var organizationId = await _userContext.GetActiveOrganizationIdAsync();

                return await _context.ApplicationScreenings
                    .Include(asc => asc.RentalApplication)
                        .ThenInclude(ra => ra!.ProspectiveTenant)
                    .Include(asc => asc.RentalApplication)
                        .ThenInclude(ra => ra!.Property)
                    .FirstOrDefaultAsync(asc => asc.Id == screeningId
                        && !asc.IsDeleted
                        && asc.OrganizationId == organizationId);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, "GetScreeningWithRelations");
                throw;
            }
        }

        #endregion

        #region Business Logic Methods

        /// <summary>
        /// Gets screening by rental application ID.
        /// </summary>
        public async Task<ApplicationScreening?> GetScreeningByApplicationIdAsync(Guid rentalApplicationId)
        {
            try
            {
                var organizationId = await _userContext.GetActiveOrganizationIdAsync();

                return await _context.ApplicationScreenings
                    .Include(asc => asc.RentalApplication)
                    .FirstOrDefaultAsync(asc => asc.RentalApplicationId == rentalApplicationId
                        && !asc.IsDeleted
                        && asc.OrganizationId == organizationId);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, "GetScreeningByApplicationId");
                throw;
            }
        }

        /// <summary>
        /// Gets screenings by result status.
        /// </summary>
        public async Task<List<ApplicationScreening>> GetScreeningsByResultAsync(string result)
        {
            try
            {
                var organizationId = await _userContext.GetActiveOrganizationIdAsync();

                return await _context.ApplicationScreenings
                    .Include(asc => asc.RentalApplication)
                        .ThenInclude(ra => ra!.ProspectiveTenant)
                    .Where(asc => asc.OverallResult == result
                        && !asc.IsDeleted
                        && asc.OrganizationId == organizationId)
                    .OrderByDescending(asc => asc.CreatedOn)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, "GetScreeningsByResult");
                throw;
            }
        }

        /// <summary>
        /// Updates screening result and automatically updates application status.
        /// </summary>
        public async Task<ApplicationScreening> UpdateScreeningResultAsync(Guid screeningId, string result, string? notes = null)
        {
            try
            {
                var screening = await GetByIdAsync(screeningId);
                if (screening == null)
                {
                    throw new InvalidOperationException($"Screening {screeningId} not found");
                }

                screening.OverallResult = result;
                if (!string.IsNullOrWhiteSpace(notes))
                {
                    screening.ResultNotes = notes;
                }

                await UpdateAsync(screening);

                // Update application status based on screening result
                var application = await _context.RentalApplications.FindAsync(screening.RentalApplicationId);
                if (application != null)
                {
                    if (result == ApplicationConstants.ScreeningResults.Passed || result == ApplicationConstants.ScreeningResults.ConditionalPass)
                    {
                        application.Status = ApplicationConstants.ApplicationStatuses.Approved;
                    }
                    else if (result == ApplicationConstants.ScreeningResults.Failed)
                    {
                        application.Status = ApplicationConstants.ApplicationStatuses.Denied;
                    }
                    
                    application.LastModifiedOn = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    // Update prospective tenant status
                    var prospective = await _context.ProspectiveTenants.FindAsync(application.ProspectiveTenantId);
                    if (prospective != null)
                    {
                        if (result == ApplicationConstants.ScreeningResults.Passed || result == ApplicationConstants.ScreeningResults.ConditionalPass)
                        {
                            prospective.Status = ApplicationConstants.ProspectiveStatuses.Approved;
                        }
                        else if (result == ApplicationConstants.ScreeningResults.Failed)
                        {
                            prospective.Status = ApplicationConstants.ProspectiveStatuses.Denied;
                        }
                        
                        prospective.LastModifiedOn = DateTime.UtcNow;
                        await _context.SaveChangesAsync();
                    }
                }

                return screening;
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, "UpdateScreeningResult");
                throw;
            }
        }

        #endregion
    }
}
