using Aquiis.SimpleStart.Components.PropertyManagement.Reports;
using Aquiis.SimpleStart.Data;
using Microsoft.EntityFrameworkCore;

namespace Aquiis.SimpleStart.Services;

public class FinancialReportService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public FinancialReportService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    /// <summary>
    /// Generate income statement for a specific period and optional property
    /// </summary>
    public async Task<IncomeStatement> GenerateIncomeStatementAsync(
        string organizationId, 
        DateTime startDate, 
        DateTime endDate, 
        int? propertyId = null)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var statement = new IncomeStatement
        {
            StartDate = startDate,
            EndDate = endDate,
            PropertyId = propertyId
        };

        // Get property name if filtering by property
        if (propertyId.HasValue)
        {
            var property = await context.Properties
                .Where(p => p.Id == propertyId.Value && p.OrganizationId == organizationId)
                .FirstOrDefaultAsync();
            statement.PropertyName = property?.Address;
        }

        // Calculate total rent income from payments (all payments are rent payments)
        var paymentsQuery = context.Payments
            .Include(p => p.Invoice)
            .ThenInclude(i => i.Lease)
            .Where(p => p.Invoice.Lease.Property.OrganizationId == organizationId &&
                       p.PaidOn >= startDate &&
                       p.PaidOn <= endDate);

        if (propertyId.HasValue)
        {
            paymentsQuery = paymentsQuery.Where(p => p.Invoice.Lease.PropertyId == propertyId.Value);
        }

        var totalPayments = await paymentsQuery.SumAsync(p => p.Amount);
        statement.TotalRentIncome = totalPayments;
        statement.TotalOtherIncome = 0; // No other income tracked currently

        // Get maintenance expenses (this is the ONLY expense type tracked)
        var maintenanceQuery = context.MaintenanceRequests
            .Where(m => m.CompletedOn.HasValue &&
                       m.CompletedOn.Value >= startDate &&
                       m.CompletedOn.Value <= endDate &&
                       m.Status == "Completed" &&
                       m.ActualCost > 0);

        if (propertyId.HasValue)
        {
            maintenanceQuery = maintenanceQuery.Where(m => m.PropertyId == propertyId.Value);
        }
        else
        {
            // For all properties, need to filter by user's properties
            var userPropertyIds = await context.Properties
                .Where(p => p.OrganizationId == organizationId)
                .Select(p => p.Id)
                .ToListAsync();
            maintenanceQuery = maintenanceQuery.Where(m => userPropertyIds.Contains(m.PropertyId));
        }

        var maintenanceRequests = await maintenanceQuery.ToListAsync();

        // All maintenance costs go to MaintenanceExpenses
        statement.MaintenanceExpenses = maintenanceRequests.Sum(m => m.ActualCost);
        
        // Other expense categories are currently zero (no data tracked for these yet)
        statement.UtilityExpenses = 0;
        statement.InsuranceExpenses = 0;
        statement.TaxExpenses = 0;
        statement.ManagementFees = 0;
        statement.OtherExpenses = 0;

        return statement;
    }

    /// <summary>
    /// Generate rent roll report showing all properties and tenants
    /// </summary>
    public async Task<List<RentRollItem>> GenerateRentRollAsync(string organizationId, DateTime asOfDate)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var rentRoll = await context.Leases
            .Include(l => l.Property)
            .Include(l => l.Tenant)
            .Include(l => l.Invoices)
            .ThenInclude(i => i.Payments)
            .Where(l => l.Property.OrganizationId == organizationId &&
                       l.StartDate <= asOfDate &&
                       l.EndDate >= asOfDate)
            .OrderBy(l => l.Property.Address)
            .ThenBy(l => l.Tenant.LastName)
            .Select(l => new RentRollItem
            {
                PropertyId = l.PropertyId,
                PropertyName = l.Property.Address,
                PropertyAddress = l.Property.Address,
                TenantId = l.TenantId,
                TenantName = l.Tenant != null ? $"{l.Tenant.FirstName} {l.Tenant.LastName}" : "Vacant",
                LeaseStatus = l.Status,
                LeaseStartDate = l.StartDate,
                LeaseEndDate = l.EndDate,
                MonthlyRent = l.MonthlyRent,
                SecurityDeposit = l.SecurityDeposit,
                TotalPaid = l.Invoices.SelectMany(i => i.Payments).Sum(p => p.Amount),
                TotalDue = l.Invoices.Where(i => i.Status != "Cancelled").Sum(i => i.Amount)
            })
            .ToListAsync();

        return rentRoll;
    }

    /// <summary>
    /// Generate property performance comparison report
    /// </summary>
    public async Task<List<PropertyPerformance>> GeneratePropertyPerformanceAsync(
        string organizationId, 
        DateTime startDate, 
        DateTime endDate)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var properties = await context.Properties
            .Where(p => p.OrganizationId == organizationId)
            .ToListAsync();

        var performance = new List<PropertyPerformance>();
        var totalDays = (endDate - startDate).Days + 1;

        foreach (var property in properties)
        {
            // Calculate income from rent payments
            var income = await context.Payments
                .Include(p => p.Invoice)
                .ThenInclude(i => i.Lease)
                .Where(p => p.Invoice.Lease.PropertyId == property.Id &&
                           p.PaidOn >= startDate &&
                           p.PaidOn <= endDate)
                .SumAsync(p => p.Amount);

            // Calculate expenses from maintenance requests only
            var expenses = await context.MaintenanceRequests
                .Where(m => m.PropertyId == property.Id &&
                           m.CompletedOn.HasValue &&
                           m.CompletedOn.Value >= startDate &&
                           m.CompletedOn.Value <= endDate &&
                           m.Status == "Completed" &&
                           m.ActualCost > 0)
                .SumAsync(m => m.ActualCost);

            // Calculate occupancy days
            var leases = await context.Leases
                .Where(l => l.PropertyId == property.Id &&
                           l.Status == "Active" &&
                           l.StartDate <= endDate &&
                           l.EndDate >= startDate)
                .ToListAsync();

            var occupancyDays = 0;
            foreach (var lease in leases)
            {
                var leaseStart = lease.StartDate > startDate ? lease.StartDate : startDate;
                var leaseEnd = lease.EndDate < endDate ? lease.EndDate : endDate;
                if (leaseEnd >= leaseStart)
                {
                    occupancyDays += (leaseEnd - leaseStart).Days + 1;
                }
            }

            // Calculate ROI (simplified - based on profit margin since we don't track purchase price)
            var roi = income > 0 
                ? ((income - expenses) / income) * 100 
                : 0;

            performance.Add(new PropertyPerformance
            {
                PropertyId = property.Id,
                PropertyName = property.Address,
                PropertyAddress = property.Address,
                TotalIncome = income,
                TotalExpenses = expenses,
                ROI = roi,
                OccupancyDays = occupancyDays,
                TotalDays = totalDays
            });
        }

        return performance.OrderByDescending(p => p.NetIncome).ToList();
    }

    /// <summary>
    /// Generate tax report data for Schedule E
    /// </summary>
    public async Task<List<TaxReportData>> GenerateTaxReportAsync(string organizationId, int year, int? propertyId = null)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var startDate = new DateTime(year, 1, 1);
        var endDate = new DateTime(year, 12, 31);

        var propertiesQuery = context.Properties.Where(p => p.OrganizationId == organizationId);
        if (propertyId.HasValue)
        {
            propertiesQuery = propertiesQuery.Where(p => p.Id == propertyId.Value);
        }

        var properties = await propertiesQuery.ToListAsync();
        var taxReports = new List<TaxReportData>();

        foreach (var property in properties)
        {
            // Calculate rent income from payments
            var rentIncome = await context.Payments
                .Include(p => p.Invoice)
                .ThenInclude(i => i.Lease)
                .Where(p => p.Invoice.Lease.PropertyId == property.Id &&
                           p.PaidOn >= startDate &&
                           p.PaidOn <= endDate)
                .SumAsync(p => p.Amount);

            // Get maintenance expenses (this is the only expense type currently tracked)
            var maintenanceExpenses = await context.MaintenanceRequests
                .Where(m => m.PropertyId == property.Id &&
                           m.CompletedOn.HasValue &&
                           m.CompletedOn.Value >= startDate &&
                           m.CompletedOn.Value <= endDate &&
                           m.Status == "Completed" &&
                           m.ActualCost > 0)
                .ToListAsync();

            // Calculate depreciation (simplified - 27.5 years for residential rental)
            // Note: Since we don't track purchase price, this should be manually entered
            var depreciationAmount = 0m;

            var totalMaintenanceCost = maintenanceExpenses.Sum(m => m.ActualCost);

            var taxReport = new TaxReportData
            {
                Year = year,
                PropertyId = property.Id,
                PropertyName = property.Address,
                TotalRentIncome = rentIncome,
                DepreciationAmount = depreciationAmount,
                
                // Currently only maintenance/repairs are tracked
                Advertising = 0,
                Cleaning = 0,
                Insurance = 0,
                Legal = 0,
                Management = 0,
                MortgageInterest = 0,
                Repairs = totalMaintenanceCost, // All maintenance costs
                Supplies = 0,
                Taxes = 0,
                Utilities = 0,
                Other = 0
            };

            taxReport.TotalExpenses = totalMaintenanceCost;

            taxReports.Add(taxReport);
        }

        return taxReports;
    }
}
