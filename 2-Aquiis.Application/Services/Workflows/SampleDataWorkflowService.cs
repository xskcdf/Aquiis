using Aquiis.Application.Services.Workflows;
using Aquiis.Core.Constants;
using Aquiis.Core.Entities;
using Aquiis.Core.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Aquiis.Application.Services.Workflows
{
    /// <summary>
    /// Workflow service for generating sample test data.
    /// Creates properties, tenants, leases, invoices, and payments for testing and demos.
    /// </summary>
    public class SampleDataWorkflowService : BaseWorkflowService
    {
        private readonly ILogger<SampleDataWorkflowService> _logger;
        private readonly InvoiceService _invoiceService;
        private readonly PaymentService _paymentService;
        private readonly Random _random;

        public SampleDataWorkflowService(
            ApplicationDbContext context,
            IUserContextService userContext,
            InvoiceService invoiceService,
            PaymentService paymentService,
            ILogger<SampleDataWorkflowService> logger) : base(context, userContext)
        {
            _logger = logger;
            _invoiceService = invoiceService;
            _paymentService = paymentService;
            _random = new Random(DateTime.Now.Millisecond); // Seed for varied data
        }

        /// <summary>
        /// Main orchestration method - generates complete sample dataset.
        /// </summary>
        public async Task<WorkflowResult> GenerateSampleDataAsync()
        {
            return await ExecuteWorkflowAsync(async () =>
            {
                _logger.LogInformation("Starting sample data generation...");

                try
                {
                    // Get context
                    var orgId = await GetActiveOrganizationIdAsync();

                    if (orgId == Guid.Empty)
                    {
                        return WorkflowResult.Fail("Organization context not available. Please ensure you are logged in.");
                    }

                    // Use SystemUser.Id for test data identification
                    var systemUserId = ApplicationConstants.SystemUser.Id;
                    _logger.LogInformation($"Generating sample data for Organization: {orgId}, CreatedBy: {systemUserId}");

                    // Generate entities in proper order (respecting dependencies)
                    var properties = await GeneratePropertiesAsync(orgId, systemUserId);
                    _logger.LogInformation($"Created {properties.Count} properties");

                    var tenants = await GenerateTenantsAsync(orgId, systemUserId);
                    _logger.LogInformation($"Created {tenants.Count} tenants");

                    var leases = await GenerateLeasesAsync(properties, tenants, orgId, systemUserId);
                    _logger.LogInformation($"Created {leases.Count} leases");

                    var invoices = await GenerateInvoicesAsync(leases, orgId, systemUserId);
                    _logger.LogInformation($"Created {invoices.Count} invoices");

                    var payments = await GeneratePaymentsAsync(invoices, orgId, systemUserId);
                    _logger.LogInformation($"Created {payments.Count} payments");

                    // Log workflow completion
                    await LogTransitionAsync(
                        entityType: "SampleData",
                        entityId: orgId,
                        fromStatus: null,
                        toStatus: "Generated",
                        action: "GenerateSampleData",
                        reason: $"Created {properties.Count} properties, {tenants.Count} tenants, {leases.Count} leases, {invoices.Count} invoices, {payments.Count} payments"
                    );

                    return WorkflowResult.Ok(
                        $"Successfully generated sample data: {properties.Count} properties, {tenants.Count} tenants, " +
                        $"{leases.Count} leases, {invoices.Count} invoices, {payments.Count} payments");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error generating sample data");
                    return WorkflowResult.Fail($"Error generating sample data: {ex.Message}");
                }
            });
        }

        /// <summary>
        /// Removes all sample data created with SystemUser.Id.
        /// Deletes properties, tenants, leases, invoices, and payments in proper order.
        /// </summary>
        public async Task<WorkflowResult> RemoveSampleDataAsync()
        {
            return await ExecuteWorkflowAsync(async () =>
            {
                _logger.LogInformation("Starting sample data removal...");

                try
                {
                    var orgId = await GetActiveOrganizationIdAsync();

                    if (orgId == Guid.Empty)
                    {
                        return WorkflowResult.Fail("Organization context not available.");
                    }

                    var systemUserId = ApplicationConstants.SystemUser.Id;
                    _logger.LogInformation($"Removing sample data for Organization: {orgId}, CreatedBy: {systemUserId}");

                    // Delete in reverse order of dependencies
                    var paymentsDeleted = await RemovePaymentsAsync(orgId, systemUserId);
                    _logger.LogInformation($"Deleted {paymentsDeleted} payments");

                    var invoicesDeleted = await RemoveInvoicesAsync(orgId, systemUserId);
                    _logger.LogInformation($"Deleted {invoicesDeleted} invoices");

                    var leasesDeleted = await RemoveLeasesAsync(orgId, systemUserId);
                    _logger.LogInformation($"Deleted {leasesDeleted} leases");

                    var tenantsDeleted = await RemoveTenantsAsync(orgId, systemUserId);
                    _logger.LogInformation($"Deleted {tenantsDeleted} tenants");

                    var propertiesDeleted = await RemovePropertiesAsync(orgId, systemUserId);
                    _logger.LogInformation($"Deleted {propertiesDeleted} properties");

                    // Log workflow completion
                    await LogTransitionAsync(
                        entityType: "SampleData",
                        entityId: orgId,
                        fromStatus: "Generated",
                        toStatus: "Removed",
                        action: "RemoveSampleData",
                        reason: $"Deleted {propertiesDeleted} properties, {tenantsDeleted} tenants, {leasesDeleted} leases, {invoicesDeleted} invoices, {paymentsDeleted} payments"
                    );

                    return WorkflowResult.Ok(
                        $"Successfully removed sample data: {propertiesDeleted} properties, {tenantsDeleted} tenants, " +
                        $"{leasesDeleted} leases, {invoicesDeleted} invoices, {paymentsDeleted} payments");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error removing sample data");
                    return WorkflowResult.Fail($"Error removing sample data: {ex.Message}");
                }
            });
        }

        #region Property Generation

        private async Task<List<Property>> GeneratePropertiesAsync(Guid organizationId, string userId)
        {
            var properties = new List<Property>();
            var now = DateTime.UtcNow;

            // Define 6 properties in Texas with varied characteristics
            var propertyData = new[]
            {
                new { Address = "1234 Riverside Dr", City = "Austin", State = "TX", Zip = "78701", Type = ApplicationConstants.PropertyTypes.House, Beds = 3, Baths = 2.0m, SqFt = 1850, Rent = 1850m, Status = ApplicationConstants.PropertyStatuses.Occupied },
                new { Address = "5678 Oak Street", City = "Houston", State = "TX", Zip = "77002", Type = ApplicationConstants.PropertyTypes.Apartment, Beds = 2, Baths = 2.0m, SqFt = 1200, Rent = 1450m, Status = ApplicationConstants.PropertyStatuses.Occupied },
                new { Address = "910 Maple Ave", City = "Dallas", State = "TX", Zip = "75201", Type = ApplicationConstants.PropertyTypes.House, Beds = 4, Baths = 3.0m, SqFt = 2500, Rent = 2200m, Status = ApplicationConstants.PropertyStatuses.Occupied },
                new { Address = "1122 Pine Ln", City = "San Antonio", State = "TX", Zip = "78205", Type = ApplicationConstants.PropertyTypes.Condo, Beds = 2, Baths = 1.0m, SqFt = 1100, Rent = 1200m, Status = ApplicationConstants.PropertyStatuses.Available },
                new { Address = "3344 Elm Ct", City = "Fort Worth", State = "TX", Zip = "76102", Type = ApplicationConstants.PropertyTypes.House, Beds = 3, Baths = 2.0m, SqFt = 1750, Rent = 1750m, Status = ApplicationConstants.PropertyStatuses.Available },
                new { Address = "5566 Cedar Rd", City = "El Paso", State = "TX", Zip = "79901", Type = ApplicationConstants.PropertyTypes.Apartment, Beds = 1, Baths = 1.0m, SqFt = 850, Rent = 1100m, Status = ApplicationConstants.PropertyStatuses.Available }
            };

            for (int i = 0; i < propertyData.Length; i++)
            {
                var data = propertyData[i];
                var createdDate = GetRandomDate(new DateTime(2025, 4, 1), new DateTime(2025, 6, 30));

                var property = new Property
                {
                    Id = Guid.NewGuid(),
                    OrganizationId = organizationId,
                    Address = data.Address,
                    City = data.City,
                    State = data.State,
                    ZipCode = data.Zip,
                    PropertyType = data.Type,
                    Bedrooms = data.Beds,
                    Bathrooms = data.Baths,
                    SquareFeet = data.SqFt,
                    MonthlyRent = data.Rent,
                    Status = data.Status,
                    IsAvailable = data.Status == ApplicationConstants.PropertyStatuses.Available,
                    Description = $"Beautiful {data.Beds} bedroom, {data.Baths} bath {data.Type.ToLower()} in {data.City}. " +
                                 $"{data.SqFt} square feet with modern amenities and convenient location.",
                    RoutineInspectionIntervalMonths = 12,
                    NextRoutineInspectionDueDate = DateTime.Today.AddMonths(6),
                    CreatedBy = userId,
                    CreatedOn = createdDate,
                    IsDeleted = false
                };

                _context.Properties.Add(property);
                properties.Add(property);
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Generated {properties.Count} properties");

            return properties;
        }

        #endregion

        #region Tenant Generation

        private async Task<List<Tenant>> GenerateTenantsAsync(Guid organizationId, string userId)
        {
            var tenants = new List<Tenant>();

            // Define 3 tenants with realistic data
            var tenantData = new[]
            {
                new { FirstName = "Sarah", LastName = "Johnson", DOB = new DateTime(1988, 5, 15), EmergencyName = "John Johnson", EmergencyPhone = "555-987-6543", Relationship = "Spouse" },
                new { FirstName = "Michael", LastName = "Chen", DOB = new DateTime(1992, 8, 22), EmergencyName = "Lisa Chen", EmergencyPhone = "555-876-5432", Relationship = "Sister" },
                new { FirstName = "Emily", LastName = "Rodriguez", DOB = new DateTime(1990, 3, 10), EmergencyName = "Carlos Rodriguez", EmergencyPhone = "555-765-4321", Relationship = "Father" }
            };

            for (int i = 0; i < tenantData.Length; i++)
            {
                var data = tenantData[i];
                var createdDate = GetRandomDate(new DateTime(2025, 5, 1), new DateTime(2025, 7, 31));

                var tenant = new Tenant
                {
                    Id = Guid.NewGuid(),
                    OrganizationId = organizationId,
                    FirstName = data.FirstName,
                    LastName = data.LastName,
                    Email = $"{data.FirstName.ToLower()}.{data.LastName.ToLower()}@example.com",
                    PhoneNumber = $"555-{_random.Next(100, 999)}-{_random.Next(1000, 9999)}",
                    DateOfBirth = data.DOB,
                    IdentificationNumber = $"DL-{_random.Next(10000000, 99999999)}",
                    IsActive = true,
                    EmergencyContactName = data.EmergencyName,
                    EmergencyContactPhone = data.EmergencyPhone,
                    Notes = $"Emergency contact relationship: {data.Relationship}",
                    CreatedBy = userId,
                    CreatedOn = createdDate,
                    IsDeleted = false
                };

                _context.Tenants.Add(tenant);
                tenants.Add(tenant);
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Generated {tenants.Count} tenants");

            return tenants;
        }

        #endregion

        #region Lease Generation

        private async Task<List<Lease>> GenerateLeasesAsync(
            List<Property> properties,
            List<Tenant> tenants,
            Guid organizationId,
            string userId)
        {
            var leases = new List<Lease>();

            // Create 3 leases for first 3 properties
            var leaseStartMonths = new[] { 5, 6, 7 }; // May, June, July 2025

            for (int i = 0; i < 3; i++)
            {
                var property = properties[i];
                var tenant = tenants[i];
                var startMonth = leaseStartMonths[i];

                // Random start day (1-5 of month)
                var startDay = _random.Next(1, 6);
                var startDate = new DateTime(2025, startMonth, startDay);
                var endDate = startDate.AddYears(1); // 12-month lease

                var lease = new Lease
                {
                    Id = Guid.NewGuid(),
                    OrganizationId = organizationId,
                    PropertyId = property.Id,
                    TenantId = tenant.Id,
                    StartDate = startDate,
                    EndDate = endDate,
                    MonthlyRent = property.MonthlyRent,
                    SecurityDeposit = property.MonthlyRent, // 1x rent
                    Status = ApplicationConstants.LeaseStatuses.Active,
                    Terms = $"12-month {ApplicationConstants.LeaseTypes.FixedTerm} lease. Rent: ${property.MonthlyRent}/month. " +
                           $"Security Deposit: ${property.MonthlyRent}. Payment due on the 5th of each month.",
                    SignedOn = startDate.AddDays(-10), // Signed 10 days before start
                    OfferedOn = startDate.AddDays(-20), // Offered 20 days before start
                    CreatedBy = userId,
                    CreatedOn = startDate.AddDays(-25),
                    IsDeleted = false
                };

                _context.Leases.Add(lease);
                leases.Add(lease);

                // Update property status to Occupied
                property.Status = ApplicationConstants.PropertyStatuses.Occupied;
                property.IsAvailable = false;
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Generated {leases.Count} leases");

            return leases;
        }

        #endregion

        #region Invoice Generation

        private async Task<List<Invoice>> GenerateInvoicesAsync(
            List<Lease> leases,
            Guid organizationId,
            string userId)
        {
            var invoices = new List<Invoice>();
            var currentDate = DateTime.UtcNow.Date;

            foreach (var lease in leases)
            {
                var invoiceDate = new DateTime(lease.StartDate.Year, lease.StartDate.Month, 20); // 20th of start month
                var currentMonth = new DateTime(currentDate.Year, currentDate.Month, 1);

                // Generate invoices from lease start to current month
                while (invoiceDate.Month <= currentDate.Month || invoiceDate.Year < currentDate.Year)
                {
                    // Skip if invoice date is in the future
                    if (invoiceDate > currentDate)
                        break;

                    var dueDate = invoiceDate.AddMonths(1);
                    dueDate = new DateTime(dueDate.Year, dueDate.Month, 5); // Due on 5th of following month

                    // Generate proper invoice number using service
                    var invoiceNumber = await _invoiceService.GenerateInvoiceNumberAsync();

                    var invoice = new Invoice
                    {
                        Id = Guid.NewGuid(),
                        OrganizationId = organizationId,
                        LeaseId = lease.Id,
                        InvoiceNumber = invoiceNumber,
                        InvoicedOn = invoiceDate,
                        DueOn = dueDate,
                        Amount = lease.MonthlyRent,
                        Status = ApplicationConstants.InvoiceStatuses.Pending,
                        Description = $"Monthly Rent - {invoiceDate:MMMM yyyy}",
                        CreatedBy = userId,
                        CreatedOn = invoiceDate,
                        IsDeleted = false
                    };

                    _context.Invoices.Add(invoice);
                    
                    // CRITICAL FIX: Save immediately after generating number to prevent collisions
                    // The MAX query in GenerateInvoiceNumberAsync needs to see this invoice
                    // before generating the next number
                    await _context.SaveChangesAsync();
                    
                    invoices.Add(invoice);

                    // Move to next month
                    invoiceDate = invoiceDate.AddMonths(1);
                }
            }

            _logger.LogInformation($"Generated {invoices.Count} invoices");

            return invoices;
        }

        #endregion

        #region Payment Generation

        private async Task<List<Payment>> GeneratePaymentsAsync(
            List<Invoice> invoices,
            Guid organizationId,
            string userId)
        {
            var payments = new List<Payment>();
            var currentDate = DateTime.UtcNow.Date;

            // Group invoices by lease to check remaining months
            var invoicesByLease = invoices.GroupBy(i => i.LeaseId).ToList();

            foreach (var leaseGroup in invoicesByLease)
            {
                var leaseInvoices = leaseGroup.OrderBy(i => i.InvoicedOn).ToList();

                // Calculate months remaining (leases end in May/June/July 2026)
                var lastInvoice = leaseInvoices.Last();
                var lease = await _context.Leases.FindAsync(leaseGroup.Key);
                
                if (lease == null) continue;

                var monthsRemaining = ((lease.EndDate.Year - currentDate.Year) * 12) + 
                                     lease.EndDate.Month - currentDate.Month;

                // If >3 months remaining, create payments for last 3 months only
                if (monthsRemaining > 3)
                {
                    // Get last 3 invoices that have passed their due date
                    var recentInvoices = leaseInvoices
                        .Where(i => i.DueOn < currentDate)
                        .OrderByDescending(i => i.InvoicedOn)
                        .Take(3)
                        .ToList();

                    foreach (var invoice in recentInvoices)
                    {
                        // Payment made 1-4 days before due date
                        var paymentDate = invoice.DueOn.AddDays(-_random.Next(1, 5));

                        // Generate proper payment number using service
                        var paymentNumber = await _paymentService.GeneratePaymentNumberAsync();

                        var payment = new Payment
                        {
                            Id = Guid.NewGuid(),
                            OrganizationId = organizationId,
                            InvoiceId = invoice.Id,
                            Amount = invoice.Amount,
                            PaymentNumber = paymentNumber,
                            PaidOn = paymentDate,
                            PaymentMethod = GetRandomPaymentMethod(),
                            Notes = $"Payment for {invoice.Description}",
                            CreatedBy = userId,
                            CreatedOn = paymentDate,
                            IsDeleted = false
                        };

                        _context.Payments.Add(payment);
                        
                        // CRITICAL FIX: Save immediately after generating number to prevent collisions
                        await _context.SaveChangesAsync();
                        
                        payments.Add(payment);

                        // Update invoice status to Paid
                        invoice.Status = ApplicationConstants.InvoiceStatuses.Paid;
                        invoice.AmountPaid = invoice.Amount;
                        invoice.PaidOn = paymentDate;
                        invoice.LastModifiedBy = userId;
                        invoice.LastModifiedOn = paymentDate;
                        
                        // Save invoice status update immediately
                        await _context.SaveChangesAsync();
                    }
                }
            }

            _logger.LogInformation($"Generated {payments.Count} payments");

            return payments;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets a random date within the specified range.
        /// </summary>
        private DateTime GetRandomDate(DateTime start, DateTime end)
        {
            var range = (end - start).Days;
            return start.AddDays(_random.Next(range + 1));
        }

        /// <summary>
        /// Gets a random payment method from available options.
        /// </summary>
        private string GetRandomPaymentMethod()
        {
            var methods = new[]
            {
                ApplicationConstants.PaymentMethods.CreditCard,
                ApplicationConstants.PaymentMethods.Check,
                ApplicationConstants.PaymentMethods.BankTransfer,
                ApplicationConstants.PaymentMethods.Cash
            };

            return methods[_random.Next(methods.Length)];
        }

        #endregion

        #region Sample Data Removal

        private async Task<int> RemovePropertiesAsync(Guid organizationId, string systemUserId)
        {
            var properties = await _context.Properties
                .Where(p => p.OrganizationId == organizationId && p.CreatedBy == systemUserId)
                .ToListAsync();

            _context.Properties.RemoveRange(properties);
            await _context.SaveChangesAsync();

            return properties.Count;
        }

        private async Task<int> RemoveTenantsAsync(Guid organizationId, string systemUserId)
        {
            var tenants = await _context.Tenants
                .Where(t => t.OrganizationId == organizationId && t.CreatedBy == systemUserId)
                .ToListAsync();

            _context.Tenants.RemoveRange(tenants);
            await _context.SaveChangesAsync();

            return tenants.Count;
        }

        private async Task<int> RemoveLeasesAsync(Guid organizationId, string systemUserId)
        {
            var leases = await _context.Leases
                .Where(l => l.OrganizationId == organizationId && l.CreatedBy == systemUserId)
                .ToListAsync();

            _context.Leases.RemoveRange(leases);
            await _context.SaveChangesAsync();

            return leases.Count;
        }

        private async Task<int> RemoveInvoicesAsync(Guid organizationId, string systemUserId)
        {
            var invoices = await _context.Invoices
                .Where(i => i.OrganizationId == organizationId && i.CreatedBy == systemUserId)
                .ToListAsync();

            _context.Invoices.RemoveRange(invoices);
            await _context.SaveChangesAsync();

            return invoices.Count;
        }

        private async Task<int> RemovePaymentsAsync(Guid organizationId, string systemUserId)
        {
            var payments = await _context.Payments
                .Where(p => p.OrganizationId == organizationId && p.CreatedBy == systemUserId)
                .ToListAsync();

            _context.Payments.RemoveRange(payments);
            await _context.SaveChangesAsync();

            return payments.Count;
        }

        #endregion
    }
}
