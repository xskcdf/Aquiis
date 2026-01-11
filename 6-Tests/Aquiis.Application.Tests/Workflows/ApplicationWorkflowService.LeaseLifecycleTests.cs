using Aquiis.Core.Entities;
using Aquiis.Core.Constants;
using Aquiis.Core.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Aquiis.Infrastructure.Data;
using Aquiis.Application.Services.Workflows;
using Aquiis.Application.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aquiis.Application.Tests;

public class ApplicationWorkflowServiceLeaseLifecycleTests
{
    [Fact]
    public async Task GenerateAndAcceptLeaseOffer_CreatesLeaseAndTenant_UpdatesProperty()
    {
        // Arrange - setup SQLite in-memory
        var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

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

        // Create DbContext and seed data
        await using TestApplicationDbContext context = new TestApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var appUserEntity = new ApplicationUser { Id = testUserId, UserName = "testuser", Email = "t@t.com", ActiveOrganizationId = orgId };
        context.Users.Add(appUserEntity);

        var org = new Organization { Id = orgId, Name = "TestOrg", OwnerId = testUserId, CreatedBy = testUserId, CreatedOn = DateTime.UtcNow };
        context.Organizations.Add(org);

        var prospect = new ProspectiveTenant { Id = Guid.NewGuid(), OrganizationId = orgId, FirstName = "Lease", LastName = "Tester", Email = "lt@example.com", Phone = "123", Status = ApplicationConstants.ProspectiveStatuses.Lead, CreatedBy = testUserId, CreatedOn = DateTime.UtcNow };
        var property = new Property { Id = Guid.NewGuid(), OrganizationId = orgId, Address = "456 Elm", City = "X", State = "ST", ZipCode = "00000", Status = ApplicationConstants.PropertyStatuses.Available, CreatedBy = testUserId, CreatedOn = DateTime.UtcNow, MonthlyRent = 1200m };
        context.ProspectiveTenants.Add(prospect);
        context.Properties.Add(property);
        await context.SaveChangesAsync();

        var noteService = new Application.Services.NoteService(context, mockUserContext.Object);

        // In CreateTestContextAsync()
        var mockEmailService = new Mock<IEmailService>();
        var mockSmsService = new Mock<ISMSService>();

        var notificationService = new NotificationService(
            context,
            mockUserContext.Object,
            mockEmailService.Object,
            mockSmsService.Object,
            Options.Create(new ApplicationSettings { SoftDeleteEnabled = true }),
            Mock.Of<ILogger<NotificationService>>()
        );

        var workflowService = new ApplicationWorkflowService(
            context, 
            mockUserContext.Object, 
            noteService,
            notificationService
        );

        // Submit application
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
        Assert.NotEqual(Guid.Empty, submitResult.Data!.Id);
        var application = submitResult.Data!;

        // Initiate screening and complete it as Passed
        var screeningResult = await workflowService.InitiateScreeningAsync(application.Id, true, true);
        Assert.True(screeningResult.Success, string.Join(";", screeningResult.Errors));
        Assert.NotEqual(Guid.Empty, screeningResult.Data!.Id);

        var completeScreeningResult = await workflowService.CompleteScreeningAsync(application.Id, new ScreeningResultModel
        {
            BackgroundCheckPassed = true,
            CreditCheckPassed = true,
            CreditScore = 700,
            OverallResult = "Passed",
            ResultNotes = "All good"
        });
        Assert.True(completeScreeningResult.Success, string.Join(";", completeScreeningResult.Errors));

        // Approve application
        var approveResult = await workflowService.ApproveApplicationAsync(application.Id);
        Assert.True(approveResult.Success, string.Join(";", approveResult.Errors));

        // Generate lease offer
        var offerModel = new LeaseOfferModel
        {
            StartDate = DateTime.Today.AddDays(14),
            EndDate = DateTime.Today.AddYears(1).AddDays(14),
            MonthlyRent = property.MonthlyRent,
            SecurityDeposit = property.MonthlyRent,
            Terms = "Standard",
            Notes = "Test offer"
        };

        var generateResult = await workflowService.GenerateLeaseOfferAsync(application.Id, offerModel);
        Assert.True(generateResult.Success, string.Join(";", generateResult.Errors));
        var leaseOffer = generateResult.Data!;
        Assert.NotEqual(Guid.Empty, leaseOffer.Id);

        // Accept lease offer
        var acceptResult = await workflowService.AcceptLeaseOfferAsync(leaseOffer.Id, "Card", DateTime.UtcNow);
        Assert.True(acceptResult.Success, string.Join(";", acceptResult.Errors));
        var lease = acceptResult.Data!;

        // Assert: Lease exists in DB, Tenant created, Property status Occupied
        var dbLease = await context.Leases.Include(l => l.Tenant).FirstOrDefaultAsync(l => l.Id == lease.Id);
        Assert.NotNull(dbLease);
        Assert.NotEqual(Guid.Empty, dbLease.Id);
        Assert.NotNull(dbLease!.Tenant);

        var dbProperty = await context.Properties.FirstOrDefaultAsync(p => p.Id == property.Id);
        Assert.NotNull(dbProperty);
        Assert.NotEqual(Guid.Empty, dbProperty.Id);
        Assert.Equal(ApplicationConstants.PropertyStatuses.Occupied, dbProperty!.Status);

        // Audit logs should contain LeaseOffer Accept entry
        var audit = await context.WorkflowAuditLogs.FirstOrDefaultAsync(w => w.EntityType == "LeaseOffer" && w.EntityId == leaseOffer.Id);
        Assert.NotNull(audit);
    }
}
