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
    /// Service for managing RentalApplication entities.
    /// Inherits common CRUD operations from BaseService and adds rental application-specific business logic.
    /// </summary>
    public class RentalApplicationService : BaseService<RentalApplication>
    {
        public RentalApplicationService(
            ApplicationDbContext context,
            ILogger<RentalApplicationService> logger,
            IUserContextService userContext,
            IOptions<ApplicationSettings> settings)
            : base(context, logger, userContext, settings)
        {
        }

        #region Overrides with RentalApplication-Specific Logic

        /// <summary>
        /// Validates a rental application entity before create/update operations.
        /// </summary>
        protected override async Task ValidateEntityAsync(RentalApplication entity)
        {
            var errors = new List<string>();

            // Required field validation
            if (entity.ProspectiveTenantId == Guid.Empty)
            {
                errors.Add("ProspectiveTenantId is required");
            }

            if (entity.PropertyId == Guid.Empty)
            {
                errors.Add("PropertyId is required");
            }

            if (entity.ApplicationFee < 0)
            {
                errors.Add("ApplicationFee cannot be negative");
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
        protected override async Task<RentalApplication> SetCreateDefaultsAsync(RentalApplication entity)
        {
            entity = await base.SetCreateDefaultsAsync(entity);
            
            // Set default status if not already set
            if (string.IsNullOrWhiteSpace(entity.Status))
            {
                entity.Status = ApplicationConstants.ApplicationStatuses.Submitted;
            }
            
            // Set applied date if not already set
            if (entity.AppliedOn == DateTime.MinValue)
            {
                entity.AppliedOn = DateTime.UtcNow;
            }

            // Get organization settings for fee and expiration defaults
            var orgSettings = await _context.OrganizationSettings
                .FirstOrDefaultAsync(s => s.OrganizationId == entity.OrganizationId && !s.IsDeleted);

            if (orgSettings != null)
            {
                // Set application fee if not already set and fees are enabled
                if (orgSettings.ApplicationFeeEnabled && entity.ApplicationFee == 0)
                {
                    entity.ApplicationFee = orgSettings.DefaultApplicationFee;
                }

                // Set expiration date if not already set
                if (entity.ExpiresOn == null)
                {
                    entity.ExpiresOn = entity.AppliedOn.AddDays(orgSettings.ApplicationExpirationDays);
                }
            }
            else
            {
                // Fallback defaults if no settings found
                if (entity.ApplicationFee == 0)
                {
                    entity.ApplicationFee = 50.00m; // Default fee
                }
                if (entity.ExpiresOn == null)
                {
                    entity.ExpiresOn = entity.AppliedOn.AddDays(30); // Default 30 days
                }
            }

            return entity;
        }

        /// <summary>
        /// Post-create hook to update related entities.
        /// </summary>
        protected override async Task AfterCreateAsync(RentalApplication entity)
        {
            await base.AfterCreateAsync(entity);

            // Update property status to ApplicationPending
            var property = await _context.Properties.FindAsync(entity.PropertyId);
            if (property != null && property.Status == ApplicationConstants.PropertyStatuses.Available)
            {
                property.Status = ApplicationConstants.PropertyStatuses.ApplicationPending;
                property.LastModifiedOn = DateTime.UtcNow;
                property.LastModifiedBy = entity.CreatedBy;
                await _context.SaveChangesAsync();
            }

            // Update ProspectiveTenant status
            var prospective = await _context.ProspectiveTenants.FindAsync(entity.ProspectiveTenantId);
            if (prospective != null)
            {
                prospective.Status = ApplicationConstants.ProspectiveStatuses.Applied;
                prospective.LastModifiedOn = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        #endregion

        #region Retrieval Methods

        /// <summary>
        /// Gets a rental application with all related entities.
        /// </summary>
        public async Task<RentalApplication?> GetRentalApplicationWithRelationsAsync(Guid applicationId)
        {
            try
            {
                var organizationId = await _userContext.GetActiveOrganizationIdAsync();

                return await _context.RentalApplications
                    .Include(ra => ra.ProspectiveTenant)
                    .Include(ra => ra.Property)
                    .Include(ra => ra.Screening)
                    .FirstOrDefaultAsync(ra => ra.Id == applicationId
                        && !ra.IsDeleted
                        && ra.OrganizationId == organizationId);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, "GetRentalApplicationWithRelations");
                throw;
            }
        }

        /// <summary>
        /// Gets all rental applications with related entities.
        /// </summary>
        public async Task<List<RentalApplication>> GetRentalApplicationsWithRelationsAsync()
        {
            try
            {
                var organizationId = await _userContext.GetActiveOrganizationIdAsync();

                return await _context.RentalApplications
                    .Include(ra => ra.ProspectiveTenant)
                    .Include(ra => ra.Property)
                    .Include(ra => ra.Screening)
                    .Where(ra => !ra.IsDeleted && ra.OrganizationId == organizationId)
                    .OrderByDescending(ra => ra.AppliedOn)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, "GetRentalApplicationsWithRelations");
                throw;
            }
        }

        #endregion

        #region Business Logic Methods

        /// <summary>
        /// Gets rental application by prospective tenant ID.
        /// </summary>
        public async Task<RentalApplication?> GetApplicationByProspectiveIdAsync(Guid prospectiveTenantId)
        {
            try
            {
                var organizationId = await _userContext.GetActiveOrganizationIdAsync();

                return await _context.RentalApplications
                    .Include(ra => ra.Property)
                    .Include(ra => ra.Screening)
                    .FirstOrDefaultAsync(ra => ra.ProspectiveTenantId == prospectiveTenantId
                        && !ra.IsDeleted
                        && ra.OrganizationId == organizationId);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, "GetApplicationByProspectiveId");
                throw;
            }
        }

        /// <summary>
        /// Gets pending rental applications.
        /// </summary>
        public async Task<List<RentalApplication>> GetPendingApplicationsAsync()
        {
            try
            {
                var organizationId = await _userContext.GetActiveOrganizationIdAsync();

                return await _context.RentalApplications
                    .Include(ra => ra.ProspectiveTenant)
                    .Include(ra => ra.Property)
                    .Include(ra => ra.Screening)
                    .Where(ra => !ra.IsDeleted
                        && ra.OrganizationId == organizationId
                        && (ra.Status == ApplicationConstants.ApplicationStatuses.Submitted
                            || ra.Status == ApplicationConstants.ApplicationStatuses.Screening))
                    .OrderByDescending(ra => ra.AppliedOn)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, "GetPendingApplications");
                throw;
            }
        }

        /// <summary>
        /// Gets rental applications by property ID.
        /// </summary>
        public async Task<List<RentalApplication>> GetApplicationsByPropertyIdAsync(Guid propertyId)
        {
            try
            {
                var organizationId = await _userContext.GetActiveOrganizationIdAsync();

                return await _context.RentalApplications
                    .Include(ra => ra.ProspectiveTenant)
                    .Include(ra => ra.Screening)
                    .Where(ra => ra.PropertyId == propertyId
                        && !ra.IsDeleted
                        && ra.OrganizationId == organizationId)
                    .OrderByDescending(ra => ra.AppliedOn)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, "GetApplicationsByPropertyId");
                throw;
            }
        }

        #endregion
    }
}
