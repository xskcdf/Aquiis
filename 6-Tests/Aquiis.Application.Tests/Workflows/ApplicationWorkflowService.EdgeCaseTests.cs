using Aquiis.Application.Services.Workflows;
using Aquiis.Core.Entities;
using Aquiis.Core.Constants;
using Aquiis.Core.Interfaces.Services;
using Aquiis.Infrastructure.Data;
using Aquiis.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Aquiis.Application.Services;

namespace Aquiis.Application.Tests;
/// <summary>
/// Edge case tests for ApplicationWorkflowService covering:
/// - Denial flow and property rollback
/// - Withdrawal flow
/// - Competing applications (one approved denies others)
/// - State transition validation
/// - Fee payment validation
/// - Lease offer decline/expire scenarios
/// </summary>
public class ApplicationWorkflowServiceEdgeCaseTests
{
    #region Test Infrastructure

    private static async Task<TestContext> CreateTestContextAsync()
    {
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

        TestApplicationDbContext context = new TestApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var appUserEntity = new ApplicationUser { Id = testUserId, UserName = "testuser", Email = "t@t.com", ActiveOrganizationId = orgId };
        context.Users.Add(appUserEntity);

        var org = new Organization { Id = orgId, Name = "TestOrg", OwnerId = testUserId, CreatedBy = testUserId, CreatedOn = DateTime.UtcNow };
        context.Organizations.Add(org);
        
        // Add OrganizationUser relationship so notifications can find users
        var userOrg = new OrganizationUser 
        { 
            UserId = testUserId, 
            OrganizationId = orgId, 
            IsActive = true,
            CreatedBy = testUserId,
            CreatedOn = DateTime.UtcNow
        };
        context.OrganizationUsers.Add(userOrg);
        
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
            Mock.Of<IHubContext<NotificationHub>>(),
            Mock.Of<ILogger<NotificationService>>()
        );

        var workflowService = new ApplicationWorkflowService(
            context, 
            mockUserContext.Object, 
            noteService,
            notificationService
        );

        return new TestContext
        {
            Connection = connection,
            Context = context,
            WorkflowService = workflowService,
            UserId = testUserId,
            OrgId = orgId
        };
    }

    private static async Task<(ProspectiveTenant Prospect, Property Property)> CreateProspectAndPropertyAsync(
        TestContext ctx, 
        string prospectEmail = "prospect@test.com",
        string propertyAddress = "123 Test St",
        string? identificationNumber = "ID12345",
        string? identificationState = "TX")
    {
        var prospect = new ProspectiveTenant
        {
            Id = Guid.NewGuid(),
            OrganizationId = ctx.OrgId,
            FirstName = "Test",
            LastName = "Prospect",
            Email = prospectEmail,
            Phone = "555-0100",
            IdentificationNumber = identificationNumber,
            IdentificationState = identificationState,
            Status = ApplicationConstants.ProspectiveStatuses.Lead,
            CreatedBy = ctx.UserId,
            CreatedOn = DateTime.UtcNow
        };

        var property = new Property
        {
            Id = Guid.NewGuid(),
            OrganizationId = ctx.OrgId,
            Address = propertyAddress,
            City = "TestCity",
            State = "TS",
            ZipCode = "12345",
            Status = ApplicationConstants.PropertyStatuses.Available,
            MonthlyRent = 1500m,
            CreatedBy = ctx.UserId,
            CreatedOn = DateTime.UtcNow
        };

        ctx.Context.ProspectiveTenants.Add(prospect);
        ctx.Context.Properties.Add(property);
        await ctx.Context.SaveChangesAsync();

        return (prospect, property);
    }

    private static ApplicationSubmissionModel CreateSubmissionModel(bool feePaid = true) => new()
    {
        ApplicationFee = 50m,
        ApplicationFeePaid = feePaid,
        ApplicationFeePaymentMethod = feePaid ? "Card" : null,
        CurrentAddress = "456 Current St",
        CurrentCity = "CurrentCity",
        CurrentState = "CS",
        CurrentZipCode = "54321",
        CurrentRent = 1200m,
        LandlordName = "Previous Landlord",
        LandlordPhone = "555-0200",
        EmployerName = "Test Corp",
        JobTitle = "Developer",
        MonthlyIncome = 5000m,
        EmploymentLengthMonths = 24,
        Reference1Name = "Reference One",
        Reference1Phone = "555-0300",
        Reference1Relationship = "Friend"
    };

    private class TestContext : IAsyncDisposable
    {
        public required Microsoft.Data.Sqlite.SqliteConnection Connection { get; init; }
        public required ApplicationDbContext Context { get; init; }
        public required ApplicationWorkflowService WorkflowService { get; init; }
        public required string UserId { get; init; }
        public required Guid OrgId { get; init; }

        public async ValueTask DisposeAsync()
        {
            await Context.DisposeAsync();
            await Connection.DisposeAsync();
        }
    }

    #endregion

    #region Denial Flow Tests

    [Fact]
    public async Task DenyApplication_UpdatesStatusAndProspect()
    {
        // Arrange
        await using var ctx = await CreateTestContextAsync();
        var (prospect, property) = await CreateProspectAndPropertyAsync(ctx);

        var submitResult = await ctx.WorkflowService.SubmitApplicationAsync(
            prospect.Id, property.Id, CreateSubmissionModel());
        Assert.True(submitResult.Success);
        var application = submitResult.Data!;

        // Act
        var denyResult = await ctx.WorkflowService.DenyApplicationAsync(
            application.Id, "Credit score below minimum threshold");

        // Assert
        Assert.True(denyResult.Success, string.Join(";", denyResult.Errors));

        var dbApp = await ctx.Context.RentalApplications
            .Include(a => a.ProspectiveTenant)
            .FirstOrDefaultAsync(a => a.Id == application.Id);

        Assert.NotNull(dbApp);
        Assert.Equal(ApplicationConstants.ApplicationStatuses.Denied, dbApp.Status);
        Assert.Equal("Credit score below minimum threshold", dbApp.DenialReason);
        Assert.NotNull(dbApp.DecidedOn);
        Assert.Equal(ApplicationConstants.ProspectiveStatuses.Denied, dbApp.ProspectiveTenant!.Status);

        // Verify notifications were created
        var allNotifications = await ctx.Context.Notifications.ToListAsync();
        
        // Should have 2 notifications: one from Submit, one from Deny
        Assert.True(allNotifications.Count >= 1, 
            $"Expected at least one notification. Found: {allNotifications.Count}");
        
        // Find the denial notification (most recent one)
        var denyNotification = allNotifications
            .OrderByDescending(n => n.CreatedOn)
            .FirstOrDefault(n => n.Title.Contains("Denied"));
        
        Assert.NotNull(denyNotification);
        Assert.Contains("Application Denied", denyNotification.Title);
    }

    [Fact]
    public async Task DenyApplication_RequiresDenialReason()
    {
        // Arrange
        await using var ctx = await CreateTestContextAsync();
        var (prospect, property) = await CreateProspectAndPropertyAsync(ctx);

        var submitResult = await ctx.WorkflowService.SubmitApplicationAsync(
            prospect.Id, property.Id, CreateSubmissionModel());
        Assert.True(submitResult.Success);

        // Act
        var denyResult = await ctx.WorkflowService.DenyApplicationAsync(
            submitResult.Data!.Id, "");

        // Assert
        Assert.False(denyResult.Success);
        Assert.Contains("reason is required", denyResult.Errors.First().ToLower());
    }

    [Fact]
    public async Task DenyApplication_CannotDenyAlreadyDenied()
    {
        // Arrange
        await using var ctx = await CreateTestContextAsync();
        var (prospect, property) = await CreateProspectAndPropertyAsync(ctx);

        var submitResult = await ctx.WorkflowService.SubmitApplicationAsync(
            prospect.Id, property.Id, CreateSubmissionModel());
        Assert.True(submitResult.Success);

        var firstDeny = await ctx.WorkflowService.DenyApplicationAsync(
            submitResult.Data!.Id, "First denial");
        Assert.True(firstDeny.Success);

        // Act - try to deny again
        var secondDeny = await ctx.WorkflowService.DenyApplicationAsync(
            submitResult.Data!.Id, "Second denial");

        // Assert
        Assert.False(secondDeny.Success);
        Assert.Contains("Denied", secondDeny.Errors.First());
    }

    #endregion

    #region Property Status Rollback Tests

    [Fact]
    public async Task DenyApplication_RollsBackPropertyStatus_WhenNoOtherActiveApps()
    {
        // Arrange
        await using var ctx = await CreateTestContextAsync();
        var (prospect, property) = await CreateProspectAndPropertyAsync(ctx);

        var submitResult = await ctx.WorkflowService.SubmitApplicationAsync(
            prospect.Id, property.Id, CreateSubmissionModel());
        Assert.True(submitResult.Success);

        // Verify property is now ApplicationPending
        var propertyBefore = await ctx.Context.Properties.FirstAsync(p => p.Id == property.Id);
        Assert.Equal(ApplicationConstants.PropertyStatuses.ApplicationPending, propertyBefore.Status);

        // Act
        var denyResult = await ctx.WorkflowService.DenyApplicationAsync(
            submitResult.Data!.Id, "Denied for test");
        Assert.True(denyResult.Success);

        // Assert - property should roll back to Available
        var propertyAfter = await ctx.Context.Properties.FirstAsync(p => p.Id == property.Id);
        Assert.Equal(ApplicationConstants.PropertyStatuses.Available, propertyAfter.Status);
    }

    [Fact]
    public async Task DenyApplication_KeepsPropertyPending_WhenOtherActiveAppsExist()
    {
        // Arrange
        await using var ctx = await CreateTestContextAsync();
        var (prospect1, property) = await CreateProspectAndPropertyAsync(ctx, "prospect1@test.com");

        // Create second prospect for same property
        var prospect2 = new ProspectiveTenant
        {
            Id = Guid.NewGuid(),
            OrganizationId = ctx.OrgId,
            FirstName = "Second",
            LastName = "Prospect",
            Email = "prospect2@test.com",
            Phone = "555-0101",
            Status = ApplicationConstants.ProspectiveStatuses.Lead,
            CreatedBy = ctx.UserId,
            CreatedOn = DateTime.UtcNow
        };
        ctx.Context.ProspectiveTenants.Add(prospect2);
        await ctx.Context.SaveChangesAsync();

        // Submit two applications for same property
        var submit1 = await ctx.WorkflowService.SubmitApplicationAsync(
            prospect1.Id, property.Id, CreateSubmissionModel());
        Assert.True(submit1.Success);

        var submit2 = await ctx.WorkflowService.SubmitApplicationAsync(
            prospect2.Id, property.Id, CreateSubmissionModel());
        Assert.True(submit2.Success);

        // Act - deny first application
        var denyResult = await ctx.WorkflowService.DenyApplicationAsync(
            submit1.Data!.Id, "Denied for test");
        Assert.True(denyResult.Success);

        // Assert - property should STAY as ApplicationPending (second app still active)
        var propertyAfter = await ctx.Context.Properties.FirstAsync(p => p.Id == property.Id);
        Assert.Equal(ApplicationConstants.PropertyStatuses.ApplicationPending, propertyAfter.Status);
    }

    #endregion

    #region Withdrawal Flow Tests

    [Fact]
    public async Task WithdrawApplication_UpdatesStatusAndRollsBackProperty()
    {
        // Arrange
        await using var ctx = await CreateTestContextAsync();
        var (prospect, property) = await CreateProspectAndPropertyAsync(ctx);

        var submitResult = await ctx.WorkflowService.SubmitApplicationAsync(
            prospect.Id, property.Id, CreateSubmissionModel());
        Assert.True(submitResult.Success);

        // Act
        var withdrawResult = await ctx.WorkflowService.WithdrawApplicationAsync(
            submitResult.Data!.Id, "Changed my mind");

        // Assert
        Assert.True(withdrawResult.Success, string.Join(";", withdrawResult.Errors));

        var dbApp = await ctx.Context.RentalApplications
            .Include(a => a.ProspectiveTenant)
            .FirstOrDefaultAsync(a => a.Id == submitResult.Data!.Id);

        Assert.NotNull(dbApp);
        Assert.Equal(ApplicationConstants.ApplicationStatuses.Withdrawn, dbApp.Status);
        Assert.Equal(ApplicationConstants.ProspectiveStatuses.Withdrawn, dbApp.ProspectiveTenant!.Status);

        var dbProperty = await ctx.Context.Properties.FirstAsync(p => p.Id == property.Id);
        Assert.Equal(ApplicationConstants.PropertyStatuses.Available, dbProperty.Status);
    }

    [Fact]
    public async Task WithdrawApplication_CanWithdrawFromLeaseOfferedState()
    {
        // Arrange
        await using var ctx = await CreateTestContextAsync();
        var (prospect, property) = await CreateProspectAndPropertyAsync(ctx);

        // Submit -> Screen -> Approve -> Generate Offer
        var submitResult = await ctx.WorkflowService.SubmitApplicationAsync(
            prospect.Id, property.Id, CreateSubmissionModel());
        Assert.True(submitResult.Success);
        var application = submitResult.Data!;

        var screenResult = await ctx.WorkflowService.InitiateScreeningAsync(application.Id, true, true);
        Assert.True(screenResult.Success);

        await ctx.WorkflowService.CompleteScreeningAsync(application.Id, new ScreeningResultModel
        {
            BackgroundCheckPassed = true,
            CreditCheckPassed = true,
            CreditScore = 720,
            OverallResult = "Passed"
        });

        var approveResult = await ctx.WorkflowService.ApproveApplicationAsync(application.Id);
        Assert.True(approveResult.Success);

        var offerResult = await ctx.WorkflowService.GenerateLeaseOfferAsync(application.Id, new LeaseOfferModel
        {
            StartDate = DateTime.Today.AddDays(30),
            EndDate = DateTime.Today.AddYears(1).AddDays(30),
            MonthlyRent = 1500m,
            SecurityDeposit = 1500m,
            Terms = "Standard"
        });
        Assert.True(offerResult.Success);

        // Act - withdraw even after lease offer
        var withdrawResult = await ctx.WorkflowService.WithdrawApplicationAsync(
            application.Id, "Found another place");

        // Assert
        Assert.True(withdrawResult.Success, string.Join(";", withdrawResult.Errors));
        
        var dbApp = await ctx.Context.RentalApplications.FirstAsync(a => a.Id == application.Id);
        Assert.Equal(ApplicationConstants.ApplicationStatuses.Withdrawn, dbApp.Status);
    }

    #endregion

    #region Competing Applications Tests

    [Fact]
    public async Task GenerateLeaseOffer_DeniesCompetingApplications()
    {
        // Arrange
        await using var ctx = await CreateTestContextAsync();
        var (prospect1, property) = await CreateProspectAndPropertyAsync(ctx, "winner@test.com");

        // Create two more prospects for the same property
        var prospect2 = new ProspectiveTenant
        {
            Id = Guid.NewGuid(),
            OrganizationId = ctx.OrgId,
            FirstName = "Loser",
            LastName = "One",
            Email = "loser1@test.com",
            Phone = "555-0102",
            Status = ApplicationConstants.ProspectiveStatuses.Lead,
            CreatedBy = ctx.UserId,
            CreatedOn = DateTime.UtcNow
        };

        var prospect3 = new ProspectiveTenant
        {
            Id = Guid.NewGuid(),
            OrganizationId = ctx.OrgId,
            FirstName = "Loser",
            LastName = "Two",
            Email = "loser2@test.com",
            Phone = "555-0103",
            Status = ApplicationConstants.ProspectiveStatuses.Lead,
            CreatedBy = ctx.UserId,
            CreatedOn = DateTime.UtcNow
        };

        ctx.Context.ProspectiveTenants.AddRange(prospect2, prospect3);
        await ctx.Context.SaveChangesAsync();

        // Submit all three applications
        var submit1 = await ctx.WorkflowService.SubmitApplicationAsync(prospect1.Id, property.Id, CreateSubmissionModel());
        var submit2 = await ctx.WorkflowService.SubmitApplicationAsync(prospect2.Id, property.Id, CreateSubmissionModel());
        var submit3 = await ctx.WorkflowService.SubmitApplicationAsync(prospect3.Id, property.Id, CreateSubmissionModel());

        Assert.True(submit1.Success && submit2.Success && submit3.Success);

        // Process first application through to approval
        var app1 = submit1.Data!;
        await ctx.WorkflowService.InitiateScreeningAsync(app1.Id, true, true);
        await ctx.WorkflowService.CompleteScreeningAsync(app1.Id, new ScreeningResultModel
        {
            BackgroundCheckPassed = true,
            CreditCheckPassed = true,
            OverallResult = "Passed"
        });
        await ctx.WorkflowService.ApproveApplicationAsync(app1.Id);

        // Act - generate lease offer (should deny competing apps)
        var offerResult = await ctx.WorkflowService.GenerateLeaseOfferAsync(app1.Id, new LeaseOfferModel
        {
            StartDate = DateTime.Today.AddDays(14),
            EndDate = DateTime.Today.AddYears(1).AddDays(14),
            MonthlyRent = 1500m,
            SecurityDeposit = 1500m,
            Terms = "Standard"
        });

        // Assert
        Assert.True(offerResult.Success);
        Assert.Contains("2 competing", offerResult.Message); // Should mention 2 denied

        // Verify competing apps are denied
        var dbApp2 = await ctx.Context.RentalApplications
            .Include(a => a.ProspectiveTenant)
            .FirstAsync(a => a.Id == submit2.Data!.Id);
        var dbApp3 = await ctx.Context.RentalApplications
            .Include(a => a.ProspectiveTenant)
            .FirstAsync(a => a.Id == submit3.Data!.Id);

        Assert.Equal(ApplicationConstants.ApplicationStatuses.Denied, dbApp2.Status);
        Assert.Equal(ApplicationConstants.ApplicationStatuses.Denied, dbApp3.Status);
        Assert.Equal("Property leased to another applicant", dbApp2.DenialReason);
        Assert.Equal("Property leased to another applicant", dbApp3.DenialReason);

        // Verify prospects are also marked as denied
        Assert.Equal(ApplicationConstants.ProspectiveStatuses.Denied, dbApp2.ProspectiveTenant!.Status);
        Assert.Equal(ApplicationConstants.ProspectiveStatuses.Denied, dbApp3.ProspectiveTenant!.Status);
    }

    #endregion

    #region State Transition Validation Tests

    [Fact]
    public async Task ApproveApplication_FailsWithoutScreeningCompleted()
    {
        // Arrange
        await using var ctx = await CreateTestContextAsync();
        var (prospect, property) = await CreateProspectAndPropertyAsync(ctx);

        var submitResult = await ctx.WorkflowService.SubmitApplicationAsync(
            prospect.Id, property.Id, CreateSubmissionModel());
        Assert.True(submitResult.Success);

        // Act - try to approve without screening
        var approveResult = await ctx.WorkflowService.ApproveApplicationAsync(submitResult.Data!.Id);

        // Assert
        Assert.False(approveResult.Success);
        Assert.Contains("Screening", approveResult.Errors.First());
    }

    [Fact]
    public async Task ApproveApplication_FailsIfScreeningFailed()
    {
        // Arrange
        await using var ctx = await CreateTestContextAsync();
        var (prospect, property) = await CreateProspectAndPropertyAsync(ctx);

        var submitResult = await ctx.WorkflowService.SubmitApplicationAsync(
            prospect.Id, property.Id, CreateSubmissionModel());
        Assert.True(submitResult.Success);
        var application = submitResult.Data!;

        var screenResult = await ctx.WorkflowService.InitiateScreeningAsync(application.Id, true, true);
        Assert.True(screenResult.Success);

        // Complete screening as Failed
        await ctx.WorkflowService.CompleteScreeningAsync(application.Id, new ScreeningResultModel
        {
            BackgroundCheckPassed = false,
            CreditCheckPassed = false,
            OverallResult = "Failed"
        });

        // Act
        var approveResult = await ctx.WorkflowService.ApproveApplicationAsync(application.Id);

        // Assert
        Assert.False(approveResult.Success);
        Assert.Contains("Failed", approveResult.Errors.First());
    }

    [Fact]
    public async Task GenerateLeaseOffer_FailsIfNotApproved()
    {
        // Arrange
        await using var ctx = await CreateTestContextAsync();
        var (prospect, property) = await CreateProspectAndPropertyAsync(ctx);

        var submitResult = await ctx.WorkflowService.SubmitApplicationAsync(
            prospect.Id, property.Id, CreateSubmissionModel());
        Assert.True(submitResult.Success);

        // Act - try to generate offer without approval
        var offerResult = await ctx.WorkflowService.GenerateLeaseOfferAsync(
            submitResult.Data!.Id,
            new LeaseOfferModel
            {
                StartDate = DateTime.Today.AddDays(14),
                EndDate = DateTime.Today.AddYears(1),
                MonthlyRent = 1500m,
                SecurityDeposit = 1500m,
                Terms = "Standard"
            });

        // Assert
        Assert.False(offerResult.Success);
        Assert.Contains("Approved", offerResult.Errors.First());
    }

    [Fact]
    public async Task GenerateLeaseOffer_FailsWithInvalidDates()
    {
        // Arrange
        await using var ctx = await CreateTestContextAsync();
        var (prospect, property) = await CreateProspectAndPropertyAsync(ctx);

        // Go through full approval flow
        var submitResult = await ctx.WorkflowService.SubmitApplicationAsync(prospect.Id, property.Id, CreateSubmissionModel());
        var application = submitResult.Data!;
        await ctx.WorkflowService.InitiateScreeningAsync(application.Id, true, true);
        await ctx.WorkflowService.CompleteScreeningAsync(application.Id, new ScreeningResultModel { OverallResult = "Passed", BackgroundCheckPassed = true, CreditCheckPassed = true });
        await ctx.WorkflowService.ApproveApplicationAsync(application.Id);

        // Act - end date before start date
        var offerResult = await ctx.WorkflowService.GenerateLeaseOfferAsync(
            application.Id,
            new LeaseOfferModel
            {
                StartDate = DateTime.Today.AddYears(1),
                EndDate = DateTime.Today, // Before start
                MonthlyRent = 1500m,
                SecurityDeposit = 1500m,
                Terms = "Standard"
            });

        // Assert
        Assert.False(offerResult.Success);
        Assert.Contains("date", offerResult.Errors.First().ToLower());
    }

    #endregion

    #region Fee Payment Validation Tests

    [Fact]
    public async Task InitiateScreening_FailsIfFeeNotPaid()
    {
        // Arrange
        await using var ctx = await CreateTestContextAsync();
        var (prospect, property) = await CreateProspectAndPropertyAsync(ctx);

        // Submit with fee NOT paid
        var submitResult = await ctx.WorkflowService.SubmitApplicationAsync(
            prospect.Id, property.Id, CreateSubmissionModel(feePaid: false));
        Assert.True(submitResult.Success);

        // Act
        var screenResult = await ctx.WorkflowService.InitiateScreeningAsync(
            submitResult.Data!.Id, true, true);

        // Assert
        Assert.False(screenResult.Success);
        Assert.Contains("fee", screenResult.Errors.First().ToLower());
    }

    #endregion

    #region Lease Offer Decline/Expire Tests

    [Fact]
    public async Task DeclineLeaseOffer_RollsBackPropertyStatus()
    {
        // Arrange
        await using var ctx = await CreateTestContextAsync();
        var (prospect, property) = await CreateProspectAndPropertyAsync(ctx);

        // Full flow to lease offer
        var submitResult = await ctx.WorkflowService.SubmitApplicationAsync(prospect.Id, property.Id, CreateSubmissionModel());
        var application = submitResult.Data!;
        await ctx.WorkflowService.InitiateScreeningAsync(application.Id, true, true);
        await ctx.WorkflowService.CompleteScreeningAsync(application.Id, new ScreeningResultModel { OverallResult = "Passed", BackgroundCheckPassed = true, CreditCheckPassed = true });
        await ctx.WorkflowService.ApproveApplicationAsync(application.Id);

        var offerResult = await ctx.WorkflowService.GenerateLeaseOfferAsync(application.Id, new LeaseOfferModel
        {
            StartDate = DateTime.Today.AddDays(14),
            EndDate = DateTime.Today.AddYears(1).AddDays(14),
            MonthlyRent = 1500m,
            SecurityDeposit = 1500m,
            Terms = "Standard"
        });
        Assert.True(offerResult.Success);

        // Verify property is LeasePending
        var propertyBefore = await ctx.Context.Properties.FirstAsync(p => p.Id == property.Id);
        Assert.Equal(ApplicationConstants.PropertyStatuses.LeasePending, propertyBefore.Status);

        // Act
        var declineResult = await ctx.WorkflowService.DeclineLeaseOfferAsync(
            offerResult.Data!.Id, "Found a better deal");

        // Assert
        Assert.True(declineResult.Success, string.Join(";", declineResult.Errors));

        var dbOffer = await ctx.Context.LeaseOffers.FirstAsync(lo => lo.Id == offerResult.Data!.Id);
        Assert.Equal("Declined", dbOffer.Status);
        Assert.Equal("Found a better deal", dbOffer.ResponseNotes);

        // Property should roll back to Available (no other apps)
        var propertyAfter = await ctx.Context.Properties.FirstAsync(p => p.Id == property.Id);
        Assert.Equal(ApplicationConstants.PropertyStatuses.Available, propertyAfter.Status);
    }

    [Fact]
    public async Task DeclineLeaseOffer_RequiresReason()
    {
        // Arrange
        await using var ctx = await CreateTestContextAsync();
        var (prospect, property) = await CreateProspectAndPropertyAsync(ctx);

        // Full flow to lease offer
        var submitResult = await ctx.WorkflowService.SubmitApplicationAsync(prospect.Id, property.Id, CreateSubmissionModel());
        var application = submitResult.Data!;
        await ctx.WorkflowService.InitiateScreeningAsync(application.Id, true, true);
        await ctx.WorkflowService.CompleteScreeningAsync(application.Id, new ScreeningResultModel { OverallResult = "Passed", BackgroundCheckPassed = true, CreditCheckPassed = true });
        await ctx.WorkflowService.ApproveApplicationAsync(application.Id);

        var offerResult = await ctx.WorkflowService.GenerateLeaseOfferAsync(application.Id, new LeaseOfferModel
        {
            StartDate = DateTime.Today.AddDays(14),
            EndDate = DateTime.Today.AddYears(1),
            MonthlyRent = 1500m,
            SecurityDeposit = 1500m,
            Terms = "Standard"
        });

        // Act
        var declineResult = await ctx.WorkflowService.DeclineLeaseOfferAsync(offerResult.Data!.Id, "");

        // Assert
        Assert.False(declineResult.Success);
        Assert.Contains("reason is required", declineResult.Errors.First().ToLower());
    }

    [Fact]
    public async Task AcceptLeaseOffer_FailsIfExpired()
    {
        // Arrange
        await using var ctx = await CreateTestContextAsync();
        var (prospect, property) = await CreateProspectAndPropertyAsync(ctx);

        // Full flow to lease offer
        var submitResult = await ctx.WorkflowService.SubmitApplicationAsync(prospect.Id, property.Id, CreateSubmissionModel());
        var application = submitResult.Data!;
        await ctx.WorkflowService.InitiateScreeningAsync(application.Id, true, true);
        await ctx.WorkflowService.CompleteScreeningAsync(application.Id, new ScreeningResultModel { OverallResult = "Passed", BackgroundCheckPassed = true, CreditCheckPassed = true });
        await ctx.WorkflowService.ApproveApplicationAsync(application.Id);

        var offerResult = await ctx.WorkflowService.GenerateLeaseOfferAsync(application.Id, new LeaseOfferModel
        {
            StartDate = DateTime.Today.AddDays(14),
            EndDate = DateTime.Today.AddYears(1),
            MonthlyRent = 1500m,
            SecurityDeposit = 1500m,
            Terms = "Standard"
        });
        Assert.True(offerResult.Success);

        // Manually expire the offer by setting ExpiresOn to past
        var offer = await ctx.Context.LeaseOffers.FirstAsync(lo => lo.Id == offerResult.Data!.Id);
        offer.ExpiresOn = DateTime.UtcNow.AddDays(-1);
        await ctx.Context.SaveChangesAsync();

        // Act
        var acceptResult = await ctx.WorkflowService.AcceptLeaseOfferAsync(
            offer.Id, "Card", DateTime.UtcNow);

        // Assert
        Assert.False(acceptResult.Success);
        Assert.Contains("expired", acceptResult.Errors.First().ToLower());
    }

    #endregion

    #region Duplicate Application Prevention Tests

    [Fact]
    public async Task SubmitApplication_FailsIfProspectHasActiveApplication()
    {
        // Arrange - prospects with same identification number/state should be blocked
        await using var ctx = await CreateTestContextAsync();
        var (prospect, property1) = await CreateProspectAndPropertyAsync(
            ctx, "prospect@test.com", "123 First St", "DL123456", "TX");

        // Create second property
        var property2 = new Property
        {
            Id = Guid.NewGuid(),
            OrganizationId = ctx.OrgId,
            Address = "456 Second St",
            City = "TestCity",
            State = "TS",
            ZipCode = "12345",
            Status = ApplicationConstants.PropertyStatuses.Available,
            MonthlyRent = 1600m,
            CreatedBy = ctx.UserId,
            CreatedOn = DateTime.UtcNow
        };
        ctx.Context.Properties.Add(property2);
        await ctx.Context.SaveChangesAsync();

        // Submit first application
        var submit1 = await ctx.WorkflowService.SubmitApplicationAsync(
            prospect.Id, property1.Id, CreateSubmissionModel());
        Assert.True(submit1.Success);

        // Act - try to submit second application for same prospect (same identification)
        var submit2 = await ctx.WorkflowService.SubmitApplicationAsync(
            prospect.Id, property2.Id, CreateSubmissionModel());

        // Assert - should fail because same identification has an active application
        Assert.False(submit2.Success);
        Assert.Contains("active application", submit2.Errors.First().ToLower());
    }

    [Fact]
    public async Task SubmitApplication_SucceedsForNewProspect_AfterPreviousProspectDenied()
    {
        // Arrange
        await using var ctx = await CreateTestContextAsync();
        
        // Create first prospect and apply
        var (prospect1, property) = await CreateProspectAndPropertyAsync(ctx, "prospect1@test.com", "123 First St");
        var submit1 = await ctx.WorkflowService.SubmitApplicationAsync(
            prospect1.Id, property.Id, CreateSubmissionModel());
        Assert.True(submit1.Success);

        // Deny first application
        await ctx.WorkflowService.DenyApplicationAsync(submit1.Data!.Id, "Denied for test");

        // Clear EF Core change tracker
        ctx.Context.ChangeTracker.Clear();

        // Create a second prospect with different identification
        var prospect2 = new ProspectiveTenant
        {
            Id = Guid.NewGuid(),
            OrganizationId = ctx.OrgId,
            FirstName = "Second",
            LastName = "Prospect",
            Email = "prospect2@test.com",
            Phone = "555-0200",
            IdentificationNumber = "DL999999",
            IdentificationState = "CA",
            Status = ApplicationConstants.ProspectiveStatuses.Lead,
            CreatedBy = ctx.UserId,
            CreatedOn = DateTime.UtcNow
        };
        ctx.Context.ProspectiveTenants.Add(prospect2);
        await ctx.Context.SaveChangesAsync();

        // Clear again before second submission
        ctx.Context.ChangeTracker.Clear();

        // Act - second prospect applies for same property
        var submit2 = await ctx.WorkflowService.SubmitApplicationAsync(
            prospect2.Id, property.Id, CreateSubmissionModel());

        // Assert
        Assert.True(submit2.Success, string.Join(";", submit2.Errors));
        
        // Property should be back to ApplicationPending (new application)
        var dbProperty = await ctx.Context.Properties.FirstAsync(p => p.Id == property.Id);
        Assert.Equal(ApplicationConstants.PropertyStatuses.ApplicationPending, dbProperty.Status);
    }

    [Fact]
    public async Task SubmitApplication_SameIdentification_SucceedsAfterPreviousApplicationDenied()
    {
        // Arrange - same identification can reapply after denial
        await using var ctx = await CreateTestContextAsync();
        var (prospect, property1) = await CreateProspectAndPropertyAsync(
            ctx, "prospect@test.com", "123 First St", "DL123456", "TX");

        // Create second property for new application
        var property2 = new Property
        {
            Id = Guid.NewGuid(),
            OrganizationId = ctx.OrgId,
            Address = "456 Second St",
            City = "TestCity",
            State = "TS",
            ZipCode = "12345",
            Status = ApplicationConstants.PropertyStatuses.Available,
            MonthlyRent = 1600m,
            CreatedBy = ctx.UserId,
            CreatedOn = DateTime.UtcNow
        };
        ctx.Context.Properties.Add(property2);
        await ctx.Context.SaveChangesAsync();

        // Submit and deny first application
        var submit1 = await ctx.WorkflowService.SubmitApplicationAsync(
            prospect.Id, property1.Id, CreateSubmissionModel());
        Assert.True(submit1.Success, "First application should succeed");

        await ctx.WorkflowService.DenyApplicationAsync(submit1.Data!.Id, "Denied for test");

        // Clear EF Core change tracker
        ctx.Context.ChangeTracker.Clear();

        // Act - same prospect reapplies for different property after denial
        var submit2 = await ctx.WorkflowService.SubmitApplicationAsync(
            prospect.Id, property2.Id, CreateSubmissionModel());

        // Assert - should succeed because previous application was denied (disposed)
        Assert.True(submit2.Success, $"Reapplication after denial should succeed. Errors: {string.Join(";", submit2.Errors)}");
    }

    #endregion

    #region Audit Trail Tests

    [Fact]
    public async Task WorkflowOperations_CreateAuditTrail()
    {
        // Arrange
        await using var ctx = await CreateTestContextAsync();
        var (prospect, property) = await CreateProspectAndPropertyAsync(ctx);

        // Act - perform multiple operations
        var submitResult = await ctx.WorkflowService.SubmitApplicationAsync(
            prospect.Id, property.Id, CreateSubmissionModel());
        Assert.True(submitResult.Success);
        var application = submitResult.Data!;

        await ctx.WorkflowService.InitiateScreeningAsync(application.Id, true, true);
        await ctx.WorkflowService.CompleteScreeningAsync(application.Id, new ScreeningResultModel
        {
            OverallResult = "Passed",
            BackgroundCheckPassed = true,
            CreditCheckPassed = true
        });
        await ctx.WorkflowService.ApproveApplicationAsync(application.Id);

        // Assert - check audit logs
        var auditLogs = await ctx.Context.WorkflowAuditLogs
            .Where(w => w.EntityType == "RentalApplication" && w.EntityId == application.Id)
            .OrderBy(w => w.PerformedOn)
            .ToListAsync();

        Assert.True(auditLogs.Count >= 3); // Submit, Screening, Approve
        
        // Verify submit log
        var submitLog = auditLogs.First(l => l.Action == "SubmitApplication");
        Assert.Equal(ApplicationConstants.ApplicationStatuses.Submitted, submitLog.ToStatus);

        // Verify approve log
        var approveLog = auditLogs.First(l => l.Action == "ApproveApplication");
        Assert.Equal(ApplicationConstants.ApplicationStatuses.Screening, approveLog.FromStatus);
        Assert.Equal(ApplicationConstants.ApplicationStatuses.Approved, approveLog.ToStatus);
    }

    #endregion
}
