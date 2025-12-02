using Microsoft.EntityFrameworkCore;
using Aquiis.SimpleStart.Infrastructure.Data;
using Aquiis.SimpleStart.Core.Entities;
using Aquiis.SimpleStart.Core.Constants;
using Aquiis.SimpleStart.Shared.Services;

namespace Aquiis.SimpleStart.Application.Services
{
    /// <summary>
    /// Service for managing security deposits, investment pool, and dividend distribution.
    /// Handles the complete lifecycle from collection to refund with investment tracking.
    /// </summary>
    public class SecurityDepositService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserContextService _userContext;

        public SecurityDepositService(ApplicationDbContext context, UserContextService userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        #region Security Deposit Management

        /// <summary>
        /// Collects a security deposit for a lease.
        /// </summary>
        public async Task<SecurityDeposit> CollectSecurityDepositAsync(
            int leaseId,
            decimal amount,
            string paymentMethod,
            string? transactionReference,
            string userId,
            int? tenantId = null)
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();
            if (organizationId == null)
                throw new InvalidOperationException("Organization context is required");

            var lease = await _context.Leases
                .Include(l => l.Tenant)
                .FirstOrDefaultAsync(l => l.Id == leaseId && !l.IsDeleted);

            if (lease == null)
                throw new InvalidOperationException($"Lease {leaseId} not found");

            // Check if deposit already exists for this lease
            var existingDeposit = await _context.SecurityDeposits
                .FirstOrDefaultAsync(sd => sd.LeaseId == leaseId && !sd.IsDeleted);

            if (existingDeposit != null)
                throw new InvalidOperationException($"Security deposit already exists for lease {leaseId}");

            // Use provided tenantId or fall back to lease.TenantId
            int depositTenantId;
            if (tenantId.HasValue)
            {
                depositTenantId = tenantId.Value;
            }
            else if (lease.TenantId > 0)
            {
                depositTenantId = lease.TenantId;
            }
            else
            {
                throw new InvalidOperationException($"Tenant ID is required to collect security deposit for lease {leaseId}");
            }

            var deposit = new SecurityDeposit
            {
                OrganizationId = organizationId,
                LeaseId = leaseId,
                TenantId = depositTenantId,
                Amount = amount,
                DateReceived = DateTime.UtcNow,
                PaymentMethod = paymentMethod,
                TransactionReference = transactionReference,
                Status = ApplicationConstants.SecurityDepositStatuses.Held,
                InInvestmentPool = false, // Will be added when lease becomes active
                CreatedBy = userId,
                CreatedOn = DateTime.UtcNow
            };

            _context.SecurityDeposits.Add(deposit);
            await _context.SaveChangesAsync();

            return deposit;
        }

        /// <summary>
        /// Adds a security deposit to the investment pool when lease becomes active.
        /// </summary>
        public async Task<bool> AddToInvestmentPoolAsync(int securityDepositId, string userId)
        {
            var deposit = await _context.SecurityDeposits
                .Include(sd => sd.Lease)
                .FirstOrDefaultAsync(sd => sd.Id == securityDepositId && !sd.IsDeleted);

            if (deposit == null)
                return false;

            if (deposit.InInvestmentPool)
                return true; // Already in pool

            deposit.InInvestmentPool = true;
            deposit.PoolEntryDate = DateTime.UtcNow;
            deposit.LastModifiedBy = userId;
            deposit.LastModifiedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Removes a security deposit from the investment pool when lease ends.
        /// </summary>
        public async Task<bool> RemoveFromInvestmentPoolAsync(int securityDepositId, string userId)
        {
            var deposit = await _context.SecurityDeposits
                .FirstOrDefaultAsync(sd => sd.Id == securityDepositId && !sd.IsDeleted);

            if (deposit == null)
                return false;

            if (!deposit.InInvestmentPool)
                return true; // Already removed

            deposit.InInvestmentPool = false;
            deposit.PoolExitDate = DateTime.UtcNow;
            deposit.LastModifiedBy = userId;
            deposit.LastModifiedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Gets security deposit by lease ID.
        /// </summary>
        public async Task<SecurityDeposit?> GetSecurityDepositByLeaseIdAsync(int leaseId)
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();

            return await _context.SecurityDeposits
                .Include(sd => sd.Lease)
                .Include(sd => sd.Tenant)
                .Include(sd => sd.Dividends)
                .Where(sd => !sd.IsDeleted && 
                            sd.OrganizationId == organizationId &&
                            sd.LeaseId == leaseId)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets all security deposits for an organization.
        /// </summary>
        public async Task<List<SecurityDeposit>> GetSecurityDepositsAsync(string? status = null)
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();
            
            if (string.IsNullOrEmpty(organizationId))
                return new List<SecurityDeposit>();

            // Filter by OrganizationId (stored as string, consistent with Property/Tenant models)
            var query = _context.SecurityDeposits
                .Where(sd => sd.OrganizationId == organizationId && !sd.IsDeleted);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(sd => sd.Status == status);

            // Load navigation properties
            var deposits = await query
                .Include(sd => sd.Lease)
                    .ThenInclude(l => l.Property)
                .Include(sd => sd.Tenant)
                .Include(sd => sd.Dividends)
                .OrderByDescending(sd => sd.DateReceived)
                .ToListAsync();
            
            return deposits;
        }

        /// <summary>
        /// Gets all security deposits that were in the investment pool during a specific year.
        /// </summary>
        public async Task<List<SecurityDeposit>> GetSecurityDepositsInPoolAsync(int year)
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();
            var yearStart = new DateTime(year, 1, 1);
            var yearEnd = new DateTime(year, 12, 31);

            return await _context.SecurityDeposits
                .Include(sd => sd.Lease)
                    .ThenInclude(l => l.Property)
                .Include(sd => sd.Tenant)
                .Include(sd => sd.Dividends)
                .Where(sd => !sd.IsDeleted && 
                            sd.OrganizationId == organizationId &&
                            sd.InInvestmentPool &&
                            sd.PoolEntryDate.HasValue &&
                            sd.PoolEntryDate.Value <= yearEnd &&
                            (!sd.PoolExitDate.HasValue || sd.PoolExitDate.Value >= yearStart))
                .OrderBy(sd => sd.PoolEntryDate)
                .ToListAsync();
        }

        #endregion

        #region Investment Pool Management

        /// <summary>
        /// Creates or gets the investment pool for a specific year.
        /// </summary>
        public async Task<SecurityDepositInvestmentPool> GetOrCreateInvestmentPoolAsync(int year, string userId)
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();
            if (organizationId == null)
                throw new InvalidOperationException("Organization context is required");

            var pool = await _context.SecurityDepositInvestmentPools
                .FirstOrDefaultAsync(p => p.Year == year && 
                                         p.OrganizationId == organizationId &&
                                         !p.IsDeleted);

            if (pool != null)
                return pool;

            // Get organization settings for default share percentage
            var settings = await _context.OrganizationSettings
                .FirstOrDefaultAsync(s => s.OrganizationId == Guid.Parse(organizationId) && !s.IsDeleted);

            pool = new SecurityDepositInvestmentPool
            {
                OrganizationId = organizationId,
                Year = year,
                StartingBalance = 0,
                EndingBalance = 0,
                TotalEarnings = 0,
                ReturnRate = 0,
                OrganizationSharePercentage = settings?.OrganizationSharePercentage ?? 0.20m,
                OrganizationShare = 0,
                TenantShareTotal = 0,
                ActiveLeaseCount = 0,
                DividendPerLease = 0,
                Status = ApplicationConstants.InvestmentPoolStatuses.Open,
                CreatedBy = userId,
                CreatedOn = DateTime.UtcNow
            };

            _context.SecurityDepositInvestmentPools.Add(pool);
            await _context.SaveChangesAsync();

            return pool;
        }

        /// <summary>
        /// Records annual investment performance for the pool.
        /// </summary>
        public async Task<SecurityDepositInvestmentPool> RecordInvestmentPerformanceAsync(
            int year,
            decimal startingBalance,
            decimal endingBalance,
            decimal totalEarnings,
            string userId)
        {
            var pool = await GetOrCreateInvestmentPoolAsync(year, userId);

            pool.StartingBalance = startingBalance;
            pool.EndingBalance = endingBalance;
            pool.TotalEarnings = totalEarnings;
            pool.ReturnRate = startingBalance > 0 ? totalEarnings / startingBalance : 0;

            // Calculate organization and tenant shares
            if (totalEarnings > 0)
            {
                pool.OrganizationShare = totalEarnings * pool.OrganizationSharePercentage;
                pool.TenantShareTotal = totalEarnings - pool.OrganizationShare;
            }
            else
            {
                // Losses absorbed by organization - no negative dividends
                pool.OrganizationShare = 0;
                pool.TenantShareTotal = 0;
            }

            pool.LastModifiedBy = userId;
            pool.LastModifiedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return pool;
        }

        /// <summary>
        /// Calculates dividends for all active deposits in a year.
        /// </summary>
        public async Task<List<SecurityDepositDividend>> CalculateDividendsAsync(int year, string userId)
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();
            if (organizationId == null)
                throw new InvalidOperationException("Organization context is required");

            var pool = await GetOrCreateInvestmentPoolAsync(year, userId);

            // Get all deposits that were in the pool during this year
            var yearStart = new DateTime(year, 1, 1);
            var yearEnd = new DateTime(year, 12, 31);

            var activeDeposits = await _context.SecurityDeposits
                .Include(sd => sd.Lease)
                .Include(sd => sd.Tenant)
                .Where(sd => !sd.IsDeleted &&
                            sd.OrganizationId == organizationId &&
                            sd.InInvestmentPool &&
                            sd.PoolEntryDate.HasValue &&
                            sd.PoolEntryDate.Value <= yearEnd &&
                            (!sd.PoolExitDate.HasValue || sd.PoolExitDate.Value >= yearStart))
                .ToListAsync();

            if (!activeDeposits.Any() || pool.TenantShareTotal <= 0)
            {
                pool.ActiveLeaseCount = 0;
                pool.DividendPerLease = 0;
                pool.Status = ApplicationConstants.InvestmentPoolStatuses.Calculated;
                pool.DividendsCalculatedOn = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return new List<SecurityDepositDividend>();
            }

            pool.ActiveLeaseCount = activeDeposits.Count;
            pool.DividendPerLease = pool.TenantShareTotal / pool.ActiveLeaseCount;

            var dividends = new List<SecurityDepositDividend>();

            // Get default payment method from settings
            var settings = await _context.OrganizationSettings
                .FirstOrDefaultAsync(s => s.OrganizationId == Guid.Parse(organizationId) && !s.IsDeleted);

            var defaultPaymentMethod = settings?.AllowTenantDividendChoice == true
                ? ApplicationConstants.DividendPaymentMethods.Pending
                : (settings?.DefaultDividendPaymentMethod ?? ApplicationConstants.DividendPaymentMethods.LeaseCredit);

            foreach (var deposit in activeDeposits)
            {
                // Check if dividend already exists
                var existingDividend = await _context.SecurityDepositDividends
                    .FirstOrDefaultAsync(d => d.SecurityDepositId == deposit.Id && 
                                            d.Year == year && 
                                            !d.IsDeleted);

                if (existingDividend != null)
                {
                    dividends.Add(existingDividend);
                    continue;
                }

                // Calculate pro-ration factor based on months in pool
                if (!deposit.PoolEntryDate.HasValue)
                    continue; // Skip if no entry date

                var effectiveStart = deposit.PoolEntryDate.Value > yearStart 
                    ? deposit.PoolEntryDate.Value 
                    : yearStart;

                var effectiveEnd = deposit.PoolExitDate.HasValue && deposit.PoolExitDate.Value < yearEnd
                    ? deposit.PoolExitDate.Value
                    : yearEnd;

                var monthsInPool = ((effectiveEnd.Year - effectiveStart.Year) * 12) + 
                                  effectiveEnd.Month - effectiveStart.Month + 1;

                var prorationFactor = Math.Min(monthsInPool / 12.0m, 1.0m);

                var dividend = new SecurityDepositDividend
                {
                    OrganizationId = organizationId,
                    SecurityDepositId = deposit.Id,
                    InvestmentPoolId = pool.Id,
                    LeaseId = deposit.LeaseId,
                    TenantId = deposit.TenantId,
                    Year = year,
                    BaseDividendAmount = pool.DividendPerLease,
                    ProrationFactor = prorationFactor,
                    DividendAmount = pool.DividendPerLease * prorationFactor,
                    PaymentMethod = defaultPaymentMethod,
                    Status = ApplicationConstants.DividendStatuses.Pending,
                    MonthsInPool = monthsInPool,
                    CreatedBy = userId,
                    CreatedOn = DateTime.UtcNow
                };

                _context.SecurityDepositDividends.Add(dividend);
                dividends.Add(dividend);
            }

            pool.Status = ApplicationConstants.InvestmentPoolStatuses.Calculated;
            pool.DividendsCalculatedOn = DateTime.UtcNow;
            pool.LastModifiedBy = userId;
            pool.LastModifiedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return dividends;
        }

        /// <summary>
        /// Gets investment pool by year.
        /// </summary>
        public async Task<SecurityDepositInvestmentPool?> GetInvestmentPoolByYearAsync(int year)
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();

            return await _context.SecurityDepositInvestmentPools
                .Include(p => p.Dividends)
                .FirstOrDefaultAsync(p => p.Year == year && 
                                         p.OrganizationId == organizationId &&
                                         !p.IsDeleted);
        }

        /// <summary>
        /// Gets an investment pool by ID.
        /// </summary>
        public async Task<SecurityDepositInvestmentPool?> GetInvestmentPoolByIdAsync(int poolId)
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();

            return await _context.SecurityDepositInvestmentPools
                .Include(p => p.Dividends)
                .FirstOrDefaultAsync(p => p.Id == poolId && 
                                         p.OrganizationId == organizationId &&
                                         !p.IsDeleted);
        }

        /// <summary>
        /// Gets all investment pools for an organization.
        /// </summary>
        public async Task<List<SecurityDepositInvestmentPool>> GetInvestmentPoolsAsync()
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();

            return await _context.SecurityDepositInvestmentPools
                .Include(p => p.Dividends)
                .Where(p => !p.IsDeleted && p.OrganizationId == organizationId)
                .OrderByDescending(p => p.Year)
                .ToListAsync();
        }

        #endregion

        #region Dividend Management

        /// <summary>
        /// Records tenant's payment method choice for dividend.
        /// </summary>
        public async Task<bool> RecordDividendChoiceAsync(
            int dividendId,
            string paymentMethod,
            string? mailingAddress,
            string userId)
        {
            var dividend = await _context.SecurityDepositDividends
                .FirstOrDefaultAsync(d => d.Id == dividendId && !d.IsDeleted);

            if (dividend == null)
                return false;

            dividend.PaymentMethod = paymentMethod;
            dividend.MailingAddress = mailingAddress;
            dividend.ChoiceMadeOn = DateTime.UtcNow;
            dividend.Status = ApplicationConstants.DividendStatuses.ChoiceMade;
            dividend.LastModifiedBy = userId;
            dividend.LastModifiedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Processes dividend payment (applies as credit or marks as paid).
        /// </summary>
        public async Task<bool> ProcessDividendPaymentAsync(
            int dividendId,
            string? paymentReference,
            string userId)
        {
            var dividend = await _context.SecurityDepositDividends
                .Include(d => d.Lease)
                .FirstOrDefaultAsync(d => d.Id == dividendId && !d.IsDeleted);

            if (dividend == null)
                return false;

            dividend.PaymentReference = paymentReference;
            dividend.PaymentProcessedOn = DateTime.UtcNow;
            dividend.Status = dividend.PaymentMethod == ApplicationConstants.DividendPaymentMethods.LeaseCredit
                ? ApplicationConstants.DividendStatuses.Applied
                : ApplicationConstants.DividendStatuses.Paid;
            dividend.LastModifiedBy = userId;
            dividend.LastModifiedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Gets dividends for a specific tenant.
        /// </summary>
        public async Task<List<SecurityDepositDividend>> GetTenantDividendsAsync(int tenantId)
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();

            return await _context.SecurityDepositDividends
                .Include(d => d.InvestmentPool)
                .Include(d => d.Lease)
                    .ThenInclude(l => l.Property)
                .Where(d => !d.IsDeleted &&
                           d.OrganizationId == organizationId &&
                           d.TenantId == tenantId)
                .OrderByDescending(d => d.Year)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all dividends for a specific year.
        /// </summary>
        public async Task<List<SecurityDepositDividend>> GetDividendsByYearAsync(int year)
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();

            return await _context.SecurityDepositDividends
                .Include(d => d.InvestmentPool)
                .Include(d => d.SecurityDeposit)
                .Include(d => d.Lease)
                    .ThenInclude(l => l.Property)
                .Include(d => d.Tenant)
                .Where(d => !d.IsDeleted &&
                           d.OrganizationId == organizationId &&
                           d.Year == year)
                .OrderBy(d => d.Tenant.LastName)
                .ThenBy(d => d.Tenant.FirstName)
                .ToListAsync();
        }

        #endregion

        #region Refund Processing

        /// <summary>
        /// Calculates total refund amount (deposit + dividends - deductions).
        /// </summary>
        public async Task<decimal> CalculateRefundAmountAsync(
            int securityDepositId,
            decimal deductionsAmount)
        {
            var deposit = await _context.SecurityDeposits
                .Include(sd => sd.Dividends.Where(d => !d.IsDeleted))
                .FirstOrDefaultAsync(sd => sd.Id == securityDepositId && !sd.IsDeleted);

            if (deposit == null)
                return 0;

            var totalDividends = deposit.Dividends
                .Where(d => d.Status == ApplicationConstants.DividendStatuses.Applied ||
                           d.Status == ApplicationConstants.DividendStatuses.Paid)
                .Sum(d => d.DividendAmount);

            return deposit.Amount + totalDividends - deductionsAmount;
        }

        /// <summary>
        /// Processes security deposit refund.
        /// </summary>
        public async Task<SecurityDeposit> ProcessRefundAsync(
            int securityDepositId,
            decimal deductionsAmount,
            string? deductionsReason,
            string refundMethod,
            string? refundReference,
            string userId)
        {
            var deposit = await _context.SecurityDeposits
                .Include(sd => sd.Dividends)
                .FirstOrDefaultAsync(sd => sd.Id == securityDepositId && !sd.IsDeleted);

            if (deposit == null)
                throw new InvalidOperationException($"Security deposit {securityDepositId} not found");

            if (deposit.IsRefunded)
                throw new InvalidOperationException($"Security deposit {securityDepositId} has already been refunded");

            // Remove from pool if still in it
            if (deposit.InInvestmentPool)
            {
                await RemoveFromInvestmentPoolAsync(securityDepositId, userId);
            }

            var refundAmount = await CalculateRefundAmountAsync(securityDepositId, deductionsAmount);

            deposit.DeductionsAmount = deductionsAmount;
            deposit.DeductionsReason = deductionsReason;
            deposit.RefundAmount = refundAmount;
            deposit.RefundMethod = refundMethod;
            deposit.RefundReference = refundReference;
            deposit.RefundProcessedDate = DateTime.UtcNow;
            deposit.Status = refundAmount < deposit.Amount
                ? ApplicationConstants.SecurityDepositStatuses.PartiallyRefunded
                : ApplicationConstants.SecurityDepositStatuses.Refunded;
            deposit.LastModifiedBy = userId;
            deposit.LastModifiedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return deposit;
        }

        /// <summary>
        /// Gets security deposits pending refund (lease ended, not yet refunded).
        /// </summary>
        public async Task<List<SecurityDeposit>> GetPendingRefundsAsync()
        {
            var organizationId = await _userContext.GetOrganizationIdAsync();

            return await _context.SecurityDeposits
                .Include(sd => sd.Lease)
                    .ThenInclude(l => l.Property)
                .Include(sd => sd.Tenant)
                .Include(sd => sd.Dividends)
                .Where(sd => !sd.IsDeleted &&
                            sd.OrganizationId == organizationId &&
                            sd.Status == ApplicationConstants.SecurityDepositStatuses.Held &&
                            sd.Lease.EndDate < DateTime.UtcNow)
                .OrderBy(sd => sd.Lease.EndDate)
                .ToListAsync();
        }

        /// <summary>
        /// Closes an investment pool, marking it as complete.
        /// </summary>
        public async Task<SecurityDepositInvestmentPool> CloseInvestmentPoolAsync(int poolId, string userId)
        {
            var pool = await _context.SecurityDepositInvestmentPools
                .FirstOrDefaultAsync(p => p.Id == poolId && !p.IsDeleted);

            if (pool == null)
                throw new InvalidOperationException($"Investment pool {poolId} not found");

            pool.Status = ApplicationConstants.InvestmentPoolStatuses.Closed;
            pool.LastModifiedBy = userId;
            pool.LastModifiedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return pool;
        }

        #endregion
    }
}
