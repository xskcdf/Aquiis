using Aquiis.SimpleStart.Core.Entities;
using Aquiis.SimpleStart.Shared.Components.Account;
using Aquiis.SimpleStart.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Aquiis.SimpleStart.Core.Constants;
using Aquiis.SimpleStart.Shared.Services;
using System.Security.Claims;
using Microsoft.Extensions.Options;

namespace Aquiis.SimpleStart.Application.Services
{
    public class PropertyManagementService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationSettings _applicationSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserContextService _userContext;
        private readonly CalendarEventService _calendarEventService;
        private readonly ChecklistService _checklistService;

        public PropertyManagementService(
            ApplicationDbContext dbContext, 
            UserManager<ApplicationUser> userManager, 
            IOptions<ApplicationSettings> settings, 
            IHttpContextAccessor httpContextAccessor,
            UserContextService userContext,
            CalendarEventService calendarEventService,
            ChecklistService checklistService)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _applicationSettings = settings.Value;
            _httpContextAccessor = httpContextAccessor;
            _userContext = userContext;
            _calendarEventService = calendarEventService;
            _checklistService = checklistService;
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

        public async Task<List<Property>> SearchPropertiesByAddressAsync(string searchTerm)
        {
            var _userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (_userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            
            var organizationId = await _userContext.GetOrganizationIdAsync();
            
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await _dbContext.Properties
                    .Where(p => !p.IsDeleted && p.OrganizationId == organizationId)
                    .OrderBy(p => p.Address)
                    .Take(20)
                    .ToListAsync();
            }
            
            return await _dbContext.Properties
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

        public async Task<Tenant> AddTenantAsync(Tenant tenant)
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
            
            return tenant;
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
                .FirstOrDefaultAsync(l => l.Id == leaseId && !l.IsDeleted && (l.Tenant == null || !l.Tenant.IsDeleted) && !l.Property.IsDeleted && l.Property.OrganizationId == organizationId);
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

        #region PreLeaseOperations

        #region ProspectiveTenant CRUD

        public async Task<List<ProspectiveTenant>> GetAllProspectiveTenantsAsync(string organizationId)
        {
            return await _dbContext.ProspectiveTenants
                .Where(pt => pt.OrganizationId == organizationId && !pt.IsDeleted)
                .Include(pt => pt.InterestedProperty)
                .Include(pt => pt.Tours)
                .Include(pt => pt.Application)
                .OrderByDescending(pt => pt.CreatedOn)
                .ToListAsync();
        }

        public async Task<ProspectiveTenant?> GetProspectiveTenantByIdAsync(int id, string organizationId)
        {
            return await _dbContext.ProspectiveTenants
                .Where(pt => pt.Id == id && pt.OrganizationId == organizationId && !pt.IsDeleted)
                .Include(pt => pt.InterestedProperty)
                .Include(pt => pt.Tours)
                .Include(pt => pt.Application)
                .FirstOrDefaultAsync();
        }

        public async Task<ProspectiveTenant> CreateProspectiveTenantAsync(ProspectiveTenant prospectiveTenant)
        {
            prospectiveTenant.CreatedOn = DateTime.UtcNow;
            prospectiveTenant.Status = ApplicationConstants.ProspectiveStatuses.Lead;
            prospectiveTenant.FirstContactedOn = DateTime.UtcNow;

            _dbContext.ProspectiveTenants.Add(prospectiveTenant);
            await _dbContext.SaveChangesAsync();
            return prospectiveTenant;
        }

        public async Task<ProspectiveTenant> UpdateProspectiveTenantAsync(ProspectiveTenant prospectiveTenant)
        {
            prospectiveTenant.LastModifiedOn = DateTime.UtcNow;
            _dbContext.ProspectiveTenants.Update(prospectiveTenant);
            await _dbContext.SaveChangesAsync();
            return prospectiveTenant;
        }

        public async Task DeleteProspectiveTenantAsync(int id, string organizationId, string deletedBy)
        {
            var prospectiveTenant = await GetProspectiveTenantByIdAsync(id, organizationId);
            if (prospectiveTenant != null)
            {
                prospectiveTenant.IsDeleted = true;
                prospectiveTenant.LastModifiedOn = DateTime.UtcNow;
                prospectiveTenant.LastModifiedBy = deletedBy;
                await _dbContext.SaveChangesAsync();
            }
        }

        #endregion

        #region Tour CRUD

        public async Task<List<Tour>> GetAllToursAsync(string organizationId)
        {
            return await _dbContext.Tours
                .Where(s => s.OrganizationId == organizationId && !s.IsDeleted)
                .Include(s => s.ProspectiveTenant)
                .Include(s => s.Property)
                .Include(s => s.Checklist)
            .OrderBy(s => s.ScheduledOn)
            .ToListAsync();
        }

        public async Task<List<Tour>> GetToursByProspectiveIdAsync(int prospectiveTenantId, string organizationId)
        {
            return await _dbContext.Tours
                .Where(s => s.ProspectiveTenantId == prospectiveTenantId && s.OrganizationId == organizationId && !s.IsDeleted)
                .Include(s => s.ProspectiveTenant)
                .Include(s => s.Property)
                .Include(s => s.Checklist)
                .OrderBy(s => s.ScheduledOn)
                .ToListAsync();
        }

        public async Task<Tour?> GetTourByIdAsync(int id, string organizationId)
        {
            return await _dbContext.Tours
                .Where(s => s.Id == id && s.OrganizationId == organizationId && !s.IsDeleted)
                .Include(s => s.ProspectiveTenant)
                .Include(s => s.Property)
                .Include(s => s.Checklist)
                .FirstOrDefaultAsync();
        }

        public async Task<Tour> CreateTourAsync(Tour tour, int? templateId = null)
        {
            tour.CreatedOn = DateTime.UtcNow;
            tour.Status = ApplicationConstants.TourStatuses.Scheduled;

            // Get prospect information for checklist
            var prospective = await _dbContext.ProspectiveTenants
                .Include(p => p.InterestedProperty)
                .FirstOrDefaultAsync(p => p.Id == tour.ProspectiveTenantId);

            // Find the specified template, or fall back to default "Property Tour" template
            ChecklistTemplate? tourTemplate = null;
            
            if (templateId.HasValue && templateId.Value > 0)
            {
                // Use the specified template
                tourTemplate = await _dbContext.ChecklistTemplates
                    .FirstOrDefaultAsync(t => t.Id == templateId.Value && 
                        (t.OrganizationId == tour.OrganizationId || t.IsSystemTemplate) && 
                        !t.IsDeleted);
            }
            
            // Fall back to default "Property Tour" template if not specified or not found
            if (tourTemplate == null)
            {
                tourTemplate = await _dbContext.ChecklistTemplates
                    .FirstOrDefaultAsync(t => t.Name == "Property Tour" && 
                        (t.OrganizationId == tour.OrganizationId || t.IsSystemTemplate) && 
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

            _dbContext.Tours.Add(tour);
            await _dbContext.SaveChangesAsync();

            // Create calendar event for the tour
            await _calendarEventService.CreateOrUpdateEventAsync(tour);

            // Update ProspectiveTenant status
            if (prospective != null && prospective.Status == ApplicationConstants.ProspectiveStatuses.Lead)
            {
                prospective.Status = ApplicationConstants.ProspectiveStatuses.TourScheduled;
                prospective.LastModifiedOn = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }

            return tour;
        }

        public async Task<Tour> UpdateTourAsync(Tour tour)
        {
            tour.LastModifiedOn = DateTime.UtcNow;
            _dbContext.Tours.Update(tour);
            await _dbContext.SaveChangesAsync();

            // Update calendar event
            await _calendarEventService.CreateOrUpdateEventAsync(tour);

            return tour;
        }

        public async Task DeleteTourAsync(int id, string organizationId, string deletedBy)
        {
            var tour = await GetTourByIdAsync(id, organizationId);
            if (tour != null)
            {
                tour.IsDeleted = true;
                tour.LastModifiedOn = DateTime.UtcNow;
                tour.LastModifiedBy = deletedBy;
                await _dbContext.SaveChangesAsync();

                // Delete associated calendar event
                await _calendarEventService.DeleteEventAsync(tour.CalendarEventId);
            }
        }

        public async Task<bool> CancelTourAsync(int tourId, string organizationId, string cancelledBy)
        {
            var tour = await GetTourByIdAsync(tourId, organizationId);
            if (tour == null) return false;

            // Update tour status to cancelled
            tour.Status = ApplicationConstants.TourStatuses.Cancelled;
            tour.LastModifiedOn = DateTime.UtcNow;
            tour.LastModifiedBy = cancelledBy;
            await _dbContext.SaveChangesAsync();

            // Update calendar event status
            await _calendarEventService.CreateOrUpdateEventAsync(tour);

            // Check if prospect has any other scheduled tours
            var prospective = await _dbContext.ProspectiveTenants.FindAsync(tour.ProspectiveTenantId);
            if (prospective != null && prospective.Status == ApplicationConstants.ProspectiveStatuses.TourScheduled)
            {
                var hasOtherScheduledTours = await _dbContext.Tours
                    .AnyAsync(s => s.ProspectiveTenantId == tour.ProspectiveTenantId
                        && s.Id != tourId
                        && !s.IsDeleted
                        && s.Status == ApplicationConstants.TourStatuses.Scheduled);

                // If no other scheduled tours, revert prospect status to Lead
                if (!hasOtherScheduledTours)
                {
                    prospective.Status = ApplicationConstants.ProspectiveStatuses.Lead;
                    prospective.LastModifiedOn = DateTime.UtcNow;
                    prospective.LastModifiedBy = cancelledBy;
                    await _dbContext.SaveChangesAsync();
                }
            }

            return true;
        }

        public async Task<bool> CompleteTourAsync(int tourId, string organizationId, string completedBy, string? feedback = null, string? interestLevel = null)
        {
            var tour = await GetTourByIdAsync(tourId, organizationId);
            if (tour == null) return false;

            // Update tour status and feedback
            tour.Status = ApplicationConstants.TourStatuses.Completed;
            tour.Feedback = feedback;
            tour.InterestLevel = interestLevel;
            tour.ConductedBy = completedBy;
            tour.LastModifiedOn = DateTime.UtcNow;
            tour.LastModifiedBy = completedBy;

            // Update calendar event status
            if (tour.CalendarEventId.HasValue)
            {
                var calendarEvent = await _dbContext.CalendarEvents
                    .FirstOrDefaultAsync(e => e.Id == tour.CalendarEventId.Value);
                if (calendarEvent != null)
                {
                    calendarEvent.Status = ApplicationConstants.TourStatuses.Completed;
                    calendarEvent.LastModifiedBy = completedBy;
                    calendarEvent.LastModifiedOn = DateTime.UtcNow;
                }
            }

            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> MarkTourAsNoShowAsync(int tourId, string organizationId, string markedBy)
        {
            var tour = await GetTourByIdAsync(tourId, organizationId);
            if (tour == null) return false;

            // Update tour status to NoShow
            tour.Status = ApplicationConstants.TourStatuses.NoShow;
            tour.LastModifiedOn = DateTime.UtcNow;
            tour.LastModifiedBy = markedBy;

            // Update calendar event status
            if (tour.CalendarEventId.HasValue)
            {
                var calendarEvent = await _dbContext.CalendarEvents
                    .FirstOrDefaultAsync(e => e.Id == tour.CalendarEventId.Value);
                if (calendarEvent != null)
                {
                    calendarEvent.Status = ApplicationConstants.TourStatuses.NoShow;
                    calendarEvent.LastModifiedBy = markedBy;
                    calendarEvent.LastModifiedOn = DateTime.UtcNow;
                }
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }

        #endregion

        #region RentalApplication CRUD

        public async Task<List<RentalApplication>> GetAllRentalApplicationsAsync(string organizationId)
        {
            return await _dbContext.RentalApplications
                .Where(ra => ra.OrganizationId == organizationId && !ra.IsDeleted)
                .Include(ra => ra.ProspectiveTenant)
                .Include(ra => ra.Property)
                .Include(ra => ra.Screening)
                .OrderByDescending(ra => ra.AppliedOn)
                .ToListAsync();
        }

        public async Task<RentalApplication?> GetRentalApplicationByIdAsync(int id, string organizationId)
        {
            return await _dbContext.RentalApplications
                .Where(ra => ra.Id == id && ra.OrganizationId == organizationId && !ra.IsDeleted)
                .Include(ra => ra.ProspectiveTenant)
                .Include(ra => ra.Property)
                .Include(ra => ra.Screening)
                .FirstOrDefaultAsync();
        }

        public async Task<RentalApplication?> GetApplicationByProspectiveIdAsync(int prospectiveTenantId, string organizationId)
        {
            return await _dbContext.RentalApplications
                .Where(ra => ra.ProspectiveTenantId == prospectiveTenantId && ra.OrganizationId == organizationId && !ra.IsDeleted)
                .Include(ra => ra.Property)
                .Include(ra => ra.Screening)
                .FirstOrDefaultAsync();
        }

        public async Task<RentalApplication> CreateRentalApplicationAsync(RentalApplication application)
        {
            application.CreatedOn = DateTime.UtcNow;
            application.AppliedOn = DateTime.UtcNow;
            application.Status = ApplicationConstants.ApplicationStatuses.Submitted;

            // Get organization settings for fee and expiration defaults
            var orgSettings = await _dbContext.OrganizationSettings
                .FirstOrDefaultAsync(s => s.OrganizationId.ToString() == application.OrganizationId && !s.IsDeleted);

            if (orgSettings != null)
            {
                // Set application fee if not already set and fees are enabled
                if (orgSettings.ApplicationFeeEnabled && application.ApplicationFee == 0)
                {
                    application.ApplicationFee = orgSettings.DefaultApplicationFee;
                }

                // Set expiration date if not already set
                if (application.ExpiresOn == null)
                {
                    application.ExpiresOn = application.AppliedOn.AddDays(orgSettings.ApplicationExpirationDays);
                }
            }
            else
            {
                // Fallback defaults if no settings found
                if (application.ApplicationFee == 0)
                {
                    application.ApplicationFee = 50.00m; // Default fee
                }
                if (application.ExpiresOn == null)
                {
                    application.ExpiresOn = application.AppliedOn.AddDays(30); // Default 30 days
                }
            }

            _dbContext.RentalApplications.Add(application);
            await _dbContext.SaveChangesAsync();

            // Update property status to ApplicationPending
            var property = await _dbContext.Properties.FindAsync(application.PropertyId);
            if (property != null && property.Status == ApplicationConstants.PropertyStatuses.Available)
            {
                property.Status = ApplicationConstants.PropertyStatuses.ApplicationPending;
                property.LastModifiedOn = DateTime.UtcNow;
                property.LastModifiedBy = application.CreatedBy;
                await _dbContext.SaveChangesAsync();
            }

            // Update ProspectiveTenant status
            var prospective = await _dbContext.ProspectiveTenants.FindAsync(application.ProspectiveTenantId);
            if (prospective != null)
            {
                prospective.Status = ApplicationConstants.ProspectiveStatuses.Applied;
                prospective.LastModifiedOn = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }

            return application;
        }

        public async Task<RentalApplication> UpdateRentalApplicationAsync(RentalApplication application)
        {
            application.LastModifiedOn = DateTime.UtcNow;
            _dbContext.RentalApplications.Update(application);
            await _dbContext.SaveChangesAsync();
            return application;
        }

        public async Task DeleteRentalApplicationAsync(int id, string organizationId, string deletedBy)
        {
            var application = await GetRentalApplicationByIdAsync(id, organizationId);
            if (application != null)
            {
                application.IsDeleted = true;
                application.LastModifiedOn = DateTime.UtcNow;
                application.LastModifiedBy = deletedBy;
                await _dbContext.SaveChangesAsync();
            }
        }

        #endregion

        #region ApplicationScreening CRUD

        public async Task<ApplicationScreening?> GetScreeningByApplicationIdAsync(int rentalApplicationId, string organizationId)
        {
            return await _dbContext.ApplicationScreenings
                .Where(asc => asc.RentalApplicationId == rentalApplicationId && asc.OrganizationId == organizationId && !asc.IsDeleted)
                .Include(asc => asc.RentalApplication)
                .FirstOrDefaultAsync();
        }

        public async Task<ApplicationScreening> CreateScreeningAsync(ApplicationScreening screening)
        {
            screening.CreatedOn = DateTime.UtcNow;
            screening.OverallResult = ApplicationConstants.ScreeningResults.Pending;

            _dbContext.ApplicationScreenings.Add(screening);
            await _dbContext.SaveChangesAsync();

            // Update application and prospective tenant status
            var application = await _dbContext.RentalApplications.FindAsync(screening.RentalApplicationId);
            if (application != null)
            {
                application.Status = ApplicationConstants.ApplicationStatuses.Screening;
                application.LastModifiedOn = DateTime.UtcNow;

                var prospective = await _dbContext.ProspectiveTenants.FindAsync(application.ProspectiveTenantId);
                if (prospective != null)
                {
                    prospective.Status = ApplicationConstants.ProspectiveStatuses.Screening;
                    prospective.LastModifiedOn = DateTime.UtcNow;
                }

                await _dbContext.SaveChangesAsync();
            }

            return screening;
        }

        public async Task<ApplicationScreening> UpdateScreeningAsync(ApplicationScreening screening)
        {
            screening.LastModifiedOn = DateTime.UtcNow;
            _dbContext.ApplicationScreenings.Update(screening);
            await _dbContext.SaveChangesAsync();
            return screening;
        }

        #endregion

        #region Business Logic

        public async Task<bool> ApproveApplicationAsync(int applicationId, string organizationId, string approvedBy)
        {
            var application = await GetRentalApplicationByIdAsync(applicationId, organizationId);
            if (application == null) return false;

            application.Status = ApplicationConstants.ApplicationStatuses.Approved;
            application.DecidedOn = DateTime.UtcNow;
            application.DecisionBy = approvedBy;
            application.LastModifiedOn = DateTime.UtcNow;
            application.LastModifiedBy = approvedBy;

            _dbContext.RentalApplications.Update(application);

            var prospective = await _dbContext.ProspectiveTenants.FindAsync(application.ProspectiveTenantId);
            if (prospective != null)
            {
                prospective.Status = ApplicationConstants.ProspectiveStatuses.Approved;
                prospective.LastModifiedOn = DateTime.UtcNow;
                prospective.LastModifiedBy = approvedBy;
                _dbContext.ProspectiveTenants.Update(prospective);
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DenyApplicationAsync(int applicationId, string organizationId, string deniedBy, string reason)
        {
            var application = await GetRentalApplicationByIdAsync(applicationId, organizationId);
            if (application == null) return false;

            application.Status = ApplicationConstants.ApplicationStatuses.Denied;
            application.DecidedOn = DateTime.UtcNow;
            application.DecisionBy = deniedBy;
            application.DenialReason = reason;
            application.LastModifiedOn = DateTime.UtcNow;
            application.LastModifiedBy = deniedBy;

            var prospective = await _dbContext.ProspectiveTenants.FindAsync(application.ProspectiveTenantId);
            if (prospective != null)
            {
                prospective.Status = ApplicationConstants.ProspectiveStatuses.Denied;
                prospective.LastModifiedOn = DateTime.UtcNow;
                prospective.LastModifiedBy = deniedBy;
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> WithdrawApplicationAsync(int applicationId, string organizationId, string withdrawnBy, string? reason = null)
        {
            var application = await GetRentalApplicationByIdAsync(applicationId, organizationId);
            if (application == null) return false;

            application.Status = ApplicationConstants.ApplicationStatuses.Withdrawn;
            application.DecidedOn = DateTime.UtcNow;
            application.DecisionBy = withdrawnBy;
            application.DenialReason = reason; // Reusing this field for withdrawal reason
            application.LastModifiedOn = DateTime.UtcNow;
            application.LastModifiedBy = withdrawnBy;

            var prospective = await _dbContext.ProspectiveTenants.FindAsync(application.ProspectiveTenantId);
            if (prospective != null)
            {
                prospective.Status = ApplicationConstants.ProspectiveStatuses.Withdrawn;
                prospective.LastModifiedOn = DateTime.UtcNow;
                prospective.LastModifiedBy = withdrawnBy;
            }

            // If there's a lease offer, mark it as withdrawn too
            var leaseOffer = await GetLeaseOfferByApplicationIdAsync(applicationId, organizationId);
            if (leaseOffer != null)
            {
                leaseOffer.Status = "Withdrawn";
                leaseOffer.RespondedOn = DateTime.UtcNow;
                leaseOffer.ResponseNotes = reason ?? "Application withdrawn";
                leaseOffer.LastModifiedOn = DateTime.UtcNow;
                leaseOffer.LastModifiedBy = withdrawnBy;
            }

            // Update property status back to available if it was in lease pending
            var property = await _dbContext.Properties.FindAsync(application.PropertyId);
            if (property != null && property.Status == ApplicationConstants.PropertyStatuses.LeasePending)
            {
                property.Status = ApplicationConstants.PropertyStatuses.Available;
                property.LastModifiedOn = DateTime.UtcNow;
                property.LastModifiedBy = withdrawnBy;
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<ProspectiveTenant>> GetProspectivesByStatusAsync(string status, string organizationId)
        {
            return await _dbContext.ProspectiveTenants
                .Where(pt => pt.Status == status && pt.OrganizationId == organizationId && !pt.IsDeleted)
                .Include(pt => pt.InterestedProperty)
                .OrderByDescending(pt => pt.CreatedOn)
                .ToListAsync();
        }

        public async Task<List<Tour>> GetUpcomingToursAsync(string organizationId, int days = 7)
        {
            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddDays(days);

            return await _dbContext.Tours
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

        public async Task<List<RentalApplication>> GetPendingApplicationsAsync(string organizationId)
        {
            return await _dbContext.RentalApplications
                .Where(ra => ra.OrganizationId == organizationId 
                    && !ra.IsDeleted 
                    && (ra.Status == ApplicationConstants.ApplicationStatuses.Submitted 
                        || ra.Status == ApplicationConstants.ApplicationStatuses.UnderReview
                        || ra.Status == ApplicationConstants.ApplicationStatuses.Screening))
                .Include(ra => ra.ProspectiveTenant)
                .Include(ra => ra.Property)
                .Include(ra => ra.Screening)
                .OrderBy(ra => ra.AppliedOn)
                .ToListAsync();
        }

        #endregion

        #region Lease Offers

        public async Task<LeaseOffer?> CreateLeaseOfferAsync(LeaseOffer leaseOffer)
        {
            _dbContext.LeaseOffers.Add(leaseOffer);
            await _dbContext.SaveChangesAsync();
            return leaseOffer;
        }

        public async Task<LeaseOffer?> GetLeaseOfferByIdAsync(int leaseOfferId, string organizationId)
        {
            return await _dbContext.LeaseOffers
                .Include(lo => lo.RentalApplication)
                .Include(lo => lo.Property)
                .Include(lo => lo.ProspectiveTenant)
                .FirstOrDefaultAsync(lo => lo.Id == leaseOfferId && lo.OrganizationId == Guid.Parse(organizationId) && !lo.IsDeleted);
        }

        public async Task<LeaseOffer?> GetLeaseOfferByApplicationIdAsync(int applicationId, string organizationId)
        {
            return await _dbContext.LeaseOffers
                .Include(lo => lo.RentalApplication)
                .Include(lo => lo.Property)
                .Include(lo => lo.ProspectiveTenant)
                .FirstOrDefaultAsync(lo => lo.RentalApplicationId == applicationId && lo.OrganizationId == Guid.Parse(organizationId) && !lo.IsDeleted);
        }

        public async Task<List<LeaseOffer>> GetLeaseOffersByPropertyIdAsync(int propertyId, string organizationId)
        {
            return await _dbContext.LeaseOffers
                .Include(lo => lo.RentalApplication)
                .Include(lo => lo.Property)
                .Include(lo => lo.ProspectiveTenant)
                .Where(lo => lo.PropertyId == propertyId && lo.OrganizationId == Guid.Parse(organizationId) && !lo.IsDeleted)
                .OrderByDescending(lo => lo.OfferedOn)
                .ToListAsync();
        }

        public async Task<LeaseOffer> UpdateLeaseOfferAsync(LeaseOffer leaseOffer)
        {
            leaseOffer.LastModifiedOn = DateTime.UtcNow;
            _dbContext.LeaseOffers.Update(leaseOffer);
            await _dbContext.SaveChangesAsync();
            return leaseOffer;
        }

        #endregion

        #endregion
   }
}