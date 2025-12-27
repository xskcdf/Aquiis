using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Aquiis.SimpleStart.Core.Constants;
using Aquiis.SimpleStart.Core.Entities;
using Aquiis.SimpleStart.Infrastructure.Data;
using Aquiis.SimpleStart.Shared.Components.Account;
using Aquiis.SimpleStart.Shared.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using PaymentService = Aquiis.SimpleStart.Application.Services.PaymentService;

namespace Aquiis.SimpleStart.Tests
{
    /// <summary>
    /// Comprehensive unit tests for PaymentService.
    /// Tests CRUD operations, validation, business logic, and organization isolation.
    /// </summary>
    public class PaymentServiceTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly ApplicationDbContext _context;
        private readonly UserContextService _userContext;
        private readonly Mock<ILogger<PaymentService>> _mockLogger;
        private readonly IOptions<ApplicationSettings> _mockSettings;
        private readonly PaymentService _service;
        private readonly Guid _testOrgId = Guid.NewGuid();
        private readonly string _testUserId = "test-user-123";
        private readonly Guid _testPropertyId = Guid.NewGuid();
        private readonly Guid _testTenantId = Guid.NewGuid();
        private readonly Guid _testLeaseId = Guid.NewGuid();
        private readonly Guid _testInvoiceId = Guid.NewGuid();

        public PaymentServiceTests()
        {
            // Setup SQLite in-memory database
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();

            // Mock AuthenticationStateProvider with claims
            var claims = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, _testUserId) },
                "TestAuth"));
            var mockAuth = new Mock<AuthenticationStateProvider>();
            mockAuth.Setup(a => a.GetAuthenticationStateAsync())
                .ReturnsAsync(new AuthenticationState(claims));

            // Mock UserManager
            var mockUserStore = new Mock<IUserStore<ApplicationUser>>();
            var mockUserManager = new Mock<UserManager<ApplicationUser>>(
                mockUserStore.Object, null, null, null, null, null, null, null, null);

            var appUser = new ApplicationUser
            {
                Id = _testUserId,
                UserName = "testuser",
                Email = "testuser@example.com",
                ActiveOrganizationId = _testOrgId
            };
            mockUserManager.Setup(u => u.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(appUser);

            var serviceProvider = new Mock<IServiceProvider>();
            _userContext = new UserContextService(mockAuth.Object, mockUserManager.Object, serviceProvider.Object);

            // Create test user
            var user = new ApplicationUser
            {
                Id = _testUserId,
                UserName = "testuser",
                Email = "testuser@example.com",
                ActiveOrganizationId = _testOrgId
            };
            _context.Users.Add(user);

            // Create test organization
            var organization = new Organization
            {
                Id = _testOrgId,
                Name = "Test Organization",
                OwnerId = _testUserId,
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            _context.Organizations.Add(organization);

            // Create test property
            var property = new Property
            {
                Id = _testPropertyId,
                OrganizationId = _testOrgId,
                Address = "123 Test St",
                City = "Test City",
                State = "TS",
                ZipCode = "12345",
                IsAvailable = false,
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            _context.Properties.Add(property);

            // Create test tenant
            var tenant = new Tenant
            {
                Id = _testTenantId,
                OrganizationId = _testOrgId,
                FirstName = "Test",
                LastName = "Tenant",
                Email = "tenant@test.com",
                IdentificationNumber = "SSN123456",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            _context.Tenants.Add(tenant);

            // Create test lease
            var lease = new Lease
            {
                Id = _testLeaseId,
                OrganizationId = _testOrgId,
                PropertyId = _testPropertyId,
                TenantId = _testTenantId,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddYears(1),
                MonthlyRent = 1500,
                SecurityDeposit = 1500,
                Status = "Active",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            _context.Leases.Add(lease);

            // Create test invoice
            var invoice = new Invoice
            {
                Id = _testInvoiceId,
                OrganizationId = _testOrgId,
                LeaseId = _testLeaseId,
                InvoiceNumber = "INV-TEST-001",
                InvoicedOn = DateTime.Today,
                DueOn = DateTime.Today.AddDays(30),
                Amount = 1500,
                Description = "Test Invoice",
                Status = "Pending",
                AmountPaid = 0,
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            _context.Invoices.Add(invoice);

            _context.SaveChanges();

            // Setup logger and settings
            _mockLogger = new Mock<ILogger<PaymentService>>();

            _mockSettings = Options.Create(new ApplicationSettings
            {
                SoftDeleteEnabled = true
            });

            // Create service instance
            _service = new PaymentService(
                _context,
                _mockLogger.Object,
                _userContext,
                _mockSettings);
        }

        public void Dispose()
        {
            _context.Dispose();
            _connection.Dispose();
        }

        #region Validation Tests

        [Fact]
        public async Task CreateAsync_ValidPayment_CreatesSuccessfully()
        {
            // Arrange
            var payment = new Payment
            {
                OrganizationId = _testOrgId,
                InvoiceId = _testInvoiceId,
                Amount = 1500,
                PaidOn = DateTime.Today,
                PaymentMethod = "Check",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };

            // Act
            var result = await _service.CreateAsync(payment);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal(1500, result.Amount);
            Assert.Equal("Check", result.PaymentMethod);
        }

        [Fact]
        public async Task CreateAsync_MissingInvoiceId_ThrowsException()
        {
            // Arrange
            var payment = new Payment
            {
                OrganizationId = _testOrgId,
                InvoiceId = Guid.Empty, // Missing
                Amount = 1500,
                PaidOn = DateTime.Today,
                PaymentMethod = "Check",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAsync(payment));
        }

        [Fact]
        public async Task CreateAsync_ZeroAmount_ThrowsException()
        {
            // Arrange
            var payment = new Payment
            {
                OrganizationId = _testOrgId,
                InvoiceId = _testInvoiceId,
                Amount = 0, // Invalid
                PaidOn = DateTime.Today,
                PaymentMethod = "Check",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAsync(payment));
        }

        [Fact]
        public async Task CreateAsync_NegativeAmount_ThrowsException()
        {
            // Arrange
            var payment = new Payment
            {
                OrganizationId = _testOrgId,
                InvoiceId = _testInvoiceId,
                Amount = -100, // Invalid
                PaidOn = DateTime.Today,
                PaymentMethod = "Check",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAsync(payment));
        }

        [Fact]
        public async Task CreateAsync_FuturePaymentDate_ThrowsException()
        {
            // Arrange
            var payment = new Payment
            {
                OrganizationId = _testOrgId,
                InvoiceId = _testInvoiceId,
                Amount = 1500,
                PaidOn = DateTime.Today.AddDays(5), // Future date
                PaymentMethod = "Check",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAsync(payment));
        }

        [Fact]
        public async Task CreateAsync_ExceedsInvoiceBalance_ThrowsException()
        {
            // Arrange
            var payment = new Payment
            {
                OrganizationId = _testOrgId,
                InvoiceId = _testInvoiceId,
                Amount = 2000, // Exceeds invoice amount of 1500
                PaidOn = DateTime.Today,
                PaymentMethod = "Check",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAsync(payment));
        }

        [Fact]
        public async Task CreateAsync_InvalidPaymentMethod_ThrowsException()
        {
            // Arrange
            var payment = new Payment
            {
                OrganizationId = _testOrgId,
                InvoiceId = _testInvoiceId,
                Amount = 1500,
                PaidOn = DateTime.Today,
                PaymentMethod = "InvalidMethod", // Invalid
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAsync(payment));
        }

        #endregion

        #region Retrieval Tests

        [Fact]
        public async Task GetPaymentsByInvoiceIdAsync_ReturnsInvoicePayments()
        {
            // Arrange - Create payments
            var payment1 = new Payment
            {
                OrganizationId = _testOrgId,
                InvoiceId = _testInvoiceId,
                Amount = 500,
                PaidOn = DateTime.Today,
                PaymentMethod = "Check",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            await _service.CreateAsync(payment1);

            var payment2 = new Payment
            {
                OrganizationId = _testOrgId,
                InvoiceId = _testInvoiceId,
                Amount = 500,
                PaidOn = DateTime.Today.AddDays(1),
                PaymentMethod = "Cash",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            await _service.CreateAsync(payment2);

            // Act
            var result = await _service.GetPaymentsByInvoiceIdAsync(_testInvoiceId);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, p => Assert.Equal(_testInvoiceId, p.InvoiceId));
        }

        [Fact]
        public async Task GetPaymentsByMethodAsync_ReturnsMatchingPayments()
        {
            // Arrange
            var check = new Payment
            {
                OrganizationId = _testOrgId,
                InvoiceId = _testInvoiceId,
                Amount = 500,
                PaidOn = DateTime.Today,
                PaymentMethod = "Check",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            await _service.CreateAsync(check);

            var cash = new Payment
            {
                OrganizationId = _testOrgId,
                InvoiceId = _testInvoiceId,
                Amount = 500,
                PaidOn = DateTime.Today,
                PaymentMethod = "Cash",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            await _service.CreateAsync(cash);

            // Act
            var checkResults = await _service.GetPaymentsByMethodAsync("Check");
            var cashResults = await _service.GetPaymentsByMethodAsync("Cash");

            // Assert
            Assert.Single(checkResults);
            Assert.Single(cashResults);
            Assert.Equal("Check", checkResults[0].PaymentMethod);
            Assert.Equal("Cash", cashResults[0].PaymentMethod);
        }

        [Fact]
        public async Task GetPaymentsByDateRangeAsync_ReturnsPaymentsInRange()
        {
            // Arrange
            var oldPayment = new Payment
            {
                OrganizationId = _testOrgId,
                InvoiceId = _testInvoiceId,
                Amount = 500,
                PaidOn = DateTime.Today.AddMonths(-2),
                PaymentMethod = "Check",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            await _service.CreateAsync(oldPayment);

            var recentPayment = new Payment
            {
                OrganizationId = _testOrgId,
                InvoiceId = _testInvoiceId,
                Amount = 500,
                PaidOn = DateTime.Today,
                PaymentMethod = "Cash",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            await _service.CreateAsync(recentPayment);

            // Act
            var startDate = DateTime.Today.AddDays(-7);
            var endDate = DateTime.Today.AddDays(1);
            var result = await _service.GetPaymentsByDateRangeAsync(startDate, endDate);

            // Assert
            Assert.Single(result);
            Assert.Equal(recentPayment.Amount, result[0].Amount);
        }

        [Fact]
        public async Task GetPaymentWithRelationsAsync_LoadsAllRelations()
        {
            // Arrange
            var payment = new Payment
            {
                OrganizationId = _testOrgId,
                InvoiceId = _testInvoiceId,
                Amount = 1500,
                PaidOn = DateTime.Today,
                PaymentMethod = "Check",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            var created = await _service.CreateAsync(payment);

            // Act
            var result = await _service.GetPaymentWithRelationsAsync(created.Id);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Invoice);
            Assert.NotNull(result.Invoice.Lease);
            Assert.NotNull(result.Invoice.Lease.Property);
            Assert.NotNull(result.Invoice.Lease.Tenant);
            Assert.Equal(_testInvoiceId, result.Invoice.Id);
        }

        #endregion

        #region Business Logic Tests

        [Fact]
        public async Task CreateAsync_UpdatesInvoiceStatus_WhenFullyPaid()
        {
            // Arrange
            var payment = new Payment
            {
                OrganizationId = _testOrgId,
                InvoiceId = _testInvoiceId,
                Amount = 1500, // Full amount
                PaidOn = DateTime.Today,
                PaymentMethod = "Check",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };

            // Act
            await _service.CreateAsync(payment);

            // Assert - Check invoice status updated
            var invoice = await _context.Invoices.FindAsync(_testInvoiceId);
            Assert.NotNull(invoice);
            Assert.Equal("Paid", invoice.Status);
            Assert.Equal(1500, invoice.AmountPaid);
            Assert.NotNull(invoice.PaidOn);
        }

        [Fact]
        public async Task CreateAsync_PartialPayment_UpdatesInvoiceAmountPaid()
        {
            // Arrange
            var payment = new Payment
            {
                OrganizationId = _testOrgId,
                InvoiceId = _testInvoiceId,
                Amount = 750, // Partial payment
                PaidOn = DateTime.Today,
                PaymentMethod = "Check",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };

            // Act
            await _service.CreateAsync(payment);

            // Assert - Check invoice updated but not marked as paid
            var invoice = await _context.Invoices.FindAsync(_testInvoiceId);
            Assert.NotNull(invoice);
            Assert.Equal("Pending", invoice.Status);
            Assert.Equal(750, invoice.AmountPaid);
        }

        [Fact]
        public async Task CreateAsync_MultiplePayments_UpdatesInvoiceCorrectly()
        {
            // Arrange - Create two partial payments
            var payment1 = new Payment
            {
                OrganizationId = _testOrgId,
                InvoiceId = _testInvoiceId,
                Amount = 750,
                PaidOn = DateTime.Today,
                PaymentMethod = "Check",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            await _service.CreateAsync(payment1);

            var payment2 = new Payment
            {
                OrganizationId = _testOrgId,
                InvoiceId = _testInvoiceId,
                Amount = 750,
                PaidOn = DateTime.Today.AddDays(1),
                PaymentMethod = "Cash",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };

            // Act
            await _service.CreateAsync(payment2);

            // Assert - Invoice should be fully paid
            var invoice = await _context.Invoices.FindAsync(_testInvoiceId);
            Assert.NotNull(invoice);
            Assert.Equal("Paid", invoice.Status);
            Assert.Equal(1500, invoice.AmountPaid);
        }

        [Fact]
        public async Task DeleteAsync_UpdatesInvoiceStatus()
        {
            // Arrange - Create full payment
            var payment = new Payment
            {
                OrganizationId = _testOrgId,
                InvoiceId = _testInvoiceId,
                Amount = 1500,
                PaidOn = DateTime.Today,
                PaymentMethod = "Check",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            var created = await _service.CreateAsync(payment);

            // Verify invoice is paid
            var invoice = await _context.Invoices.FindAsync(_testInvoiceId);
            Assert.Equal("Paid", invoice!.Status);

            // Act - Delete payment
            await _service.DeleteAsync(created.Id);

            // Assert - Invoice should be back to pending
            invoice = await _context.Invoices.FindAsync(_testInvoiceId);
            Assert.NotNull(invoice);
            Assert.Equal("Pending", invoice.Status);
            Assert.Equal(0, invoice.AmountPaid);
        }

        [Fact]
        public async Task CalculateTotalPaymentsAsync_ReturnsCorrectTotal()
        {
            // Arrange - Create payments
            var payment1 = new Payment
            {
                OrganizationId = _testOrgId,
                InvoiceId = _testInvoiceId,
                Amount = 500,
                PaidOn = DateTime.Today,
                PaymentMethod = "Check",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            await _service.CreateAsync(payment1);

            var payment2 = new Payment
            {
                OrganizationId = _testOrgId,
                InvoiceId = _testInvoiceId,
                Amount = 750,
                PaidOn = DateTime.Today,
                PaymentMethod = "Cash",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            await _service.CreateAsync(payment2);

            // Act
            var total = await _service.CalculateTotalPaymentsAsync();

            // Assert
            Assert.Equal(1250m, total);
        }

        [Fact]
        public async Task CalculateTotalPaymentsAsync_WithDateRange_ReturnsFilteredTotal()
        {
            // Arrange
            var oldPayment = new Payment
            {
                OrganizationId = _testOrgId,
                InvoiceId = _testInvoiceId,
                Amount = 500,
                PaidOn = DateTime.Today.AddMonths(-2),
                PaymentMethod = "Check",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            await _service.CreateAsync(oldPayment);

            var recentPayment = new Payment
            {
                OrganizationId = _testOrgId,
                InvoiceId = _testInvoiceId,
                Amount = 750,
                PaidOn = DateTime.Today,
                PaymentMethod = "Cash",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            await _service.CreateAsync(recentPayment);

            // Act
            var startDate = DateTime.Today.AddDays(-7);
            var endDate = DateTime.Today.AddDays(1);
            var total = await _service.CalculateTotalPaymentsAsync(startDate, endDate);

            // Assert
            Assert.Equal(750m, total); // Only recent payment
        }

        [Fact]
        public async Task GetPaymentSummaryByMethodAsync_ReturnsCorrectSummary()
        {
            // Arrange
            var check1 = new Payment
            {
                OrganizationId = _testOrgId,
                InvoiceId = _testInvoiceId,
                Amount = 500,
                PaidOn = DateTime.Today,
                PaymentMethod = "Check",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            await _service.CreateAsync(check1);

            var check2 = new Payment
            {
                OrganizationId = _testOrgId,
                InvoiceId = _testInvoiceId,
                Amount = 300,
                PaidOn = DateTime.Today,
                PaymentMethod = "Check",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            await _service.CreateAsync(check2);

            var cash = new Payment
            {
                OrganizationId = _testOrgId,
                InvoiceId = _testInvoiceId,
                Amount = 700,
                PaidOn = DateTime.Today,
                PaymentMethod = "Cash",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            await _service.CreateAsync(cash);

            // Act
            var summary = await _service.GetPaymentSummaryByMethodAsync();

            // Assert
            Assert.Equal(2, summary.Count);
            Assert.Equal(800m, summary["Check"]); // 500 + 300
            Assert.Equal(700m, summary["Cash"]);
        }

        [Fact]
        public async Task GetTotalPaidForInvoiceAsync_ReturnsCorrectTotal()
        {
            // Arrange
            var payment1 = new Payment
            {
                OrganizationId = _testOrgId,
                InvoiceId = _testInvoiceId,
                Amount = 500,
                PaidOn = DateTime.Today,
                PaymentMethod = "Check",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            await _service.CreateAsync(payment1);

            var payment2 = new Payment
            {
                OrganizationId = _testOrgId,
                InvoiceId = _testInvoiceId,
                Amount = 750,
                PaidOn = DateTime.Today,
                PaymentMethod = "Cash",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            await _service.CreateAsync(payment2);

            // Act
            var total = await _service.GetTotalPaidForInvoiceAsync(_testInvoiceId);

            // Assert
            Assert.Equal(1250m, total);
        }

        #endregion

        #region Organization Isolation Tests

        [Fact]
        public async Task GetByIdAsync_DifferentOrganization_ReturnsNull()
        {
            // Arrange - Create different organization and payment
            var otherUserId = "other-user-456";
            var otherUser = new ApplicationUser
            {
                Id = otherUserId,
                UserName = "otheruser",
                Email = "otheruser@example.com"
            };
            _context.Users.Add(otherUser);
            await _context.SaveChangesAsync();

            var otherOrg = new Organization
            {
                Id = Guid.NewGuid(),
                Name = "Other Organization",
                OwnerId = otherUserId,
                CreatedBy = otherUserId,
                CreatedOn = DateTime.UtcNow
            };
            await _context.Organizations.AddAsync(otherOrg);

            var otherProperty = new Property
            {
                Id = Guid.NewGuid(),
                OrganizationId = otherOrg.Id,
                Address = "999 Other St",
                City = "Other City",
                State = "OT",
                ZipCode = "99999",
                IsAvailable = true,
                CreatedBy = otherUserId,
                CreatedOn = DateTime.UtcNow
            };
            await _context.Properties.AddAsync(otherProperty);

            var otherTenant = new Tenant
            {
                Id = Guid.NewGuid(),
                OrganizationId = otherOrg.Id,
                FirstName = "Other",
                LastName = "Tenant",
                Email = "other@test.com",
                IdentificationNumber = "ID999",
                CreatedBy = otherUserId,
                CreatedOn = DateTime.UtcNow
            };
            await _context.Tenants.AddAsync(otherTenant);

            var otherLease = new Lease
            {
                Id = Guid.NewGuid(),
                OrganizationId = otherOrg.Id,
                PropertyId = otherProperty.Id,
                TenantId = otherTenant.Id,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddYears(1),
                MonthlyRent = 2000,
                SecurityDeposit = 2000,
                Status = "Active",
                CreatedBy = otherUserId,
                CreatedOn = DateTime.UtcNow
            };
            await _context.Leases.AddAsync(otherLease);

            var otherInvoice = new Invoice
            {
                Id = Guid.NewGuid(),
                OrganizationId = otherOrg.Id,
                LeaseId = otherLease.Id,
                InvoiceNumber = "INV-OTHER",
                InvoicedOn = DateTime.Today,
                DueOn = DateTime.Today.AddDays(30),
                Amount = 2000,
                Description = "Other Org Invoice",
                Status = "Pending",
                CreatedBy = otherUserId,
                CreatedOn = DateTime.UtcNow
            };
            await _context.Invoices.AddAsync(otherInvoice);

            var otherOrgPayment = new Payment
            {
                Id = Guid.NewGuid(),
                OrganizationId = otherOrg.Id,
                InvoiceId = otherInvoice.Id,
                Amount = 2000,
                PaidOn = DateTime.Today,
                PaymentMethod = "Check",
                CreatedBy = otherUserId,
                CreatedOn = DateTime.UtcNow
            };
            await _context.Payments.AddAsync(otherOrgPayment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetByIdAsync(otherOrgPayment.Id);

            // Assert
            Assert.Null(result); // Should not access payment from different org
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOnlyCurrentOrganizationPayments()
        {
            // Arrange - Create payment in test org
            var testOrgPayment = new Payment
            {
                OrganizationId = _testOrgId,
                InvoiceId = _testInvoiceId,
                Amount = 1500,
                PaidOn = DateTime.Today,
                PaymentMethod = "Check",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            await _service.CreateAsync(testOrgPayment);

            // Create payment in different org
            var otherUserId = "other-user-456";
            var otherUser = new ApplicationUser
            {
                Id = otherUserId,
                UserName = "otheruser",
                Email = "otheruser@example.com"
            };
            _context.Users.Add(otherUser);
            await _context.SaveChangesAsync();

            var otherOrg = new Organization
            {
                Id = Guid.NewGuid(),
                Name = "Other Organization",
                OwnerId = otherUserId,
                CreatedBy = otherUserId,
                CreatedOn = DateTime.UtcNow
            };
            await _context.Organizations.AddAsync(otherOrg);

            var otherProperty = new Property
            {
                Id = Guid.NewGuid(),
                OrganizationId = otherOrg.Id,
                Address = "888 Other Ave",
                City = "Other City",
                State = "OT",
                ZipCode = "88888",
                IsAvailable = true,
                CreatedBy = otherUserId,
                CreatedOn = DateTime.UtcNow
            };
            await _context.Properties.AddAsync(otherProperty);

            var otherTenant = new Tenant
            {
                Id = Guid.NewGuid(),
                OrganizationId = otherOrg.Id,
                FirstName = "Other",
                LastName = "Tenant",
                Email = "other@test.com",
                IdentificationNumber = "ID888",
                CreatedBy = otherUserId,
                CreatedOn = DateTime.UtcNow
            };
            await _context.Tenants.AddAsync(otherTenant);

            var otherLease = new Lease
            {
                Id = Guid.NewGuid(),
                OrganizationId = otherOrg.Id,
                PropertyId = otherProperty.Id,
                TenantId = otherTenant.Id,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddYears(1),
                MonthlyRent = 2500,
                SecurityDeposit = 2500,
                Status = "Active",
                CreatedBy = otherUserId,
                CreatedOn = DateTime.UtcNow
            };
            await _context.Leases.AddAsync(otherLease);

            var otherInvoice = new Invoice
            {
                Id = Guid.NewGuid(),
                OrganizationId = otherOrg.Id,
                LeaseId = otherLease.Id,
                InvoiceNumber = "INV-OTHER-ORG",
                InvoicedOn = DateTime.Today,
                DueOn = DateTime.Today.AddDays(30),
                Amount = 2500,
                Description = "Other",
                Status = "Pending",
                CreatedBy = otherUserId,
                CreatedOn = DateTime.UtcNow
            };
            await _context.Invoices.AddAsync(otherInvoice);

            var otherOrgPayment = new Payment
            {
                Id = Guid.NewGuid(),
                OrganizationId = otherOrg.Id,
                InvoiceId = otherInvoice.Id,
                Amount = 2500,
                PaidOn = DateTime.Today,
                PaymentMethod = "Cash",
                CreatedBy = otherUserId,
                CreatedOn = DateTime.UtcNow
            };
            await _context.Payments.AddAsync(otherOrgPayment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal(_testOrgId, result[0].OrganizationId);
        }

        #endregion
    }
}
