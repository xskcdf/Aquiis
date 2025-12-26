using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Aquiis.SimpleStart.Application.Services;
using Aquiis.SimpleStart.Core.Constants;
using Aquiis.SimpleStart.Core.Entities;
using Aquiis.SimpleStart.Infrastructure.Data;
using Aquiis.SimpleStart.Shared.Components.Account;
using Aquiis.SimpleStart.Shared.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Aquiis.SimpleStart.Tests
{
    /// <summary>
    /// Unit tests for PropertyService business logic and property-specific operations.
    /// </summary>
    public class PropertyServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly UserContextService _userContext;
        private readonly PropertyService _service;
        private readonly string _testUserId;
        private readonly Guid _testOrgId;
        private readonly Microsoft.Data.Sqlite.SqliteConnection _connection;

        public PropertyServiceTests()
        {
            // Setup SQLite in-memory database
            _connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=:memory:");
            _connection.Open();
            
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();

            // Setup test user and organization
            _testUserId = "test-user-123";
            _testOrgId = Guid.NewGuid();

            // Mock AuthenticationStateProvider
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
                Email = "test@example.com",
                ActiveOrganizationId = _testOrgId 
            };
            mockUserManager.Setup(u => u.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(appUser);

            var serviceProvider = new Mock<IServiceProvider>();
            _userContext = new UserContextService(mockAuth.Object, mockUserManager.Object, serviceProvider.Object);

            // Seed test data
            var user = new ApplicationUser 
            { 
                Id = _testUserId, 
                UserName = "testuser", 
                Email = "test@example.com",
                ActiveOrganizationId = _testOrgId 
            };
            _context.Users.Add(user);

            var org = new Organization 
            { 
                Id = _testOrgId, 
                Name = "Test Org", 
                OwnerId = _testUserId,
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow 
            };
            _context.Organizations.Add(org);
            _context.SaveChanges();

            // Create PropertyService with mocked dependencies
            var mockSettings = Options.Create(new ApplicationSettings 
            { 
                SoftDeleteEnabled = true 
            });
            
            var mockLogger = new Mock<ILogger<PropertyService>>();
            
            // Create real CalendarEventService for testing
            var mockCalendarSettings = new Mock<CalendarSettingsService>(_context, _userContext);
            var calendarService = new CalendarEventService(_context, mockCalendarSettings.Object, _userContext);
            
            _service = new PropertyService(_context, mockLogger.Object, _userContext, mockSettings, calendarService);
        }

        public void Dispose()
        {
            _context?.Dispose();
            _connection?.Dispose();
        }

        #region CreateAsync Override Tests

        [Fact]
        public async Task CreateAsync_SetsNextRoutineInspectionDate()
        {
            // Arrange
            var property = new Property
            {
                Address = "123 Main St",
                City = "Test City",
                State = "TS",
                ZipCode = "12345",
                PropertyType = "House"
            };
            var expectedDate = DateTime.Today.AddDays(30);

            // Act
            var result = await _service.CreateAsync(property);

            // Assert
            Assert.NotNull(result.NextRoutineInspectionDueDate);
            Assert.Equal(expectedDate, result.NextRoutineInspectionDueDate!.Value.Date);
        }

        #endregion

        #region Validation Tests

        [Fact]
        public async Task CreateAsync_MissingAddress_ThrowsValidationException()
        {
            // Arrange
            var property = new Property
            {
                Address = "", // Empty address
                City = "Test City",
                State = "TS",
                ZipCode = "12345",
                PropertyType = "House"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAsync(property));
        }

        [Fact]
        public async Task CreateAsync_DuplicateAddress_ThrowsValidationException()
        {
            // Arrange
            var existingProperty = new Property
            {
                Id = Guid.NewGuid(),
                OrganizationId = _testOrgId,
                Address = "456 Duplicate St",
                City = "Same City",
                State = "SC",
                ZipCode = "54321",
                PropertyType = "House",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            _context.Properties.Add(existingProperty);
            await _context.SaveChangesAsync();

            var duplicateProperty = new Property
            {
                Address = "456 Duplicate St", // Same address
                City = "Same City",
                State = "SC",
                ZipCode = "54321",
                PropertyType = "Apartment"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAsync(duplicateProperty));
            Assert.Contains("already exists", exception.Message);
        }

        [Fact]
        public async Task CreateAsync_SameAddressDifferentOrganization_AllowsCreation()
        {
            // Arrange
            var differentOrgId = Guid.NewGuid();
            var differentOrg = new Organization 
            { 
                Id = differentOrgId, 
                Name = "Different Org", 
                OwnerId = _testUserId,
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow 
            };
            _context.Organizations.Add(differentOrg);
            
            var existingProperty = new Property
            {
                Id = Guid.NewGuid(),
                OrganizationId = differentOrgId, // Different organization
                Address = "789 Shared St",
                City = "Test City",
                State = "TS",
                ZipCode = "12345",
                PropertyType = "House",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            _context.Properties.Add(existingProperty);
            await _context.SaveChangesAsync();

            var newProperty = new Property
            {
                Address = "789 Shared St", // Same address, different org
                City = "Test City",
                State = "TS",
                ZipCode = "12345",
                PropertyType = "Apartment"
            };

            // Act
            var result = await _service.CreateAsync(newProperty);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newProperty.Address, result.Address);
        }

        #endregion

        #region GetPropertyWithRelationsAsync Tests

        [Fact]
        public async Task GetPropertyWithRelationsAsync_LoadsLeasesAndDocuments()
        {
            // Arrange
            var property = new Property
            {
                Id = Guid.NewGuid(),
                OrganizationId = _testOrgId,
                Address = "100 Relation St",
                City = "Test City",
                State = "TS",
                ZipCode = "12345",
                PropertyType = "House",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            _context.Properties.Add(property);

            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                OrganizationId = _testOrgId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "555-1234",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            _context.Tenants.Add(tenant);

            var lease = new Lease
            {
                Id = Guid.NewGuid(),
                OrganizationId = _testOrgId,
                PropertyId = property.Id,
                TenantId = tenant.Id,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddYears(1),
                MonthlyRent = 1500m,
                Status = ApplicationConstants.LeaseStatuses.Active,
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            _context.Leases.Add(lease);

            var document = new Document
            {
                Id = Guid.NewGuid(),
                OrganizationId = _testOrgId,
                PropertyId = property.Id,
                FileName = "test.pdf",
                FileType = "application/pdf",
                FileSize = 1024,
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            _context.Documents.Add(document);

            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetPropertyWithRelationsAsync(property.Id);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Leases);
            Assert.NotNull(result.Documents);
            Assert.Single(result.Leases);
            Assert.Single(result.Documents);
        }

        #endregion

        #region SearchPropertiesByAddressAsync Tests

        [Fact]
        public async Task SearchPropertiesByAddressAsync_EmptySearchTerm_ReturnsFirst20()
        {
            // Arrange - Create 25 properties
            for (int i = 1; i <= 25; i++)
            {
                var property = new Property
                {
                    Id = Guid.NewGuid(),
                    OrganizationId = _testOrgId,
                    Address = $"{i * 100} Test St",
                    City = "Test City",
                    State = "TS",
                    ZipCode = "12345",
                    PropertyType = "House",
                    CreatedBy = _testUserId,
                    CreatedOn = DateTime.UtcNow
                };
                _context.Properties.Add(property);
            }
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.SearchPropertiesByAddressAsync("");

            // Assert
            Assert.Equal(20, result.Count); // Should limit to 20
        }

        [Fact]
        public async Task SearchPropertiesByAddressAsync_SearchByAddress_ReturnsMatches()
        {
            // Arrange
            var properties = new[]
            {
                new Property { Id = Guid.NewGuid(), OrganizationId = _testOrgId, Address = "123 Main St", City = "City", State = "ST", ZipCode = "12345", PropertyType = "House", CreatedBy = _testUserId, CreatedOn = DateTime.UtcNow },
                new Property { Id = Guid.NewGuid(), OrganizationId = _testOrgId, Address = "456 Main Ave", City = "City", State = "ST", ZipCode = "12345", PropertyType = "Apartment", CreatedBy = _testUserId, CreatedOn = DateTime.UtcNow },
                new Property { Id = Guid.NewGuid(), OrganizationId = _testOrgId, Address = "789 Oak St", City = "City", State = "ST", ZipCode = "12345", PropertyType = "Condo", CreatedBy = _testUserId, CreatedOn = DateTime.UtcNow }
            };
            _context.Properties.AddRange(properties);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.SearchPropertiesByAddressAsync("Main");

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, p => Assert.Contains("Main", p.Address));
        }

        [Fact]
        public async Task SearchPropertiesByAddressAsync_SearchByCity_ReturnsMatches()
        {
            // Arrange
            var properties = new[]
            {
                new Property { Id = Guid.NewGuid(), OrganizationId = _testOrgId, Address = "100 Test St", City = "Springfield", State = "ST", ZipCode = "12345", PropertyType = "House", CreatedBy = _testUserId, CreatedOn = DateTime.UtcNow },
                new Property { Id = Guid.NewGuid(), OrganizationId = _testOrgId, Address = "200 Test St", City = "Springfield", State = "ST", ZipCode = "12345", PropertyType = "Apartment", CreatedBy = _testUserId, CreatedOn = DateTime.UtcNow },
                new Property { Id = Guid.NewGuid(), OrganizationId = _testOrgId, Address = "300 Test St", City = "Shelbyville", State = "ST", ZipCode = "54321", PropertyType = "Condo", CreatedBy = _testUserId, CreatedOn = DateTime.UtcNow }
            };
            _context.Properties.AddRange(properties);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.SearchPropertiesByAddressAsync("Springfield");

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, p => Assert.Equal("Springfield", p.City));
        }

        [Fact]
        public async Task SearchPropertiesByAddressAsync_SearchByZipCode_ReturnsMatches()
        {
            // Arrange
            var properties = new[]
            {
                new Property { Id = Guid.NewGuid(), OrganizationId = _testOrgId, Address = "100 Test St", City = "City", State = "ST", ZipCode = "90210", PropertyType = "House", CreatedBy = _testUserId, CreatedOn = DateTime.UtcNow },
                new Property { Id = Guid.NewGuid(), OrganizationId = _testOrgId, Address = "200 Test St", City = "City", State = "ST", ZipCode = "12345", PropertyType = "Apartment", CreatedBy = _testUserId, CreatedOn = DateTime.UtcNow }
            };
            _context.Properties.AddRange(properties);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.SearchPropertiesByAddressAsync("90210");

            // Assert
            Assert.Single(result);
            Assert.Equal("90210", result[0].ZipCode);
        }

        #endregion

        #region GetVacantPropertiesAsync Tests

        [Fact]
        public async Task GetVacantPropertiesAsync_ReturnsOnlyVacantProperties()
        {
            // Arrange
            var vacantProperty = new Property
            {
                Id = Guid.NewGuid(),
                OrganizationId = _testOrgId,
                Address = "100 Vacant St",
                City = "Test City",
                State = "TS",
                ZipCode = "12345",
                PropertyType = "House",
                IsAvailable = true,
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };

            var occupiedProperty = new Property
            {
                Id = Guid.NewGuid(),
                OrganizationId = _testOrgId,
                Address = "200 Occupied St",
                City = "Test City",
                State = "TS",
                ZipCode = "12345",
                PropertyType = "House",
                IsAvailable = true,
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };

            _context.Properties.AddRange(vacantProperty, occupiedProperty);
            await _context.SaveChangesAsync();

            // Add active lease to occupied property
            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                OrganizationId = _testOrgId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                PhoneNumber = "555-1234",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            _context.Tenants.Add(tenant);

            var activeLease = new Lease
            {
                Id = Guid.NewGuid(),
                OrganizationId = _testOrgId,
                PropertyId = occupiedProperty.Id,
                TenantId = tenant.Id,
                StartDate = DateTime.Today.AddMonths(-1),
                EndDate = DateTime.Today.AddMonths(11),
                MonthlyRent = 1500m,
                Status = ApplicationConstants.LeaseStatuses.Active,
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            _context.Leases.Add(activeLease);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetVacantPropertiesAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal(vacantProperty.Id, result[0].Id);
        }

        [Fact]
        public async Task GetVacantPropertiesAsync_ExcludesUnavailableProperties()
        {
            // Arrange
            var unavailableProperty = new Property
            {
                Id = Guid.NewGuid(),
                OrganizationId = _testOrgId,
                Address = "300 Unavailable St",
                City = "Test City",
                State = "TS",
                ZipCode = "12345",
                PropertyType = "House",
                IsAvailable = false, // Not available
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            _context.Properties.Add(unavailableProperty);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetVacantPropertiesAsync();

            // Assert
            Assert.Empty(result);
        }

        #endregion

        #region CalculateOccupancyRateAsync Tests

        [Fact]
        public async Task CalculateOccupancyRateAsync_NoProperties_ReturnsZero()
        {
            // Act
            var result = await _service.CalculateOccupancyRateAsync();

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task CalculateOccupancyRateAsync_CalculatesCorrectPercentage()
        {
            // Arrange - Create 4 available properties, 3 occupied
            var properties = Enumerable.Range(1, 4).Select(i => new Property
            {
                Id = Guid.NewGuid(),
                OrganizationId = _testOrgId,
                Address = $"{i}00 Test St",
                City = "Test City",
                State = "TS",
                ZipCode = "12345",
                PropertyType = "House",
                IsAvailable = true,
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            }).ToArray();
            _context.Properties.AddRange(properties);
            await _context.SaveChangesAsync();

            // Create tenant
            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                OrganizationId = _testOrgId,
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@example.com",
                PhoneNumber = "555-5678",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            _context.Tenants.Add(tenant);

            // Add active leases to 3 properties
            for (int i = 0; i < 3; i++)
            {
                var lease = new Lease
                {
                    Id = Guid.NewGuid(),
                    OrganizationId = _testOrgId,
                    PropertyId = properties[i].Id,
                    TenantId = tenant.Id,
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddYears(1),
                    MonthlyRent = 1500m,
                    Status = ApplicationConstants.LeaseStatuses.Active,
                    CreatedBy = _testUserId,
                    CreatedOn = DateTime.UtcNow
                };
                _context.Leases.Add(lease);
            }
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.CalculateOccupancyRateAsync();

            // Assert
            Assert.Equal(75m, result); // 3 out of 4 = 75%
        }

        #endregion

        #region GetPropertiesDueForInspectionAsync Tests

        [Fact]
        public async Task GetPropertiesDueForInspectionAsync_ReturnsDueProperties()
        {
            // Arrange
            var dueProperty = new Property
            {
                Id = Guid.NewGuid(),
                OrganizationId = _testOrgId,
                Address = "400 Due St",
                City = "Test City",
                State = "TS",
                ZipCode = "12345",
                PropertyType = "House",
                NextRoutineInspectionDueDate = DateTime.Today.AddDays(5), // Due in 5 days
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };

            var notDueProperty = new Property
            {
                Id = Guid.NewGuid(),
                OrganizationId = _testOrgId,
                Address = "500 Not Due St",
                City = "Test City",
                State = "TS",
                ZipCode = "12345",
                PropertyType = "House",
                NextRoutineInspectionDueDate = DateTime.Today.AddDays(30), // Not due yet
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };

            _context.Properties.AddRange(dueProperty, notDueProperty);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetPropertiesDueForInspectionAsync(7);

            // Assert
            Assert.Single(result);
            Assert.Equal(dueProperty.Id, result[0].Id);
        }

        [Fact]
        public async Task GetPropertiesDueForInspectionAsync_OrdersByDueDate()
        {
            // Arrange
            var properties = new[]
            {
                new Property { Id = Guid.NewGuid(), OrganizationId = _testOrgId, Address = "600 Test St", City = "City", State = "ST", ZipCode = "12345", PropertyType = "House", NextRoutineInspectionDueDate = DateTime.Today.AddDays(5), CreatedBy = _testUserId, CreatedOn = DateTime.UtcNow },
                new Property { Id = Guid.NewGuid(), OrganizationId = _testOrgId, Address = "700 Test St", City = "City", State = "ST", ZipCode = "12345", PropertyType = "House", NextRoutineInspectionDueDate = DateTime.Today.AddDays(2), CreatedBy = _testUserId, CreatedOn = DateTime.UtcNow },
                new Property { Id = Guid.NewGuid(), OrganizationId = _testOrgId, Address = "800 Test St", City = "City", State = "ST", ZipCode = "12345", PropertyType = "House", NextRoutineInspectionDueDate = DateTime.Today.AddDays(7), CreatedBy = _testUserId, CreatedOn = DateTime.UtcNow }
            };
            _context.Properties.AddRange(properties);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetPropertiesDueForInspectionAsync(10);

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal("700 Test St", result[0].Address); // Due in 2 days (first)
            Assert.Equal("600 Test St", result[1].Address); // Due in 5 days (second)
            Assert.Equal("800 Test St", result[2].Address); // Due in 7 days (third)
        }

        #endregion
    }
}
