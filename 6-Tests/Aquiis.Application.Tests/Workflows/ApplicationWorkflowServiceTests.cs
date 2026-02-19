using Aquiis.Core.Entities;
using Aquiis.Core.Constants;
using Aquiis.Core.Interfaces.Services;
using Aquiis.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Aquiis.Infrastructure.Data;
using Aquiis.SimpleStart.Entities;
using Aquiis.Application.Services.Workflows;
using Aquiis.Application.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aquiis.Application.Tests;

public class ApplicationWorkflowServiceTests
{
    [Fact]
    public async Task GetApplicationWorkflowStateAsync_ReturnsExpectedState()
    {
        // Arrange
        // Use SQLite in-memory to support transactions used by workflow base class
        var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;
        // Create test user and org
        var testUserId = "test-user-id";
        var orgId = Guid.NewGuid();

        // Mock IUserContextService
        var mockUserContext = new Mock<IUserContextService>();
        mockUserContext.Setup(x => x.GetUserIdAsync())
            .ReturnsAsync(testUserId);
        mockUserContext.Setup(x => x.GetActiveOrganizationIdAsync())
            .ReturnsAsync(orgId);
        mockUserContext.Setup(x => x.GetUserNameAsync())
            .ReturnsAsync("testuser");
        mockUserContext.Setup(x => x.GetUserEmailAsync())
            .ReturnsAsync("t@t.com");
        mockUserContext.Setup(x => x.GetOrganizationIdAsync())
            .ReturnsAsync(orgId);

        // Create DbContext and seed prospect/property
        await using TestApplicationDbContext context = new TestApplicationDbContext(options);
        // Ensure schema is created for SQLite in-memory
        await context.Database.EnsureCreatedAsync();
        var appUserEntity = new ApplicationUser { Id = testUserId, UserName = "testuser", Email = "t@t.com", ActiveOrganizationId = orgId };
        context.Users.Add(appUserEntity);

        var org = new Organization { Id = orgId, Name = "TestOrg", OwnerId = testUserId, CreatedBy = testUserId, CreatedOn = DateTime.UtcNow };
        context.Organizations.Add(org);

        var prospect = new ProspectiveTenant { Id = Guid.NewGuid(), OrganizationId = orgId, FirstName = "Test", LastName = "User", Email = "t@t.com", Phone = "123", Status = "Lead", CreatedBy = testUserId, CreatedOn = DateTime.UtcNow };
        var property = new Property { Id = Guid.NewGuid(), OrganizationId = orgId, Address = "123 Main", City = "X", State = "ST", ZipCode = "00000", Status = ApplicationConstants.PropertyStatuses.Available, CreatedBy = testUserId, CreatedOn = DateTime.UtcNow };
        context.ProspectiveTenants.Add(prospect);
        context.Properties.Add(property);
        await context.SaveChangesAsync();

        // Create NoteService (not used heavily in this test)
        var noteService = new Application.Services.NoteService(context, mockUserContext.Object);

        // Create ApplicationWorkflowService
        // In CreateTestContextAsync()
        var mockEmailService = new Mock<IEmailService>();
        var mockSmsService = new Mock<ISMSService>();

        var notificationService = new NotificationService(
            context,
            mockUserContext.Object,
            mockEmailService.Object,
            mockSmsService.Object,
            Options.Create(new ApplicationSettings { SoftDeleteEnabled = true }),
            Mock.Of<IHubContext<NotificationHub>>(),
            Mock.Of<ILogger<NotificationService>>()
        );

        var workflowService = new ApplicationWorkflowService(
            context, 
            mockUserContext.Object, 
            noteService,
            notificationService
        );

        // Act - submit application then initiate screening
        var submissionModel = new ApplicationSubmissionModel
        {
            ApplicationFee = 25m,
            ApplicationFeePaid = true,
            ApplicationFeePaymentMethod = "Card",
            CurrentAddress = "Addr",
            CurrentCity = "C",
            CurrentState = "ST",
            CurrentZipCode = "00000",
            CurrentRent = 1000m,
            LandlordName = "L",
            LandlordPhone = "P",
            EmployerName = "E",
            JobTitle = "J",
            MonthlyIncome = 2000m,
            EmploymentLengthMonths = 12,
            Reference1Name = "R1",
            Reference1Phone = "111",
            Reference1Relationship = "Friend"
        };

        var submitResult = await workflowService.SubmitApplicationAsync(prospect.Id, property.Id, submissionModel);
        Assert.True(submitResult.Success, string.Join(";", submitResult.Errors));

        var application = submitResult.Data!;

        var screeningResult = await workflowService.InitiateScreeningAsync(application.Id, true, true);
        Assert.True(screeningResult.Success, string.Join(";", screeningResult.Errors));

        // Get aggregated workflow state
        var state = await workflowService.GetApplicationWorkflowStateAsync(application.Id);

        // Assert
        Assert.NotNull(state.Application);
        Assert.NotEqual(Guid.Empty, state.Application.Id);
        Assert.NotNull(state.Prospect);
        Assert.NotEqual(Guid.Empty, state.Prospect.Id);
        Assert.NotNull(state.Property);
        Assert.NotEqual(Guid.Empty, state.Property.Id);
        Assert.NotNull(state.Screening);
        Assert.NotEqual(Guid.Empty, state.Screening.Id);
        Assert.NotEmpty(state.AuditHistory);
        Assert.All(state.AuditHistory, item => Assert.NotEqual(Guid.Empty, item.Id));

    }
}
