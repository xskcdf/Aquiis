using Aquiis.Core.Interfaces.Services;
using System.ComponentModel.DataAnnotations;
using Aquiis.Core.Constants;
using Aquiis.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Aquiis.Application.Services;

namespace Aquiis.Application.Services
{
    /// <summary>
    /// Service for managing ProspectiveTenant entities.
    /// Inherits common CRUD operations from BaseService and adds prospective tenant-specific business logic.
    /// </summary>
    public class ProspectiveTenantService : BaseService<ProspectiveTenant>
    {
        public ProspectiveTenantService(
            ApplicationDbContext context,
            ILogger<ProspectiveTenantService> logger,
            IUserContextService userContext,
            IOptions<ApplicationSettings> settings)
            : base(context, logger, userContext, settings)
        {
        }

        #region Overrides with ProspectiveTenant-Specific Logic

        /// <summary>
        /// Validates a prospective tenant entity before create/update operations.
        /// </summary>
        protected override async Task ValidateEntityAsync(ProspectiveTenant entity)
        {
            var errors = new List<string>();

            // Required field validation
            if (string.IsNullOrWhiteSpace(entity.FirstName))
            {
                errors.Add("FirstName is required");
            }

            if (string.IsNullOrWhiteSpace(entity.LastName))
            {
                errors.Add("LastName is required");
            }

            if (string.IsNullOrWhiteSpace(entity.Email) && string.IsNullOrWhiteSpace(entity.Phone))
            {
                errors.Add("Either Email or Phone is required");
            }

            // Email format validation
            if (!string.IsNullOrWhiteSpace(entity.Email) && !entity.Email.Contains("@"))
            {
                errors.Add("Email must be a valid email address");
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
        protected override async Task<ProspectiveTenant> SetCreateDefaultsAsync(ProspectiveTenant entity)
        {
            entity = await base.SetCreateDefaultsAsync(entity);
            
            // Set default status if not already set
            if (string.IsNullOrWhiteSpace(entity.Status))
            {
                entity.Status = ApplicationConstants.ProspectiveStatuses.Lead;
            }
            
            // Set first contacted date if not already set
            if (entity.FirstContactedOn == DateTime.MinValue)
            {
                entity.FirstContactedOn = DateTime.UtcNow;
            }

            return entity;
        }

        #endregion

        #region Retrieval Methods

        /// <summary>
        /// Gets a prospective tenant with all related entities.
        /// </summary>
        public async Task<ProspectiveTenant?> GetProspectiveTenantWithRelationsAsync(Guid prospectiveTenantId)
        {
            try
            {
                var organizationId = await _userContext.GetActiveOrganizationIdAsync();

                return await _context.ProspectiveTenants
                    .Include(pt => pt.InterestedProperty)
                    .Include(pt => pt.Tours)
                    .Include(pt => pt.Applications)
                    .FirstOrDefaultAsync(pt => pt.Id == prospectiveTenantId
                        && !pt.IsDeleted
                        && pt.OrganizationId == organizationId);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, "GetProspectiveTenantWithRelations");
                throw;
            }
        }

        /// <summary>
        /// Gets all prospective tenants with related entities.
        /// </summary>
        public async Task<List<ProspectiveTenant>> GetProspectiveTenantsWithRelationsAsync()
        {
            try
            {
                var organizationId = await _userContext.GetActiveOrganizationIdAsync();

                return await _context.ProspectiveTenants
                    .Include(pt => pt.InterestedProperty)
                    .Include(pt => pt.Tours)
                    .Include(pt => pt.Applications)
                    .Where(pt => !pt.IsDeleted && pt.OrganizationId == organizationId)
                    .OrderByDescending(pt => pt.CreatedOn)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, "GetProspectiveTenantsWithRelations");
                throw;
            }
        }

        #endregion

        #region Business Logic Methods

        /// <summary>
        /// Gets prospective tenants by status.
        /// </summary>
        public async Task<List<ProspectiveTenant>> GetProspectivesByStatusAsync(string status)
        {
            try
            {
                var organizationId = await _userContext.GetActiveOrganizationIdAsync();

                return await _context.ProspectiveTenants
                    .Where(pt => pt.Status == status 
                        && !pt.IsDeleted 
                        && pt.OrganizationId == organizationId)
                    .Include(pt => pt.InterestedProperty)
                    .OrderByDescending(pt => pt.CreatedOn)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, "GetProspectivesByStatus");
                throw;
            }
        }

        /// <summary>
        /// Gets prospective tenants interested in a specific property.
        /// </summary>
        public async Task<List<ProspectiveTenant>> GetProspectivesByPropertyIdAsync(Guid propertyId)
        {
            try
            {
                var organizationId = await _userContext.GetActiveOrganizationIdAsync();

                return await _context.ProspectiveTenants
                    .Where(pt => pt.InterestedPropertyId == propertyId
                        && !pt.IsDeleted
                        && pt.OrganizationId == organizationId)
                    .Include(pt => pt.InterestedProperty)
                    .Include(pt => pt.Tours)
                    .OrderByDescending(pt => pt.CreatedOn)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, "GetProspectivesByPropertyId");
                throw;
            }
        }

        /// <summary>
        /// Updates a prospective tenant's status.
        /// </summary>
        public async Task<ProspectiveTenant> UpdateStatusAsync(Guid prospectiveTenantId, string newStatus)
        {
            try
            {
                var prospect = await GetByIdAsync(prospectiveTenantId);
                if (prospect == null)
                {
                    throw new InvalidOperationException($"Prospective tenant {prospectiveTenantId} not found");
                }

                prospect.Status = newStatus;
                return await UpdateAsync(prospect);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, "UpdateStatus");
                throw;
            }
        }

        #endregion
    }
}
