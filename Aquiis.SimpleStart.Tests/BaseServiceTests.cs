using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Aquiis.SimpleStart.Core.Constants;
using Aquiis.SimpleStart.Core.Entities;
using Aquiis.SimpleStart.Core.Services;
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
    /// Unit tests for BaseService<TEntity> generic CRUD operations.
    /// Tests organization isolation, soft delete, audit fields, and security.
    /// </summary>
    public class BaseServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly UserContextService _userContext;
        private readonly TestPropertyService _service;
        private readonly string _testUserId;
        private readonly Guid _testOrgId;
        private readonly Microsoft.Data.Sqlite.SqliteConnection _connection;

        public BaseServiceTests()
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

            // Create service with mocked settings
            var mockSettings = Options.Create(new ApplicationSettings 
            { 
                SoftDeleteEnabled = true 
            });
            
            var mockLogger = new Mock<ILogger<TestPropertyService>>();
            _service = new TestPropertyService(_context, mockLogger.Object, _userContext, mockSettings);
        }

        public void Dispose()
        {
            _context?.Dispose();
            _connection?.Dispose();
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_ValidEntity_CreatesSuccessfully()
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

            // Act
            var result = await _service.CreateAsync(property);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal(_testOrgId, result.OrganizationId);
            Assert.Equal(_testUserId, result.CreatedBy);
            Assert.True(result.CreatedOn <= DateTime.UtcNow);
            Assert.False(result.IsDeleted);
        }

        [Fact]
        public async Task CreateAsync_AutoGeneratesIdIfEmpty()
        {
            // Arrange
            var property = new Property
            {
                Id = Guid.Empty, // Explicitly empty
                Address = "456 Oak Ave",
                City = "Test City",
                State = "TS",
                ZipCode = "12345",
                PropertyType = "Apartment"
            };

            // Act
            var result = await _service.CreateAsync(property);

            // Assert
            Assert.NotEqual(Guid.Empty, result.Id);
        }

        [Fact]
        public async Task CreateAsync_SetsAuditFieldsAutomatically()
        {
            // Arrange
            var property = new Property
            {
                Address = "789 Pine St",
                City = "Test City",
                State = "TS",
                ZipCode = "12345",
                PropertyType = "Condo"
            };
            var beforeCreate = DateTime.UtcNow;

            // Act
            var result = await _service.CreateAsync(property);

            // Assert
            Assert.Equal(_testUserId, result.CreatedBy);
            Assert.True(result.CreatedOn >= beforeCreate);
            Assert.True(result.CreatedOn <= DateTime.UtcNow);
            Assert.Null(result.LastModifiedBy);
            Assert.Null(result.LastModifiedOn);
        }

        [Fact]
        public async Task CreateAsync_SetsOrganizationIdAutomatically()
        {
            // Arrange
            var property = new Property
            {
                OrganizationId = Guid.Empty, // Even if explicitly empty
                Address = "321 Elm St",
                City = "Test City",
                State = "TS",
                ZipCode = "12345",
                PropertyType = "Townhouse"
            };

            // Act
            var result = await _service.CreateAsync(property);

            // Assert
            Assert.Equal(_testOrgId, result.OrganizationId);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_ExistingEntity_ReturnsEntity()
        {
            // Arrange
            var property = new Property
            {
                Id = Guid.NewGuid(),
                OrganizationId = _testOrgId,
                Address = "555 Maple Dr",
                City = "Test City",
                State = "TS",
                ZipCode = "12345",
                PropertyType = "House",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            _context.Properties.Add(property);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetByIdAsync(property.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(property.Id, result.Id);
            Assert.Equal(property.Address, result.Address);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistentEntity_ReturnsNull()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _service.GetByIdAsync(nonExistentId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_SoftDeletedEntity_ReturnsNull()
        {
            // Arrange
            var property = new Property
            {
                Id = Guid.NewGuid(),
                OrganizationId = _testOrgId,
                Address = "777 Birch Ln",
                City = "Test City",
                State = "TS",
                ZipCode = "12345",
                PropertyType = "House",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow,
                IsDeleted = true // Soft deleted
            };
            _context.Properties.Add(property);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetByIdAsync(property.Id);

            // Assert
            Assert.Null(result); // Should not return deleted entities
        }

        [Fact]
        public async Task GetByIdAsync_DifferentOrganization_ReturnsNull()
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
            
            var property = new Property
            {
                Id = Guid.NewGuid(),
                OrganizationId = differentOrgId, // Different organization
                Address = "999 Cedar Ct",
                City = "Test City",
                State = "TS",
                ZipCode = "12345",
                PropertyType = "House",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            _context.Properties.Add(property);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetByIdAsync(property.Id);

            // Assert
            Assert.Null(result); // Should not return entities from other orgs
        }

        #endregion

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_ReturnsAllActiveEntities()
        {
            // Arrange
            var properties = new[]
            {
                new Property { Id = Guid.NewGuid(), OrganizationId = _testOrgId, Address = "100 Test St", City = "City", State = "ST", ZipCode = "12345", PropertyType = "House", CreatedBy = _testUserId, CreatedOn = DateTime.UtcNow },
                new Property { Id = Guid.NewGuid(), OrganizationId = _testOrgId, Address = "200 Test St", City = "City", State = "ST", ZipCode = "12345", PropertyType = "Apartment", CreatedBy = _testUserId, CreatedOn = DateTime.UtcNow },
                new Property { Id = Guid.NewGuid(), OrganizationId = _testOrgId, Address = "300 Test St", City = "City", State = "ST", ZipCode = "12345", PropertyType = "Condo", CreatedBy = _testUserId, CreatedOn = DateTime.UtcNow }
            };
            _context.Properties.AddRange(properties);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public async Task GetAllAsync_ExcludesSoftDeletedEntities()
        {
            // Arrange
            var activeProperty = new Property { Id = Guid.NewGuid(), OrganizationId = _testOrgId, Address = "400 Test St", City = "City", State = "ST", ZipCode = "12345", PropertyType = "House", CreatedBy = _testUserId, CreatedOn = DateTime.UtcNow, IsDeleted = false };
            var deletedProperty = new Property { Id = Guid.NewGuid(), OrganizationId = _testOrgId, Address = "500 Test St", City = "City", State = "ST", ZipCode = "12345", PropertyType = "House", CreatedBy = _testUserId, CreatedOn = DateTime.UtcNow, IsDeleted = true };
            _context.Properties.AddRange(activeProperty, deletedProperty);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal(activeProperty.Id, result[0].Id);
        }

        [Fact]
        public async Task GetAllAsync_FiltersOnlyCurrentOrganization()
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
            
            var myProperty = new Property { Id = Guid.NewGuid(), OrganizationId = _testOrgId, Address = "600 Test St", City = "City", State = "ST", ZipCode = "12345", PropertyType = "House", CreatedBy = _testUserId, CreatedOn = DateTime.UtcNow };
            var otherProperty = new Property { Id = Guid.NewGuid(), OrganizationId = differentOrgId, Address = "700 Test St", City = "City", State = "ST", ZipCode = "12345", PropertyType = "House", CreatedBy = _testUserId, CreatedOn = DateTime.UtcNow };
            _context.Properties.AddRange(myProperty, otherProperty);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal(myProperty.Id, result[0].Id);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_ValidEntity_UpdatesSuccessfully()
        {
            // Arrange
            var property = new Property
            {
                Id = Guid.NewGuid(),
                OrganizationId = _testOrgId,
                Address = "800 Original St",
                City = "Test City",
                State = "TS",
                ZipCode = "12345",
                PropertyType = "House",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            _context.Properties.Add(property);
            await _context.SaveChangesAsync();

            property.Address = "800 Updated St";
            var beforeUpdate = DateTime.UtcNow;

            // Act
            var result = await _service.UpdateAsync(property);

            // Assert
            Assert.Equal("800 Updated St", result.Address);
            Assert.Equal(_testUserId, result.LastModifiedBy);
            Assert.NotNull(result.LastModifiedOn);
            Assert.True(result.LastModifiedOn >= beforeUpdate);
        }

        [Fact]
        public async Task UpdateAsync_SetsLastModifiedFields()
        {
            // Arrange
            var property = new Property
            {
                Id = Guid.NewGuid(),
                OrganizationId = _testOrgId,
                Address = "900 Test St",
                City = "Test City",
                State = "TS",
                ZipCode = "12345",
                PropertyType = "House",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            _context.Properties.Add(property);
            await _context.SaveChangesAsync();

            property.MonthlyRent = 1500m;
            var beforeUpdate = DateTime.UtcNow;

            // Act
            var result = await _service.UpdateAsync(property);

            // Assert
            Assert.Equal(_testUserId, result.LastModifiedBy);
            Assert.NotNull(result.LastModifiedOn);
            Assert.True(result.LastModifiedOn >= beforeUpdate);
            Assert.True(result.LastModifiedOn <= DateTime.UtcNow);
        }

        [Fact]
        public async Task UpdateAsync_NonExistentEntity_ThrowsException()
        {
            // Arrange
            var property = new Property
            {
                Id = Guid.NewGuid(), // Not in database
                OrganizationId = _testOrgId,
                Address = "1000 Test St",
                City = "Test City",
                State = "TS",
                ZipCode = "12345",
                PropertyType = "House"
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(property));
        }

        [Fact]
        public async Task UpdateAsync_DifferentOrganization_ThrowsUnauthorizedException()
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
            
            var property = new Property
            {
                Id = Guid.NewGuid(),
                OrganizationId = differentOrgId,
                Address = "1100 Test St",
                City = "Test City",
                State = "TS",
                ZipCode = "12345",
                PropertyType = "House",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            _context.Properties.Add(property);
            await _context.SaveChangesAsync();

            property.Address = "1100 Updated St";

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.UpdateAsync(property));
        }

        [Fact]
        public async Task UpdateAsync_PreventsOrganizationHijacking()
        {
            // Arrange
            var property = new Property
            {
                Id = Guid.NewGuid(),
                OrganizationId = _testOrgId,
                Address = "1200 Test St",
                City = "Test City",
                State = "TS",
                ZipCode = "12345",
                PropertyType = "House",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            _context.Properties.Add(property);
            await _context.SaveChangesAsync();

            // Detach the entity so we can simulate an external update attempt
            _context.Entry(property).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            // Attempt to change organization on a new instance
            var updatedProperty = new Property
            {
                Id = property.Id,
                OrganizationId = Guid.NewGuid(), // Try to hijack
                Address = "1200 Updated St", // Also update something else
                City = "Test City",
                State = "TS",
                ZipCode = "12345",
                PropertyType = "House",
                CreatedBy = _testUserId,
                CreatedOn = property.CreatedOn
            };

            // Act
            var result = await _service.UpdateAsync(updatedProperty);

            // Assert - OrganizationId should be preserved as original
            Assert.Equal(_testOrgId, result.OrganizationId);
            Assert.Equal("1200 Updated St", result.Address); // Other changes should apply
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_SoftDeleteEnabled_SoftDeletesEntity()
        {
            // Arrange
            var property = new Property
            {
                Id = Guid.NewGuid(),
                OrganizationId = _testOrgId,
                Address = "1300 Test St",
                City = "Test City",
                State = "TS",
                ZipCode = "12345",
                PropertyType = "House",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            _context.Properties.Add(property);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.DeleteAsync(property.Id);

            // Assert
            Assert.True(result);
            var deletedEntity = await _context.Properties.FindAsync(property.Id);
            Assert.NotNull(deletedEntity);
            Assert.True(deletedEntity!.IsDeleted);
            Assert.Equal(_testUserId, deletedEntity.LastModifiedBy);
            Assert.NotNull(deletedEntity.LastModifiedOn);
        }

        [Fact]
        public async Task DeleteAsync_NonExistentEntity_ReturnsFalse()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _service.DeleteAsync(nonExistentId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_DifferentOrganization_ThrowsUnauthorizedException()
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
            
            var property = new Property
            {
                Id = Guid.NewGuid(),
                OrganizationId = differentOrgId,
                Address = "1400 Test St",
                City = "Test City",
                State = "TS",
                ZipCode = "12345",
                PropertyType = "House",
                CreatedBy = _testUserId,
                CreatedOn = DateTime.UtcNow
            };
            _context.Properties.Add(property);
            await _context.SaveChangesAsync();

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.DeleteAsync(property.Id));
        }

        #endregion

        #region Security & Authorization Tests

        [Fact]
        public async Task CreateAsync_UnauthenticatedUser_ThrowsUnauthorizedException()
        {
            // Arrange - Create service with no authenticated user
            var mockAuth = new Mock<AuthenticationStateProvider>();
            var claims = new ClaimsPrincipal(new ClaimsIdentity()); // Not authenticated
            mockAuth.Setup(a => a.GetAuthenticationStateAsync())
                .ReturnsAsync(new AuthenticationState(claims));

            var mockUserStore = new Mock<IUserStore<ApplicationUser>>();
            var mockUserManager = new Mock<UserManager<ApplicationUser>>(
                mockUserStore.Object, null, null, null, null, null, null, null, null);
            mockUserManager.Setup(u => u.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);

            var serviceProvider = new Mock<IServiceProvider>();
            var unauthorizedUserContext = new UserContextService(mockAuth.Object, mockUserManager.Object, serviceProvider.Object);

            var mockSettings = Options.Create(new ApplicationSettings { SoftDeleteEnabled = true });
            var mockLogger = new Mock<ILogger<TestPropertyService>>();
            var unauthorizedService = new TestPropertyService(_context, mockLogger.Object, unauthorizedUserContext, mockSettings);

            var property = new Property
            {
                Address = "1500 Test St",
                City = "Test City",
                State = "TS",
                ZipCode = "12345",
                PropertyType = "House"
            };

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => unauthorizedService.CreateAsync(property));
        }

        #endregion

        /// <summary>
        /// Test implementation of BaseService using Property entity for testing purposes.
        /// </summary>
        public class TestPropertyService : BaseService<Property>
        {
            public TestPropertyService(
                ApplicationDbContext context,
                ILogger<TestPropertyService> logger,
                UserContextService userContext,
                IOptions<ApplicationSettings> settings)
                : base(context, logger, userContext, settings)
            {
            }
        }
    }
}
