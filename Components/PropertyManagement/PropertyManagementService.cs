using Aquiis.SimpleStart.Components.PropertyManagement.Properties;
using Aquiis.SimpleStart.Components.PropertyManagement.Leases;
using Aquiis.SimpleStart.Components.PropertyManagement.Tenants;
using Aquiis.SimpleStart.Components.PropertyManagement.Invoices;
using Aquiis.SimpleStart.Components.PropertyManagement.Payments;
using Aquiis.SimpleStart.Components.PropertyManagement.Documents;
using Aquiis.SimpleStart.Components.PropertyManagement.Inspections;
using Aquiis.SimpleStart.Components.PropertyManagement.MaintenanceRequests;
using Aquiis.SimpleStart.Components.Account;
using Aquiis.SimpleStart.Data;
using Aquiis.SimpleStart.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Aquiis.SimpleStart.Components.Administration.Application;
using Aquiis.SimpleStart.Services;
using System.Security.Claims;
using Microsoft.Extensions.Options;

namespace Aquiis.SimpleStart.Components.PropertyManagement
{
    public class PropertyManagementService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationSettings _applicationSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserContextService _userContext;
        private readonly CalendarEventService _calendarEventService;

        public PropertyManagementService(
            ApplicationDbContext dbContext, 
            UserManager<ApplicationUser> userManager, 
            IOptions<ApplicationSettings> settings, 
            IHttpContextAccessor httpContextAccessor,
            UserContextService userContext,
            CalendarEventService calendarEventService)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _applicationSettings = settings.Value;
            _httpContextAccessor = httpContextAccessor;
            _userContext = userContext;
            _calendarEventService = calendarEventService;


        }

        #region Properties
        public async Task<List<Property>> GetPropertiesAsync()
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            
            var organizationId = await _userContext.GetOrganizationIdAsync();
            
            return await _dbContext.Properties
                .Include(p => p.Leases)
                .Include(p => p.Documents)
                .Where(p => !p.IsDeleted && p.OrganizationId == organizationId)
                .ToListAsync();
        }

        public async Task<Property?> GetPropertyByIdAsync(int propertyId)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            
            var organizationId = await _userContext.GetOrganizationIdAsync();

            return await _dbContext.Properties
            .Include(p => p.Leases)
            .Include(p => p.Documents)
            .FirstOrDefaultAsync(p => p.Id == propertyId && p.OrganizationId == organizationId && !p.IsDeleted);
        }

        public async Task<List<Property>> GetPropertiesByOrganizationIdAsync(string organizationId)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            
            // In a real application, this would call a database or API
            List<Property> properties = await _dbContext.Properties
                .Include(p => p.Leases)
                .Include(p => p.Documents)
                .Where(p => p.OrganizationId == organizationId && !p.IsDeleted)
                .ToListAsync();
            return properties;
        }

        public async Task AddPropertyAsync(Property property)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            property.OrganizationId = organizationId!;

            // Set initial routine inspection due date to 30 days from creation
            property.NextRoutineInspectionDueDate = DateTime.Today.AddDays(30);

            await _dbContext.Properties.AddAsync(property);
            await _dbContext.SaveChangesAsync();

            // Create calendar event for the first routine inspection
            await CreateRoutineInspectionCalendarEventAsync(property);
        }

        public async Task UpdatePropertyAsync(Property property)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            if (property.OrganizationId != organizationId)
            {
                throw new UnauthorizedAccessException("You do not have permission to update this property.");
            }

            _dbContext.Properties.Update(property);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeletePropertyAsync(int propertyId)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            
            if (_applicationSettings.SoftDeleteEnabled)
            {
                await SoftDeletePropertyAsync(propertyId);
                return;
            }
            else
            {
                var property = await _dbContext.Properties
                    .FirstOrDefaultAsync(p => p.Id == propertyId &&
                        p.OrganizationId == organizationId);

                if (property != null)
                {
                    _dbContext.Properties.Remove(property);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        private async Task SoftDeletePropertyAsync(int propertyId)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            var property = await _dbContext.Properties
                .FirstOrDefaultAsync(p => p.Id == propertyId && p.OrganizationId == organizationId);
                
            if (property != null && !property.IsDeleted)
            {
                property.IsDeleted = true;
                property.LastModifiedOn = DateTime.UtcNow;
                property.LastModifiedBy = _userId;
                _dbContext.Properties.Update(property);
                await _dbContext.SaveChangesAsync();

                var leases = await GetLeasesByPropertyIdAsync(propertyId);
                foreach (var lease in leases)
                {
                    lease.Status = ApplicationConstants.LeaseStatuses.Terminated;
                    lease.LastModifiedOn = DateTime.UtcNow;
                    lease.LastModifiedBy = _userId;
                    await UpdateLeaseAsync(lease);

                    var tenants = await GetTenantsByLeaseIdAsync(lease.Id);
                    foreach (var tenant in tenants)
                    {
                        var tenantLeases = await GetLeasesByTenantIdAsync(tenant.Id);
                        tenantLeases = tenantLeases.Where(l => l.PropertyId != propertyId && !l.IsDeleted).ToList();

                        if(tenantLeases.Count == 0) // Only this lease
                        {
                            tenant.IsActive = false;
                            tenant.LastModifiedBy = _userId;
                            tenant.LastModifiedOn = DateTime.UtcNow;
                            await UpdateTenantAsync(tenant);
                        }
                    }

                }

            }
        }
        #endregion

        #region Tenants

        public async Task<List<Tenant>> GetTenantsAsync()
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            
            var organizationId = await _userContext.GetOrganizationIdAsync();
            
            return await _dbContext.Tenants
                .Include(t => t.Leases)
                .Where(t => !t.IsDeleted && t.OrganizationId == organizationId)
                .ToListAsync();
        }
        
        public async Task<List<Tenant>> GetTenantsByLeaseIdAsync(int leaseId)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            var leases = await _dbContext.Leases
                .Include(l => l.Tenant)
                .Where(l => l.Id == leaseId && l.Tenant.OrganizationId == organizationId && !l.IsDeleted && !l.Tenant.IsDeleted)
                .ToListAsync();

            var tenantIds = leases.Select(l => l.TenantId).Distinct().ToList();
            
            return await _dbContext.Tenants
                .Where(t => tenantIds.Contains(t.Id) && t.OrganizationId == organizationId && !t.IsDeleted)
                .ToListAsync();
        }
        public async Task<List<Tenant>> GetTenantsByPropertyIdAsync(int propertyId)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            var leases = await _dbContext.Leases
                .Include(l => l.Tenant)
                .Where(l => l.PropertyId == propertyId && l.Tenant.OrganizationId == organizationId && !l.IsDeleted && !l.Tenant.IsDeleted)
                .ToListAsync();

            var tenantIds = leases.Select(l => l.TenantId).Distinct().ToList();
            
            return await _dbContext.Tenants
                .Where(t => tenantIds.Contains(t.Id) && t.OrganizationId == organizationId && !t.IsDeleted)
                .ToListAsync();
        }

        public async Task<Tenant?> GetTenantByIdAsync(int tenantId)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            return await _dbContext.Tenants
                .Include(t => t.Leases)
                .FirstOrDefaultAsync(t => t.Id == tenantId && t.OrganizationId == organizationId && !t.IsDeleted);
        }

        public async Task<Tenant?> GetTenantByIdentificationNumberAsync(string identificationNumber)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            return await _dbContext.Tenants
                .Include(t => t.Leases)
                .FirstOrDefaultAsync(t => t.IdentificationNumber == identificationNumber && t.OrganizationId == organizationId && !t.IsDeleted);
        }

        public async Task<List<Tenant>> GetTenantsByOrganizationIdAsync(string organizationId)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            return await _dbContext.Tenants
                .Include(t => t.Leases)
                .Where(t => t.OrganizationId == organizationId && !t.IsDeleted)
                .ToListAsync();
        }

        public async Task AddTenantAsync(Tenant tenant)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            tenant.OrganizationId = organizationId!;
            await _dbContext.Tenants.AddAsync(tenant);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateTenantAsync(Tenant tenant)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            var organizationId = await _userContext.GetOrganizationIdAsync();

            if (tenant.OrganizationId != organizationId)
            {
                throw new UnauthorizedAccessException("You do not have permission to update this tenant.");
            }

            tenant.LastModifiedOn = DateTime.UtcNow;
            tenant.LastModifiedBy = _userId;

            _dbContext.Tenants.Update(tenant);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteTenantAsync(Tenant tenant)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            if (_applicationSettings.SoftDeleteEnabled)
            {
                await SoftDeleteTenantAsync(tenant, userId);
                return;
            }
            else
            {
                if (tenant != null)
                {
                    _dbContext.Tenants.Remove(tenant);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        private async Task SoftDeleteTenantAsync(Tenant tenant, string userId)
        {
            if (tenant != null && !tenant.IsDeleted && !string.IsNullOrEmpty(userId))
            {
                tenant.IsDeleted = true;
                tenant.LastModifiedOn = DateTime.UtcNow;
                tenant.LastModifiedBy = userId;
                _dbContext.Tenants.Update(tenant);
                await _dbContext.SaveChangesAsync();
            }
        }

        #endregion

        #region Leases

        public async Task<List<Lease>> GetLeasesAsync()
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            return await _dbContext.Leases
                .Include(l => l.Property)
                .Include(l => l.Tenant)
                .Where(l => !l.IsDeleted && !l.Tenant.IsDeleted && !l.Property.IsDeleted && l.Property.OrganizationId == organizationId)
                .ToListAsync();
        }
        public async Task<Lease?> GetLeaseByIdAsync(int leaseId)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            
            var organizationId = await _userContext.GetOrganizationIdAsync();

            return await _dbContext.Leases
                .Include(l => l.Property)
                .Include(l => l.Tenant)
                .FirstOrDefaultAsync(l => l.Id == leaseId && !l.IsDeleted && !l.Tenant.IsDeleted && !l.Property.IsDeleted && l.Property.OrganizationId == organizationId);
        }

        public async Task<List<Lease>> GetLeasesByPropertyIdAsync(int propertyId)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            
            var organizationId = await _userContext.GetOrganizationIdAsync();

            var leases = await _dbContext.Leases
            .Include(l => l.Property)
            .Include(l => l.Tenant)
            .Where(l => l.PropertyId == propertyId && !l.IsDeleted && l.Property.OrganizationId == organizationId)
            .ToListAsync();
            
            return leases
                .Where(l => l.IsActive)
                .ToList();
        }

        public async Task<List<Lease>> GetActiveLeasesByPropertyIdAsync(int propertyId)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            
            var organizationId = await _userContext.GetOrganizationIdAsync();

            var leases = await _dbContext.Leases
            .Include(l => l.Property)
            .Include(l => l.Tenant)
            .Where(l => l.PropertyId == propertyId && !l.IsDeleted && !l.Tenant.IsDeleted && !l.Property.IsDeleted && l.Property.OrganizationId == organizationId)
            .ToListAsync();
            
            return leases
                .Where(l => l.IsActive)
                .ToList();
        }

        public async Task<List<Lease>> GetLeasesByOrganizationIdAsync(string organizationId)
        {

            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            
            return await _dbContext.Leases
                .Include(l => l.Property)
                .Include(l => l.Tenant)
                .Where(l => !l.IsDeleted && !l.Tenant.IsDeleted && !l.Property.IsDeleted && l.Property.OrganizationId == organizationId)
                .ToListAsync();
        }

        public async Task<List<Lease>> GetLeasesByTenantIdAsync(int tenantId)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            return await _dbContext.Leases
                .Include(l => l.Property)
                .Include(l => l.Tenant)
                .Where(l => l.TenantId == tenantId && !l.Tenant.IsDeleted && !l.IsDeleted && l.Property.OrganizationId == organizationId)
                .ToListAsync();
        }

        public async Task<Lease?> AddLeaseAsync(Lease lease)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            var property = await GetPropertyByIdAsync(lease.PropertyId);
            if(property is null || property.OrganizationId != organizationId)
                return lease;

            await _dbContext.Leases.AddAsync(lease);

            property.IsAvailable = false;
            property.LastModifiedOn = DateTime.UtcNow;
            property.LastModifiedBy = _userId;

            _dbContext.Properties.Update(property);

            await _dbContext.SaveChangesAsync();

            return lease;
        }

        public async Task UpdateLeaseAsync(Lease lease)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (_userId == null)
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            
            var organizationId = await _userContext.GetOrganizationIdAsync();
            
            if (lease.Property.OrganizationId != organizationId)
            {
                throw new UnauthorizedAccessException("User does not have access to this lease.");
            }
            
            lease.LastModifiedOn = DateTime.UtcNow;
            lease.LastModifiedBy = _userId;

            _dbContext.Leases.Update(lease);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteLeaseAsync(int leaseId)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                // Handle the case when the user is not authenticated
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            if( !await _dbContext.Leases.AnyAsync(l => l.Id == leaseId && l.Property.OrganizationId == organizationId))
            {
                throw new UnauthorizedAccessException("User does not have access to this lease.");
            }

            if (_applicationSettings.SoftDeleteEnabled)
            {
                await SoftDeleteLeaseAsync(leaseId, userId);
                return;
            }
            else
            {
                var lease = await _dbContext.Leases.FirstOrDefaultAsync(l => l.Id == leaseId);
                if (lease != null)
                {
                    _dbContext.Leases.Remove(lease);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        private async Task SoftDeleteLeaseAsync(int leaseId, string userId)
        {
            var lease = await _dbContext.Leases.FirstOrDefaultAsync(l => l.Id == leaseId);
            if (lease != null && !lease.IsDeleted && !string.IsNullOrEmpty(userId))
            {
                lease.IsDeleted = true;
                lease.LastModifiedOn = DateTime.UtcNow;
                lease.LastModifiedBy = userId;
                _dbContext.Leases.Update(lease);
                await _dbContext.SaveChangesAsync();
            }
        }

    #endregion

        #region Invoices
        
        public async Task<List<Invoice>> GetInvoicesAsync()
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();
            
            return await _dbContext.Invoices
                .Include(i => i.Lease)
                    .ThenInclude(l => l.Property)
                .Include(i => i.Lease)
                    .ThenInclude(l => l.Tenant)
                .Include(i => i.Payments)
                .Where(i => !i.IsDeleted && i.Lease.Property.OrganizationId == organizationId)
                .OrderByDescending(i => i.DueOn)
                .ToListAsync();
        }

        public async Task<List<Invoice>> GetInvoicesByOrganizationIdAsync(string organizationId)
        {
            return await _dbContext.Invoices
                .Include(i => i.Lease)
                    .ThenInclude(l => l.Property)
                .Include(i => i.Lease)
                    .ThenInclude(l => l.Tenant)
                .Include(i => i.Payments)
                .Where(i => !i.IsDeleted && i.Lease.Property.OrganizationId == organizationId)
                .OrderByDescending(i => i.DueOn)
                .ToListAsync();
        }

        public async Task<Invoice?> GetInvoiceByIdAsync(int invoiceId)
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();


            return await _dbContext.Invoices
                .Include(i => i.Lease)
                    .ThenInclude(l => l.Property)
                .Include(i => i.Lease)
                    .ThenInclude(l => l.Tenant)
                .Include(i => i.Payments)
                .FirstOrDefaultAsync(i => i.Id == invoiceId
                    && !i.IsDeleted 
                    && i.Lease.Property.OrganizationId == organizationId);
        }

        public async Task<List<Invoice>> GetInvoicesByLeaseIdAsync(int leaseId)
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();

            return await _dbContext.Invoices
                .Include(i => i.Lease)
                    .ThenInclude(l => l.Property)
                .Include(i => i.Lease)
                    .ThenInclude(l => l.Tenant)
                .Include(i => i.Payments)
                .Where(i => i.LeaseId == leaseId
                    && !i.IsDeleted
                    && i.Lease.Property.OrganizationId == organizationId)
                .OrderByDescending(i => i.DueOn)
                .ToListAsync();
        }

        public async Task AddInvoiceAsync(Invoice invoice)
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();

            var lease = await _dbContext.Leases
                .Include(l => l.Property)
                .FirstOrDefaultAsync(l => l.Id == invoice.LeaseId && !l.IsDeleted);

            if (lease == null || lease.Property.OrganizationId != organizationId)
            {
                throw new UnauthorizedAccessException("User does not have access to this lease.");
            }

            await _dbContext.Invoices.AddAsync(invoice);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateInvoiceAsync(Invoice invoice)
        {
            var cuserId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (cuserId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();
            var lease = await _dbContext.Leases
                .Include(l => l.Property)
                .FirstOrDefaultAsync(l => l.Id == invoice.LeaseId && !l.IsDeleted);

            if (lease == null || lease.Property.OrganizationId != organizationId)
            {
                throw new UnauthorizedAccessException("User does not have access to this lease.");
            }

            invoice.LastModifiedOn = DateTime.UtcNow;
            invoice.LastModifiedBy = cuserId;

            _dbContext.Invoices.Update(invoice);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteInvoiceAsync(Invoice invoice)
        {
            var cuserId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (cuserId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            if (_applicationSettings.SoftDeleteEnabled)
            {
                invoice.IsDeleted = true;
                invoice.LastModifiedOn = DateTime.UtcNow;
                invoice.LastModifiedBy = cuserId;
                _dbContext.Invoices.Update(invoice);
            }
            else
            {
                _dbContext.Invoices.Remove(invoice);
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task<string> GenerateInvoiceNumberAsync()
        {
            var lastInvoice = await _dbContext.Invoices
                .OrderByDescending(i => i.Id)
                .FirstOrDefaultAsync();
            
            var nextNumber = lastInvoice != null ? lastInvoice.Id + 1 : 1;
            return $"INV-{DateTime.Now:yyyyMM}-{nextNumber:D5}";
        }

        #endregion

        #region Payments
        
        public async Task<List<Payment>> GetPaymentsAsync()
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();
            
            return await _dbContext.Payments
                .Include(p => p.Invoice)
                    .ThenInclude(i => i!.Lease)
                        .ThenInclude(l => l!.Property)
                .Include(p => p.Invoice)
                    .ThenInclude(i => i!.Lease)
                        .ThenInclude(l => l!.Tenant)
                .Where(p => !p.IsDeleted && p.Invoice.Lease.Property.OrganizationId == organizationId)
                .OrderByDescending(p => p.PaidOn)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetPaymentsByUserIdAsync(string userId)
        {
            return await _dbContext.Payments
                .Include(p => p.Invoice)
                    .ThenInclude(i => i!.Lease)
                        .ThenInclude(l => l!.Property)
                .Include(p => p.Invoice)
                    .ThenInclude(i => i!.Lease)
                        .ThenInclude(l => l!.Tenant)
                .Where(p => p.UserId == userId && !p.IsDeleted)
                .OrderByDescending(p => p.PaidOn)
                .ToListAsync();
        }

        public async Task<Payment?> GetPaymentByIdAsync(int paymentId)
        {
            return await _dbContext.Payments
                .Include(p => p.Invoice)
                    .ThenInclude(i => i!.Lease)
                        .ThenInclude(l => l!.Property)
                .Include(p => p.Invoice)
                    .ThenInclude(i => i!.Lease)
                        .ThenInclude(l => l!.Tenant)
                .FirstOrDefaultAsync(p => p.Id == paymentId && !p.IsDeleted);
        }

        public async Task<List<Payment>> GetPaymentsByInvoiceIdAsync(int invoiceId)
        {
            return await _dbContext.Payments
                .Include(p => p.Invoice)
                .Where(p => p.InvoiceId == invoiceId && !p.IsDeleted)
                .OrderByDescending(p => p.PaidOn)
                .ToListAsync();
        }

        public async Task AddPaymentAsync(Payment payment)
        {
            await _dbContext.Payments.AddAsync(payment);
            await _dbContext.SaveChangesAsync();
            
            // Update invoice paid amount
            await UpdateInvoicePaidAmountAsync(payment.InvoiceId);
        }

        public async Task UpdatePaymentAsync(Payment payment)
        {
            payment.LastModifiedOn = DateTime.UtcNow;
            _dbContext.Payments.Update(payment);
            await _dbContext.SaveChangesAsync();
            
            // Update invoice paid amount
            await UpdateInvoicePaidAmountAsync(payment.InvoiceId);
        }

        public async Task DeletePaymentAsync(Payment payment)
        {
            var cuserId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (cuserId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var invoiceId = payment.InvoiceId;

            if (_applicationSettings.SoftDeleteEnabled)
            {
                payment.IsDeleted = true;
                payment.LastModifiedOn = DateTime.UtcNow;
                payment.LastModifiedBy = cuserId;
                _dbContext.Payments.Update(payment);
            }
            else
            {
                _dbContext.Payments.Remove(payment);
            }
            await _dbContext.SaveChangesAsync();
            
            // Update invoice paid amount
            await UpdateInvoicePaidAmountAsync(invoiceId);
        }

        private async Task UpdateInvoicePaidAmountAsync(int invoiceId)
        {
            var invoice = await _dbContext.Invoices.FindAsync(invoiceId);
            if (invoice != null)
            {
                var totalPaid = await _dbContext.Payments
                    .Where(p => p.InvoiceId == invoiceId && !p.IsDeleted)
                    .SumAsync(p => p.Amount);
                
                invoice.AmountPaid = totalPaid;
                
                // Update invoice status based on payment
                if (totalPaid >= invoice.Amount)
                {
                    invoice.Status = "Paid";
                    invoice.PaidOn = DateTime.UtcNow;
                }
                else if (totalPaid > 0)
                {
                    invoice.Status = "Partial";
                }
                
                await _dbContext.SaveChangesAsync();
            }
        }

        #endregion

        #region Documents
        
        public async Task<List<Document>> GetDocumentsAsync()
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();
            
            return await _dbContext.Documents
                .Include(d => d.Property)
                .Include(d => d.Tenant)
                .Include(d => d.Lease)
                    .ThenInclude(l => l!.Property)
                .Include(d => d.Lease)
                    .ThenInclude(l => l!.Tenant)
                .Include(d => d.Invoice)
                .Include(d => d.Payment)
                .Where(d => !d.IsDeleted && d.OrganizationId == organizationId && d.Property != null && !d.Property.IsDeleted)
                .OrderByDescending(d => d.CreatedOn)
                .ToListAsync();
        }

        public async Task<Document?> GetDocumentByIdAsync(int documentId)
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();


            return await _dbContext.Documents
                .Include(d => d.Property)
                .Include(d => d.Tenant)
                .Include(d => d.Lease)
                    .ThenInclude(l => l!.Property)
                .Include(d => d.Lease)
                    .ThenInclude(l => l!.Tenant)
                .Include(d => d.Invoice)
                .Include(d => d.Payment)
                .FirstOrDefaultAsync(d => d.Id == documentId && !d.IsDeleted && d.OrganizationId == organizationId);
        }

        public async Task<List<Document>> GetDocumentsByLeaseIdAsync(int leaseId)
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();

            return await _dbContext.Documents
                .Include(d => d.Lease)
                    .ThenInclude(l => l!.Property)
                .Include(d => d.Lease)
                    .ThenInclude(l => l!.Tenant)
                .Where(d => d.LeaseId == leaseId && !d.IsDeleted && d.OrganizationId == organizationId)
                .OrderByDescending(d => d.CreatedOn)
                .ToListAsync();
        }
        
        public async Task<List<Document>> GetDocumentsByPropertyIdAsync(int propertyId)
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();

            return await _dbContext.Documents
                .Include(d => d.Property)
                .Include(d => d.Tenant)
                .Include(d => d.Lease)
                .Where(d => d.PropertyId == propertyId && !d.IsDeleted && d.OrganizationId == organizationId)
                .OrderByDescending(d => d.CreatedOn)
                .ToListAsync();
        }

        public async Task<List<Document>> GetDocumentsByTenantIdAsync(int tenantId)
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();

            return await _dbContext.Documents
                .Include(d => d.Property)
                .Include(d => d.Tenant)
                .Include(d => d.Lease)
                .Where(d => d.TenantId == tenantId && !d.IsDeleted && d.OrganizationId == organizationId)
                .OrderByDescending(d => d.CreatedOn)
                .ToListAsync();
        }

        public async Task<Document> AddDocumentAsync(Document document)
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();

            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (_userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            document.CreatedBy = _userId!;
            document.OrganizationId = organizationId!;
            document.CreatedOn = DateTime.UtcNow;
            _dbContext.Documents.Add(document);
            await _dbContext.SaveChangesAsync();
            return document;
        }

        public async Task UpdateDocumentAsync(Document document)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (_userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            document.LastModifiedBy = _userId!;
            document.LastModifiedOn = DateTime.UtcNow;
            _dbContext.Documents.Update(document);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteDocumentAsync(Document document)
        {

            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (_userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            
            if (!_applicationSettings.SoftDeleteEnabled)
            {
                _dbContext.Documents.Remove(document);
            }
            else
            {
                document.IsDeleted = true;
                document.LastModifiedBy = _userId!;
                document.LastModifiedOn = DateTime.UtcNow;
                _dbContext.Documents.Update(document);

                // Clear reverse foreign keys in related entities
                // Since soft delete doesn't trigger DB cascade, we need to manually clear DocumentId
                
                // Clear Inspection.DocumentId if any inspection links to this document
                var inspection = await _dbContext.Inspections
                    .FirstOrDefaultAsync(i => i.DocumentId == document.Id);
                if (inspection != null)
                {
                    inspection.DocumentId = null;
                    inspection.LastModifiedBy = _userId;
                    inspection.LastModifiedOn = DateTime.UtcNow;
                    _dbContext.Inspections.Update(inspection);
                }

                // Clear Lease.DocumentId if any lease links to this document
                var lease = await _dbContext.Leases
                    .FirstOrDefaultAsync(l => l.DocumentId == document.Id);
                if (lease != null)
                {
                    lease.DocumentId = null;
                    lease.LastModifiedBy = _userId;
                    lease.LastModifiedOn = DateTime.UtcNow;
                    _dbContext.Leases.Update(lease);
                }

                // Clear Invoice.DocumentId if any invoice links to this document
                if (document.InvoiceId != null)
                {
                    var invoice = await _dbContext.Invoices
                        .FirstOrDefaultAsync(i => i.Id == document.InvoiceId.Value && i.DocumentId == document.Id);
                    if (invoice != null)
                    {
                        invoice.DocumentId = null;
                        invoice.LastModifiedBy = _userId;
                        invoice.LastModifiedOn = DateTime.UtcNow;
                        _dbContext.Invoices.Update(invoice);
                    }
                }

                // Clear Payment.DocumentId if any payment links to this document
                if (document.PaymentId != null)
                {
                    var payment = await _dbContext.Payments
                        .FirstOrDefaultAsync(p => p.Id == document.PaymentId.Value && p.DocumentId == document.Id);
                    if (payment != null)
                    {
                        payment.DocumentId = null;
                        payment.LastModifiedBy = _userId;
                        payment.LastModifiedOn = DateTime.UtcNow;
                        _dbContext.Payments.Update(payment);
                    }
                }
            }
            await _dbContext.SaveChangesAsync();
        }

        #endregion

        #region Inspections
        
        public async Task<List<Inspection>> GetInspectionsAsync()
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();
            
            return await _dbContext.Inspections
                .Include(i => i.Property)
                .Include(i => i.Lease)
                    .ThenInclude(l => l!.Tenant)
                .Where(i => !i.IsDeleted && i.OrganizationId == organizationId)
                .OrderByDescending(i => i.CompletedOn)
                .ToListAsync();
        }

        public async Task<List<Inspection>> GetInspectionsByPropertyIdAsync(int propertyId)
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();
            
            return await _dbContext.Inspections
                .Include(i => i.Property)
                .Include(i => i.Lease)
                    .ThenInclude(l => l!.Tenant)
                .Where(i => i.PropertyId == propertyId && !i.IsDeleted && i.OrganizationId == organizationId)
                .OrderByDescending(i => i.CompletedOn)
                .ToListAsync();
        }

        public async Task<Inspection?> GetInspectionByIdAsync(int inspectionId)
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();
            
            return await _dbContext.Inspections
                .Include(i => i.Property)
                .Include(i => i.Lease)
                    .ThenInclude(l => l!.Tenant)
                .FirstOrDefaultAsync(i => i.Id == inspectionId && !i.IsDeleted && i.OrganizationId == organizationId);
        }

        public async Task AddInspectionAsync(Inspection inspection)
        {

            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (_userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            inspection.CreatedBy = _userId!;
            inspection.CreatedOn = DateTime.UtcNow;
            await _dbContext.Inspections.AddAsync(inspection);
            await _dbContext.SaveChangesAsync();

            // Create calendar event for the inspection
            await _calendarEventService.CreateOrUpdateEventAsync(inspection);

            // Update property inspection tracking if this is a routine inspection
            if (inspection.InspectionType == "Routine")
            {
                // Find and update/delete the original property-based routine inspection calendar event
                var propertyBasedEvent = await _dbContext.CalendarEvents
                    .FirstOrDefaultAsync(e => 
                        e.PropertyId == inspection.PropertyId &&
                        e.SourceEntityType == "Property" &&
                        e.EventType == CalendarEventTypes.Inspection &&
                        !e.IsDeleted);

                if (propertyBasedEvent != null)
                {
                    // Remove the old property-based event since we now have an actual inspection record
                    _dbContext.CalendarEvents.Remove(propertyBasedEvent);
                    await _dbContext.SaveChangesAsync();
                }

                await UpdatePropertyInspectionTrackingAsync(
                    inspection.PropertyId, 
                    inspection.CompletedOn);
            }
        }

        public async Task UpdateInspectionAsync(Inspection inspection)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (_userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            inspection.LastModifiedBy = _userId!;
            inspection.LastModifiedOn = DateTime.UtcNow;
            _dbContext.Inspections.Update(inspection);
            await _dbContext.SaveChangesAsync();

            // Update calendar event
            await _calendarEventService.CreateOrUpdateEventAsync(inspection);
        }

        public async Task DeleteInspectionAsync(int inspectionId)
        {
            var userId = await _userContext.GetUserIdAsync();
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var inspection = await _dbContext.Inspections.FindAsync(inspectionId);
            if (inspection != null && !inspection.IsDeleted)
            {
                if (_applicationSettings.SoftDeleteEnabled)
                {
                    inspection.IsDeleted = true;
                    inspection.LastModifiedOn = DateTime.UtcNow;
                    inspection.LastModifiedBy = userId;
                    _dbContext.Inspections.Update(inspection);
                }
                else
                {
                    _dbContext.Inspections.Remove(inspection);
                }
                await _dbContext.SaveChangesAsync();

                // Delete associated calendar event
                await _calendarEventService.DeleteEventAsync(inspection.CalendarEventId);
            }
        }

        #endregion

        #region Inspection Tracking

        /// <summary>
        /// Updates property inspection tracking after a routine inspection is completed
        /// </summary>
        public async Task UpdatePropertyInspectionTrackingAsync(int propertyId, DateTime inspectionDate, int intervalMonths = 12)
        {
            var property = await _dbContext.Properties.FindAsync(propertyId);
            if (property == null || property.IsDeleted)
            {
                throw new InvalidOperationException("Property not found.");
            }

            property.LastRoutineInspectionDate = inspectionDate;
            property.NextRoutineInspectionDueDate = inspectionDate.AddMonths(intervalMonths);
            property.RoutineInspectionIntervalMonths = intervalMonths;
            property.LastModifiedOn = DateTime.UtcNow;
            
            var userId = await _userContext.GetUserIdAsync();
            property.LastModifiedBy = userId ?? "System";

            _dbContext.Properties.Update(property);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Gets properties with overdue routine inspections
        /// </summary>
        public async Task<List<Property>> GetPropertiesWithOverdueInspectionsAsync()
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();
            
            return await _dbContext.Properties
                .Where(p => p.OrganizationId == organizationId && 
                           !p.IsDeleted &&
                           p.NextRoutineInspectionDueDate.HasValue &&
                           p.NextRoutineInspectionDueDate.Value < DateTime.Today)
                .OrderBy(p => p.NextRoutineInspectionDueDate)
                .ToListAsync();
        }

        /// <summary>
        /// Gets properties with inspections due within specified days
        /// </summary>
        public async Task<List<Property>> GetPropertiesWithInspectionsDueSoonAsync(int daysAhead = 30)
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();
            var dueDate = DateTime.Today.AddDays(daysAhead);
            
            return await _dbContext.Properties
                .Where(p => p.OrganizationId == organizationId && 
                           !p.IsDeleted &&
                           p.NextRoutineInspectionDueDate.HasValue &&
                           p.NextRoutineInspectionDueDate.Value >= DateTime.Today &&
                           p.NextRoutineInspectionDueDate.Value <= dueDate)
                .OrderBy(p => p.NextRoutineInspectionDueDate)
                .ToListAsync();
        }

        /// <summary>
        /// Gets count of properties with overdue inspections
        /// </summary>
        public async Task<int> GetOverdueInspectionCountAsync()
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();
            
            return await _dbContext.Properties
                .CountAsync(p => p.OrganizationId == organizationId && 
                                !p.IsDeleted &&
                                p.NextRoutineInspectionDueDate.HasValue &&
                                p.NextRoutineInspectionDueDate.Value < DateTime.Today);
        }

        /// <summary>
        /// Initializes inspection tracking for a property (sets first inspection due date)
        /// </summary>
        public async Task InitializePropertyInspectionTrackingAsync(int propertyId, int intervalMonths = 12)
        {
            var property = await _dbContext.Properties.FindAsync(propertyId);
            if (property == null || property.IsDeleted)
            {
                throw new InvalidOperationException("Property not found.");
            }

            if (!property.NextRoutineInspectionDueDate.HasValue)
            {
                property.NextRoutineInspectionDueDate = DateTime.Today.AddMonths(intervalMonths);
                property.RoutineInspectionIntervalMonths = intervalMonths;
                property.LastModifiedOn = DateTime.UtcNow;
                
                var userId = await _userContext.GetUserIdAsync();
                property.LastModifiedBy = userId ?? "System";

                _dbContext.Properties.Update(property);
                await _dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Creates a calendar event for a routine property inspection
        /// </summary>
        private async Task CreateRoutineInspectionCalendarEventAsync(Property property)
        {
            if (!property.NextRoutineInspectionDueDate.HasValue)
            {
                return;
            }

            var userId = await _userContext.GetUserIdAsync();

            var calendarEvent = new CalendarEvent
            {
                Title = $"Routine Inspection - {property.Address}",
                Description = $"Routine inspection due for property at {property.Address}, {property.City}, {property.State}",
                StartOn = property.NextRoutineInspectionDueDate.Value,
                DurationMinutes = 60, // Default 1 hour for inspection
                EventType = CalendarEventTypes.Inspection,
                Status = "Scheduled",
                PropertyId = property.Id,
                Location = $"{property.Address}, {property.City}, {property.State} {property.ZipCode}",
                Color = CalendarEventTypes.GetColor(CalendarEventTypes.Inspection),
                Icon = CalendarEventTypes.GetIcon(CalendarEventTypes.Inspection),
                OrganizationId = property.OrganizationId,
                CreatedBy = userId ?? "System",
                CreatedOn = DateTime.UtcNow,
                SourceEntityType = "Property",
                SourceEntityId = property.Id
            };

            _dbContext.CalendarEvents.Add(calendarEvent);
            await _dbContext.SaveChangesAsync();
        }

        #endregion

        #region Maintenance Requests

        public async Task<List<MaintenanceRequest>> GetMaintenanceRequestsAsync()
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();

            return await _dbContext.MaintenanceRequests
                .Include(m => m.Property)
                .Include(m => m.Lease)
                .Where(m => m.OrganizationId == organizationId && !m.IsDeleted)
                .OrderByDescending(m => m.RequestedOn)
                .ToListAsync();
        }

        public async Task<List<MaintenanceRequest>> GetMaintenanceRequestsByPropertyAsync(int propertyId)
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();

            return await _dbContext.MaintenanceRequests
                .Include(m => m.Property)
                .Include(m => m.Lease)
                .Where(m => m.PropertyId == propertyId && m.OrganizationId == organizationId && !m.IsDeleted)
                .OrderByDescending(m => m.RequestedOn)
                .ToListAsync();
        }

        public async Task<List<MaintenanceRequest>> GetMaintenanceRequestsByLeaseAsync(int leaseId)
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();

            return await _dbContext.MaintenanceRequests
                .Include(m => m.Property)
                .Include(m => m.Lease)
                .Where(m => m.LeaseId == leaseId && m.OrganizationId == organizationId && !m.IsDeleted)
                .OrderByDescending(m => m.RequestedOn)
                .ToListAsync();
        }

        public async Task<List<MaintenanceRequest>> GetMaintenanceRequestsByStatusAsync(string status)
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();

            return await _dbContext.MaintenanceRequests
                .Include(m => m.Property)
                .Include(m => m.Lease)
                .Where(m => m.Status == status && m.OrganizationId == organizationId && !m.IsDeleted)
                .OrderByDescending(m => m.RequestedOn)
                .ToListAsync();
        }

        public async Task<List<MaintenanceRequest>> GetMaintenanceRequestsByPriorityAsync(string priority)
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();

            return await _dbContext.MaintenanceRequests
                .Include(m => m.Property)
                .Include(m => m.Lease)
                .Where(m => m.Priority == priority && m.OrganizationId == organizationId && !m.IsDeleted)
                .OrderByDescending(m => m.RequestedOn)
                .ToListAsync();
        }

        public async Task<List<MaintenanceRequest>> GetOverdueMaintenanceRequestsAsync()
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();
            var today = DateTime.Today;

            return await _dbContext.MaintenanceRequests
                .Include(m => m.Property)
                .Include(m => m.Lease)
                .Where(m => m.OrganizationId == organizationId && 
                           !m.IsDeleted &&
                           m.Status != "Completed" &&
                           m.Status != "Cancelled" &&
                           m.ScheduledOn.HasValue &&
                           m.ScheduledOn.Value.Date < today)
                .OrderBy(m => m.ScheduledOn)
                .ToListAsync();
        }

        public async Task<int> GetOpenMaintenanceRequestCountAsync()
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();

            return await _dbContext.MaintenanceRequests
                .Where(m => m.OrganizationId == organizationId && 
                           !m.IsDeleted &&
                           m.Status != "Completed" &&
                           m.Status != "Cancelled")
                .CountAsync();
        }

        public async Task<int> GetUrgentMaintenanceRequestCountAsync()
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();

            return await _dbContext.MaintenanceRequests
                .Where(m => m.OrganizationId == organizationId && 
                           !m.IsDeleted &&
                           m.Priority == "Urgent" &&
                           m.Status != "Completed" &&
                           m.Status != "Cancelled")
                .CountAsync();
        }

        public async Task<MaintenanceRequest?> GetMaintenanceRequestByIdAsync(int id)
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();

            return await _dbContext.MaintenanceRequests
                .Include(m => m.Property)
                .Include(m => m.Lease)
                .FirstOrDefaultAsync(m => m.Id == id && m.OrganizationId == organizationId && !m.IsDeleted);
        }

        public async Task AddMaintenanceRequestAsync(MaintenanceRequest maintenanceRequest)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();
            maintenanceRequest.OrganizationId = organizationId!;
            maintenanceRequest.CreatedBy = _userId;
            maintenanceRequest.CreatedOn = DateTime.UtcNow;

            await _dbContext.MaintenanceRequests.AddAsync(maintenanceRequest);
            await _dbContext.SaveChangesAsync();

            // Create calendar event for the maintenance request
            await _calendarEventService.CreateOrUpdateEventAsync(maintenanceRequest);
        }

        public async Task UpdateMaintenanceRequestAsync(MaintenanceRequest maintenanceRequest)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            if (maintenanceRequest.OrganizationId != organizationId)
            {
                throw new UnauthorizedAccessException("User is not authorized to update this maintenance request.");
            }

            maintenanceRequest.LastModifiedBy = _userId;
            maintenanceRequest.LastModifiedOn = DateTime.UtcNow;

            _dbContext.MaintenanceRequests.Update(maintenanceRequest);
            await _dbContext.SaveChangesAsync();

            // Update calendar event
            await _calendarEventService.CreateOrUpdateEventAsync(maintenanceRequest);
        }

        public async Task DeleteMaintenanceRequestAsync(int id)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            var maintenanceRequest = await _dbContext.MaintenanceRequests
                .FirstOrDefaultAsync(m => m.Id == id && m.OrganizationId == organizationId);

            if (maintenanceRequest != null)
            {
                maintenanceRequest.IsDeleted = true;
                maintenanceRequest.LastModifiedOn = DateTime.Now;
                maintenanceRequest.LastModifiedBy = _userId;

                _dbContext.MaintenanceRequests.Update(maintenanceRequest);
                await _dbContext.SaveChangesAsync();

                // Delete associated calendar event
                await _calendarEventService.DeleteEventAsync(maintenanceRequest.CalendarEventId);
            }
        }

        public async Task UpdateMaintenanceRequestStatusAsync(int id, string status)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var organizationId = await _userContext.GetOrganizationIdAsync();

            var maintenanceRequest = await _dbContext.MaintenanceRequests
                .FirstOrDefaultAsync(m => m.Id == id && m.OrganizationId == organizationId && !m.IsDeleted);

            if (maintenanceRequest != null)
            {
                maintenanceRequest.Status = status;
                maintenanceRequest.LastModifiedOn = DateTime.Now;
                maintenanceRequest.LastModifiedBy = _userId;

                if (status == "Completed")
                {
                    maintenanceRequest.CompletedOn = DateTime.Today;
                }

                _dbContext.MaintenanceRequests.Update(maintenanceRequest);
                await _dbContext.SaveChangesAsync();
            }
        }

        #endregion

        #region Organization Settings

        /// <summary>
        /// Gets the organization settings for the current user's organization.
        /// If no settings exist, creates default settings.
        /// </summary>
        public async Task<OrganizationSettings?> GetOrganizationSettingsAsync()
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();

            if (string.IsNullOrEmpty(organizationId))
            {
                throw new InvalidOperationException("Organization ID not found for current user");
            }

            var settings = await _dbContext.OrganizationSettings
                .Where(s => !s.IsDeleted && s.OrganizationId.ToString() == organizationId)
                .FirstOrDefaultAsync();

            // Create default settings if they don't exist
            if (settings == null)
            {
                var userId = await _userContext.GetUserIdAsync();
                settings = new OrganizationSettings
                {
                    OrganizationId = Guid.Parse(organizationId),
                    LateFeeEnabled = true,
                    LateFeeAutoApply = true,
                    LateFeeGracePeriodDays = 3,
                    LateFeePercentage = 0.05m,
                    MaxLateFeeAmount = 50.00m,
                    PaymentReminderEnabled = true,
                    PaymentReminderDaysBefore = 3,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = userId ?? "System"
                };
                
                await _dbContext.OrganizationSettings.AddAsync(settings);
                await _dbContext.SaveChangesAsync();
            }

            return settings;
        }

        /// <summary>
        /// Gets organization settings by organization ID (used by scheduled tasks).
        /// </summary>
        public async Task<OrganizationSettings?> GetOrganizationSettingsByOrgIdAsync(Guid organizationId)
        {
            return await _dbContext.OrganizationSettings
                .Where(s => !s.IsDeleted && s.OrganizationId == organizationId)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets organization settings by organization ID string (used by scheduled tasks).
        /// </summary>
        public async Task<OrganizationSettings?> GetOrganizationSettingsByOrgIdAsync(string organizationId)
        {
            if (Guid.TryParse(organizationId, out var guid))
            {
                return await GetOrganizationSettingsByOrgIdAsync(guid);
            }
            return null;
        }

        /// <summary>
        /// Updates the organization settings for the current user's organization.
        /// </summary>
        public async Task UpdateOrganizationSettingsAsync(OrganizationSettings settings)
        {
            var userId = await _userContext.GetUserIdAsync();
            
            settings.LastModifiedOn = DateTime.UtcNow;
            settings.LastModifiedBy = userId ?? "System";

            _dbContext.OrganizationSettings.Update(settings);
            await _dbContext.SaveChangesAsync();
        }

        #endregion
   }
}