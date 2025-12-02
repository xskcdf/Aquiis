using Aquiis.SimpleStart.Core.Entities;
using Aquiis.SimpleStart.Core.Entities;
using Aquiis.SimpleStart.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Aquiis.SimpleStart.Application.Services
{
    /// <summary>
    /// Handles conversion of ProspectiveTenant to Tenant during lease signing workflow
    /// </summary>
    public class TenantConversionService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TenantConversionService> _logger;

        public TenantConversionService(
            ApplicationDbContext context,
            ILogger<TenantConversionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Converts a ProspectiveTenant to a Tenant, maintaining audit trail
        /// </summary>
        /// <param name="prospectiveTenantId">ID of the prospective tenant to convert</param>
        /// <param name="userId">User performing the conversion</param>
        /// <returns>The newly created Tenant, or existing Tenant if already converted</returns>
        public async Task<Tenant?> ConvertProspectToTenantAsync(int prospectiveTenantId, string userId)
        {
            try
            {
                // Check if this prospect has already been converted
                var existingTenant = await _context.Tenants
                    .FirstOrDefaultAsync(t => t.ProspectiveTenantId == prospectiveTenantId && !t.IsDeleted);

                if (existingTenant != null)
                {
                    _logger.LogInformation("ProspectiveTenant {ProspectId} already converted to Tenant {TenantId}", 
                        prospectiveTenantId, existingTenant.Id);
                    return existingTenant;
                }

                // Load the prospective tenant
                var prospect = await _context.ProspectiveTenants
                    .FirstOrDefaultAsync(p => p.Id == prospectiveTenantId && !p.IsDeleted);

                if (prospect == null)
                {
                    _logger.LogWarning("ProspectiveTenant {ProspectId} not found", prospectiveTenantId);
                    return null;
                }

                // Create new tenant from prospect data
                var tenant = new Tenant
                {
                    OrganizationId = prospect.OrganizationId,
                    UserId = userId,
                    FirstName = prospect.FirstName,
                    LastName = prospect.LastName,
                    Email = prospect.Email,
                    PhoneNumber = prospect.Phone,
                    DateOfBirth = prospect.DateOfBirth,
                    IdentificationNumber = prospect.IdentificationNumber ?? string.Empty,
                    IsActive = true,
                    Notes = prospect.Notes ?? string.Empty,
                    ProspectiveTenantId = prospectiveTenantId, // Maintain audit trail
                    CreatedBy = userId,
                    CreatedOn = DateTime.UtcNow
                };

                _context.Tenants.Add(tenant);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully converted ProspectiveTenant {ProspectId} to Tenant {TenantId}", 
                    prospectiveTenantId, tenant.Id);

                return tenant;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error converting ProspectiveTenant {ProspectId} to Tenant", prospectiveTenantId);
                throw;
            }
        }

        /// <summary>
        /// Gets tenant by ProspectiveTenantId, or null if not yet converted
        /// </summary>
        public async Task<Tenant?> GetTenantByProspectIdAsync(int prospectiveTenantId)
        {
            return await _context.Tenants
                .FirstOrDefaultAsync(t => t.ProspectiveTenantId == prospectiveTenantId && !t.IsDeleted);
        }

        /// <summary>
        /// Checks if a prospect has already been converted to a tenant
        /// </summary>
        public async Task<bool> IsProspectAlreadyConvertedAsync(int prospectiveTenantId)
        {
            return await _context.Tenants
                .AnyAsync(t => t.ProspectiveTenantId == prospectiveTenantId && !t.IsDeleted);
        }

        /// <summary>
        /// Gets the ProspectiveTenant history for a given Tenant
        /// </summary>
        public async Task<ProspectiveTenant?> GetProspectHistoryForTenantAsync(int tenantId)
        {
            var tenant = await _context.Tenants
                .FirstOrDefaultAsync(t => t.Id == tenantId && !t.IsDeleted);

            if (tenant?.ProspectiveTenantId == null)
                return null;

            return await _context.ProspectiveTenants
                .Include(p => p.InterestedProperty)
                .Include(p => p.Application)
                .Include(p => p.Tours)
                .FirstOrDefaultAsync(p => p.Id == tenant.ProspectiveTenantId.Value);
        }
    }
}
