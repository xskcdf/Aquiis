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
    /// Service for managing LeaseOffer entities.
    /// Inherits common CRUD operations from BaseService and adds lease offer-specific business logic.
    /// </summary>
    public class LeaseOfferService : BaseService<LeaseOffer>
    {
        public LeaseOfferService(
            ApplicationDbContext context,
            ILogger<LeaseOfferService> logger,
            IUserContextService userContext,
            IOptions<ApplicationSettings> settings)
            : base(context, logger, userContext, settings)
        {
        }

        #region Overrides with LeaseOffer-Specific Logic

        /// <summary>
        /// Validates a lease offer entity before create/update operations.
        /// </summary>
        protected override async Task ValidateEntityAsync(LeaseOffer entity)
        {
            var errors = new List<string>();

            // Required field validation
            if (entity.RentalApplicationId == Guid.Empty)
            {
                errors.Add("RentalApplicationId is required");
            }

            if (entity.PropertyId == Guid.Empty)
            {
                errors.Add("PropertyId is required");
            }

            if (entity.ProspectiveTenantId == Guid.Empty)
            {
                errors.Add("ProspectiveTenantId is required");
            }

            if (entity.MonthlyRent <= 0)
            {
                errors.Add("MonthlyRent must be greater than zero");
            }

            if (entity.SecurityDeposit < 0)
            {
                errors.Add("SecurityDeposit cannot be negative");
            }

            if (entity.OfferedOn == DateTime.MinValue)
            {
                errors.Add("OfferedOn is required");
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
        protected override async Task<LeaseOffer> SetCreateDefaultsAsync(LeaseOffer entity)
        {
            entity = await base.SetCreateDefaultsAsync(entity);
            
            // Set default status if not already set
            if (string.IsNullOrWhiteSpace(entity.Status))
            {
                entity.Status = "Pending";
            }
            
            // Set offered date if not already set
            if (entity.OfferedOn == DateTime.MinValue)
            {
                entity.OfferedOn = DateTime.UtcNow;
            }

            // Set expiration date if not already set (default 7 days)
            if (entity.ExpiresOn == DateTime.MinValue)
            {
                entity.ExpiresOn = entity.OfferedOn.AddDays(7);
            }

            return entity;
        }

        #endregion

        #region Retrieval Methods

        /// <summary>
        /// Gets a lease offer with all related entities.
        /// </summary>
        public async Task<LeaseOffer?> GetLeaseOfferWithRelationsAsync(Guid leaseOfferId)
        {
            try
            {
                var organizationId = await _userContext.GetActiveOrganizationIdAsync();

                return await _context.LeaseOffers
                    .Include(lo => lo.RentalApplication)
                    .Include(lo => lo.Property)
                    .Include(lo => lo.ProspectiveTenant)
                    .FirstOrDefaultAsync(lo => lo.Id == leaseOfferId
                        && !lo.IsDeleted
                        && lo.OrganizationId == organizationId);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, "GetLeaseOfferWithRelations");
                throw;
            }
        }

        /// <summary>
        /// Gets all lease offers with related entities.
        /// </summary>
        public async Task<List<LeaseOffer>> GetLeaseOffersWithRelationsAsync()
        {
            try
            {
                var organizationId = await _userContext.GetActiveOrganizationIdAsync();

                return await _context.LeaseOffers
                    .Include(lo => lo.RentalApplication)
                    .Include(lo => lo.Property)
                    .Include(lo => lo.ProspectiveTenant)
                    .Where(lo => !lo.IsDeleted && lo.OrganizationId == organizationId)
                    .OrderByDescending(lo => lo.OfferedOn)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, "GetLeaseOffersWithRelations");
                throw;
            }
        }

        #endregion

        #region Business Logic Methods

        /// <summary>
        /// Gets lease offer by rental application ID.
        /// </summary>
        public async Task<LeaseOffer?> GetLeaseOfferByApplicationIdAsync(Guid applicationId)
        {
            try
            {
                var organizationId = await _userContext.GetActiveOrganizationIdAsync();

                return await _context.LeaseOffers
                    .Include(lo => lo.RentalApplication)
                    .Include(lo => lo.Property)
                    .Include(lo => lo.ProspectiveTenant)
                    .FirstOrDefaultAsync(lo => lo.RentalApplicationId == applicationId
                        && !lo.IsDeleted
                        && lo.OrganizationId == organizationId);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, "GetLeaseOfferByApplicationId");
                throw;
            }
        }

        /// <summary>
        /// Gets lease offers by property ID.
        /// </summary>
        public async Task<List<LeaseOffer>> GetLeaseOffersByPropertyIdAsync(Guid propertyId)
        {
            try
            {
                var organizationId = await _userContext.GetActiveOrganizationIdAsync();

                return await _context.LeaseOffers
                    .Include(lo => lo.RentalApplication)
                    .Include(lo => lo.Property)
                    .Include(lo => lo.ProspectiveTenant)
                    .Where(lo => lo.PropertyId == propertyId
                        && !lo.IsDeleted
                        && lo.OrganizationId == organizationId)
                    .OrderByDescending(lo => lo.OfferedOn)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, "GetLeaseOffersByPropertyId");
                throw;
            }
        }

        /// <summary>
        /// Gets lease offers by status.
        /// </summary>
        public async Task<List<LeaseOffer>> GetLeaseOffersByStatusAsync(string status)
        {
            try
            {
                var organizationId = await _userContext.GetActiveOrganizationIdAsync();

                return await _context.LeaseOffers
                    .Include(lo => lo.RentalApplication)
                    .Include(lo => lo.Property)
                    .Include(lo => lo.ProspectiveTenant)
                    .Where(lo => lo.Status == status
                        && !lo.IsDeleted
                        && lo.OrganizationId == organizationId)
                    .OrderByDescending(lo => lo.OfferedOn)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, "GetLeaseOffersByStatus");
                throw;
            }
        }

        /// <summary>
        /// Gets active (pending) lease offers.
        /// </summary>
        public async Task<List<LeaseOffer>> GetActiveLeaseOffersAsync()
        {
            try
            {
                var organizationId = await _userContext.GetActiveOrganizationIdAsync();

                return await _context.LeaseOffers
                    .Include(lo => lo.RentalApplication)
                    .Include(lo => lo.Property)
                    .Include(lo => lo.ProspectiveTenant)
                    .Where(lo => lo.Status == "Pending"
                        && !lo.IsDeleted
                        && lo.OrganizationId == organizationId
                        && lo.ExpiresOn > DateTime.UtcNow)
                    .OrderByDescending(lo => lo.OfferedOn)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, "GetActiveLeaseOffers");
                throw;
            }
        }

        /// <summary>
        /// Updates lease offer status.
        /// </summary>
        public async Task<LeaseOffer> UpdateLeaseOfferStatusAsync(Guid leaseOfferId, string newStatus, string? responseNotes = null)
        {
            try
            {
                var leaseOffer = await GetByIdAsync(leaseOfferId);
                if (leaseOffer == null)
                {
                    throw new InvalidOperationException($"Lease offer {leaseOfferId} not found");
                }

                leaseOffer.Status = newStatus;
                leaseOffer.RespondedOn = DateTime.UtcNow;
                
                if (!string.IsNullOrWhiteSpace(responseNotes))
                {
                    leaseOffer.ResponseNotes = responseNotes;
                }

                return await UpdateAsync(leaseOffer);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex, "UpdateLeaseOfferStatus");
                throw;
            }
        }

        #endregion
    }
}
