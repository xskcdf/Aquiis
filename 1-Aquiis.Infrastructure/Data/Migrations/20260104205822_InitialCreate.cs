using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Aquiis.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CalendarSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EntityType = table.Column<string>(type: "TEXT", nullable: false),
                    AutoCreateEvents = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowOnCalendar = table.Column<bool>(type: "INTEGER", nullable: false),
                    DefaultColor = table.Column<string>(type: "TEXT", nullable: true),
                    DefaultIcon = table.Column<string>(type: "TEXT", nullable: true),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Category = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    IsSystemTemplate = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", maxLength: 100, nullable: false),
                    Content = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: false),
                    EntityType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EntityId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserFullName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OwnerId = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    State = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "TEXT", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    LateFeeEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LateFeeAutoApply = table.Column<bool>(type: "INTEGER", nullable: false),
                    LateFeeGracePeriodDays = table.Column<int>(type: "INTEGER", nullable: false),
                    LateFeePercentage = table.Column<decimal>(type: "TEXT", precision: 5, scale: 4, nullable: false),
                    MaxLateFeeAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    PaymentReminderEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    PaymentReminderDaysBefore = table.Column<int>(type: "INTEGER", nullable: false),
                    TourNoShowGracePeriodHours = table.Column<int>(type: "INTEGER", nullable: false),
                    ApplicationFeeEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    DefaultApplicationFee = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    ApplicationExpirationDays = table.Column<int>(type: "INTEGER", nullable: false),
                    SecurityDepositInvestmentEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    OrganizationSharePercentage = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    AutoCalculateSecurityDeposit = table.Column<bool>(type: "INTEGER", nullable: false),
                    SecurityDepositMultiplier = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RefundProcessingDays = table.Column<int>(type: "INTEGER", nullable: false),
                    DividendDistributionMonth = table.Column<int>(type: "INTEGER", nullable: false),
                    AllowTenantDividendChoice = table.Column<bool>(type: "INTEGER", nullable: false),
                    DefaultDividendPaymentMethod = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SchemaVersions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Version = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    AppliedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchemaVersions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SecurityDepositInvestmentPools",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    StartingBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EndingBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalEarnings = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReturnRate = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    OrganizationSharePercentage = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    OrganizationShare = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TenantShareTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ActiveLeaseCount = table.Column<int>(type: "INTEGER", nullable: false),
                    DividendPerLease = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DividendsCalculatedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DividendsDistributedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityDepositInvestmentPools", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowAuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    EntityType = table.Column<string>(type: "TEXT", nullable: false),
                    EntityId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FromStatus = table.Column<string>(type: "TEXT", nullable: true),
                    ToStatus = table.Column<string>(type: "TEXT", nullable: false),
                    Action = table.Column<string>(type: "TEXT", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", nullable: true),
                    PerformedBy = table.Column<string>(type: "TEXT", nullable: false),
                    PerformedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Metadata = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowAuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistTemplateItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ChecklistTemplateId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ItemText = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    ItemOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CategorySection = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    SectionOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsRequired = table.Column<bool>(type: "INTEGER", nullable: false),
                    RequiresValue = table.Column<bool>(type: "INTEGER", nullable: false),
                    AllowsNotes = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistTemplateItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistTemplateItems_ChecklistTemplates_ChecklistTemplateId",
                        column: x => x.ChecklistTemplateId,
                        principalTable: "ChecklistTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificationPreferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    EnableInAppNotifications = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableEmailNotifications = table.Column<bool>(type: "INTEGER", nullable: false),
                    EmailAddress = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    EmailLeaseExpiring = table.Column<bool>(type: "INTEGER", nullable: false),
                    EmailPaymentDue = table.Column<bool>(type: "INTEGER", nullable: false),
                    EmailPaymentReceived = table.Column<bool>(type: "INTEGER", nullable: false),
                    EmailApplicationStatusChange = table.Column<bool>(type: "INTEGER", nullable: false),
                    EmailMaintenanceUpdate = table.Column<bool>(type: "INTEGER", nullable: false),
                    EmailInspectionScheduled = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableSMSNotifications = table.Column<bool>(type: "INTEGER", nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    SMSPaymentDue = table.Column<bool>(type: "INTEGER", nullable: false),
                    SMSMaintenanceEmergency = table.Column<bool>(type: "INTEGER", nullable: false),
                    SMSLeaseExpiringUrgent = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableDailyDigest = table.Column<bool>(type: "INTEGER", nullable: false),
                    DailyDigestTime = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    EnableWeeklyDigest = table.Column<bool>(type: "INTEGER", nullable: false),
                    WeeklyDigestDay = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationPreferences_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    RecipientUserId = table.Column<string>(type: "TEXT", nullable: false),
                    SentOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ReadOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsRead = table.Column<bool>(type: "INTEGER", nullable: false),
                    RelatedEntityId = table.Column<Guid>(type: "TEXT", nullable: true),
                    RelatedEntityType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    SendInApp = table.Column<bool>(type: "INTEGER", nullable: false),
                    SendEmail = table.Column<bool>(type: "INTEGER", nullable: false),
                    SendSMS = table.Column<bool>(type: "INTEGER", nullable: false),
                    EmailSent = table.Column<bool>(type: "INTEGER", nullable: false),
                    EmailSentOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SMSSent = table.Column<bool>(type: "INTEGER", nullable: false),
                    SMSSentOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EmailError = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    SMSError = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationEmailSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProviderName = table.Column<string>(type: "TEXT", nullable: false),
                    SmtpServer = table.Column<string>(type: "TEXT", nullable: false),
                    SmtpPort = table.Column<int>(type: "INTEGER", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    EnableSsl = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsEmailEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    SendGridApiKeyEncrypted = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    FromEmail = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    FromName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    EmailsSentToday = table.Column<int>(type: "INTEGER", nullable: false),
                    EmailsSentThisMonth = table.Column<int>(type: "INTEGER", nullable: false),
                    LastEmailSentOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    StatsLastUpdatedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DailyCountResetOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    MonthlyCountResetOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DailyLimit = table.Column<int>(type: "INTEGER", nullable: true),
                    MonthlyLimit = table.Column<int>(type: "INTEGER", nullable: true),
                    PlanType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsVerified = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastVerifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastError = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    LastErrorOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationEmailSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationEmailSettings_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationSMSSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsSMSEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    ProviderName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    TwilioAccountSidEncrypted = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    TwilioAuthTokenEncrypted = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    TwilioPhoneNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    SMSSentToday = table.Column<int>(type: "INTEGER", nullable: false),
                    SMSSentThisMonth = table.Column<int>(type: "INTEGER", nullable: false),
                    LastSMSSentOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    StatsLastUpdatedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DailyCountResetOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    MonthlyCountResetOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AccountBalance = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true),
                    CostPerSMS = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: true),
                    AccountType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsVerified = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastVerifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastError = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationSMSSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationSMSSettings_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Properties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    UnitNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    City = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ZipCode = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    PropertyType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    MonthlyRent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Bedrooms = table.Column<int>(type: "INTEGER", maxLength: 3, nullable: false),
                    Bathrooms = table.Column<decimal>(type: "decimal(3,1)", maxLength: 3, nullable: false),
                    SquareFeet = table.Column<int>(type: "INTEGER", maxLength: 7, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    IsAvailable = table.Column<bool>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    LastRoutineInspectionDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    NextRoutineInspectionDueDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RoutineInspectionIntervalMonths = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Properties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Properties_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    IdentificationNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    EmergencyContactName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    EmergencyContactPhone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    ProspectiveTenantId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tenants_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserOrganizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Role = table.Column<string>(type: "TEXT", nullable: false),
                    GrantedBy = table.Column<string>(type: "TEXT", nullable: false),
                    GrantedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RevokedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "TEXT", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOrganizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserOrganizations_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CalendarEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    StartOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DurationMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    EventType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    PropertyId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Location = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Color = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    SourceEntityId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SourceEntityType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalendarEvents_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ProspectiveTenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", maxLength: 100, nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IdentificationNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IdentificationState = table.Column<string>(type: "TEXT", maxLength: 2, nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Source = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    InterestedPropertyId = table.Column<Guid>(type: "TEXT", nullable: true),
                    DesiredMoveInDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FirstContactedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProspectiveTenants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProspectiveTenants_Properties_InterestedPropertyId",
                        column: x => x.InterestedPropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "RentalApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", maxLength: 100, nullable: false),
                    ProspectiveTenantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PropertyId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AppliedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CurrentAddress = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    CurrentCity = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CurrentState = table.Column<string>(type: "TEXT", maxLength: 2, nullable: false),
                    CurrentZipCode = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    CurrentRent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LandlordName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    LandlordPhone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    EmployerName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    JobTitle = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    MonthlyIncome = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EmploymentLengthMonths = table.Column<int>(type: "INTEGER", nullable: false),
                    Reference1Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Reference1Phone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Reference1Relationship = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Reference2Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Reference2Phone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Reference2Relationship = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ApplicationFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ApplicationFeePaid = table.Column<bool>(type: "INTEGER", nullable: false),
                    ApplicationFeePaidOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ApplicationFeePaymentMethod = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    ExpiresOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DenialReason = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    DecidedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DecisionBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RentalApplications_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RentalApplications_ProspectiveTenants_ProspectiveTenantId",
                        column: x => x.ProspectiveTenantId,
                        principalTable: "ProspectiveTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationScreenings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RentalApplicationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    BackgroundCheckRequested = table.Column<bool>(type: "INTEGER", nullable: false),
                    BackgroundCheckRequestedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    BackgroundCheckPassed = table.Column<bool>(type: "INTEGER", nullable: true),
                    BackgroundCheckCompletedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    BackgroundCheckNotes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreditCheckRequested = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreditCheckRequestedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreditScore = table.Column<int>(type: "INTEGER", nullable: true),
                    CreditCheckPassed = table.Column<bool>(type: "INTEGER", nullable: true),
                    CreditCheckCompletedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreditCheckNotes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    OverallResult = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ResultNotes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationScreenings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationScreenings_RentalApplications_RentalApplicationId",
                        column: x => x.RentalApplicationId,
                        principalTable: "RentalApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeaseOffers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", maxLength: 100, nullable: false),
                    RentalApplicationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PropertyId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProspectiveTenantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MonthlyRent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SecurityDeposit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Terms = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    OfferedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExpiresOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    RespondedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ResponseNotes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ConvertedLeaseId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaseOffers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaseOffers_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeaseOffers_ProspectiveTenants_ProspectiveTenantId",
                        column: x => x.ProspectiveTenantId,
                        principalTable: "ProspectiveTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeaseOffers_RentalApplications_RentalApplicationId",
                        column: x => x.RentalApplicationId,
                        principalTable: "RentalApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ChecklistId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ItemText = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    ItemOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CategorySection = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    SectionOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    RequiresValue = table.Column<bool>(type: "INTEGER", nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    PhotoUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsChecked = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Checklists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PropertyId = table.Column<Guid>(type: "TEXT", nullable: true),
                    LeaseId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ChecklistTemplateId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ChecklistType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CompletedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CompletedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DocumentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    GeneralNotes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Checklists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Checklists_ChecklistTemplates_ChecklistTemplateId",
                        column: x => x.ChecklistTemplateId,
                        principalTable: "ChecklistTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Checklists_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tours",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProspectiveTenantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PropertyId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ScheduledOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DurationMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Feedback = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    InterestLevel = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    ConductedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ChecklistId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CalendarEventId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tours_Checklists_ChecklistId",
                        column: x => x.ChecklistId,
                        principalTable: "Checklists",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tours_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tours_ProspectiveTenants_ProspectiveTenantId",
                        column: x => x.ProspectiveTenantId,
                        principalTable: "ProspectiveTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", maxLength: 100, nullable: false),
                    FileName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    FileExtension = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    FileData = table.Column<byte[]>(type: "BLOB", nullable: false),
                    FilePath = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    FileType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    FileSize = table.Column<long>(type: "INTEGER", nullable: false),
                    DocumentType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    PropertyId = table.Column<Guid>(type: "TEXT", nullable: true),
                    TenantId = table.Column<Guid>(type: "TEXT", nullable: true),
                    LeaseId = table.Column<Guid>(type: "TEXT", nullable: true),
                    InvoiceId = table.Column<Guid>(type: "TEXT", nullable: true),
                    PaymentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Documents_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Documents_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Leases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PropertyId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TenantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LeaseOfferId = table.Column<Guid>(type: "TEXT", nullable: true),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MonthlyRent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SecurityDeposit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Terms = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    OfferedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SignedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeclinedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ExpiresOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RenewalNotificationSent = table.Column<bool>(type: "INTEGER", nullable: true),
                    RenewalNotificationSentOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RenewalReminderSentOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RenewalStatus = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    RenewalOfferedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RenewalResponseOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ProposedRenewalRent = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RenewalNotes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    PreviousLeaseId = table.Column<Guid>(type: "TEXT", nullable: true),
                    RenewalNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    TerminationNoticedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ExpectedMoveOutDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ActualMoveOutDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TerminationReason = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    DocumentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Leases_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Leases_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Leases_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Leases_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Inspections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", maxLength: 100, nullable: false),
                    PropertyId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CalendarEventId = table.Column<Guid>(type: "TEXT", nullable: true),
                    LeaseId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CompletedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    InspectionType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    InspectedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ExteriorRoofGood = table.Column<bool>(type: "INTEGER", nullable: false),
                    ExteriorRoofNotes = table.Column<string>(type: "TEXT", nullable: true),
                    ExteriorGuttersGood = table.Column<bool>(type: "INTEGER", nullable: false),
                    ExteriorGuttersNotes = table.Column<string>(type: "TEXT", nullable: true),
                    ExteriorSidingGood = table.Column<bool>(type: "INTEGER", nullable: false),
                    ExteriorSidingNotes = table.Column<string>(type: "TEXT", nullable: true),
                    ExteriorWindowsGood = table.Column<bool>(type: "INTEGER", nullable: false),
                    ExteriorWindowsNotes = table.Column<string>(type: "TEXT", nullable: true),
                    ExteriorDoorsGood = table.Column<bool>(type: "INTEGER", nullable: false),
                    ExteriorDoorsNotes = table.Column<string>(type: "TEXT", nullable: true),
                    ExteriorFoundationGood = table.Column<bool>(type: "INTEGER", nullable: false),
                    ExteriorFoundationNotes = table.Column<string>(type: "TEXT", nullable: true),
                    LandscapingGood = table.Column<bool>(type: "INTEGER", nullable: false),
                    LandscapingNotes = table.Column<string>(type: "TEXT", nullable: true),
                    InteriorWallsGood = table.Column<bool>(type: "INTEGER", nullable: false),
                    InteriorWallsNotes = table.Column<string>(type: "TEXT", nullable: true),
                    InteriorCeilingsGood = table.Column<bool>(type: "INTEGER", nullable: false),
                    InteriorCeilingsNotes = table.Column<string>(type: "TEXT", nullable: true),
                    InteriorFloorsGood = table.Column<bool>(type: "INTEGER", nullable: false),
                    InteriorFloorsNotes = table.Column<string>(type: "TEXT", nullable: true),
                    InteriorDoorsGood = table.Column<bool>(type: "INTEGER", nullable: false),
                    InteriorDoorsNotes = table.Column<string>(type: "TEXT", nullable: true),
                    InteriorWindowsGood = table.Column<bool>(type: "INTEGER", nullable: false),
                    InteriorWindowsNotes = table.Column<string>(type: "TEXT", nullable: true),
                    KitchenAppliancesGood = table.Column<bool>(type: "INTEGER", nullable: false),
                    KitchenAppliancesNotes = table.Column<string>(type: "TEXT", nullable: true),
                    KitchenCabinetsGood = table.Column<bool>(type: "INTEGER", nullable: false),
                    KitchenCabinetsNotes = table.Column<string>(type: "TEXT", nullable: true),
                    KitchenCountersGood = table.Column<bool>(type: "INTEGER", nullable: false),
                    KitchenCountersNotes = table.Column<string>(type: "TEXT", nullable: true),
                    KitchenSinkPlumbingGood = table.Column<bool>(type: "INTEGER", nullable: false),
                    KitchenSinkPlumbingNotes = table.Column<string>(type: "TEXT", nullable: true),
                    BathroomToiletGood = table.Column<bool>(type: "INTEGER", nullable: false),
                    BathroomToiletNotes = table.Column<string>(type: "TEXT", nullable: true),
                    BathroomSinkGood = table.Column<bool>(type: "INTEGER", nullable: false),
                    BathroomSinkNotes = table.Column<string>(type: "TEXT", nullable: true),
                    BathroomTubShowerGood = table.Column<bool>(type: "INTEGER", nullable: false),
                    BathroomTubShowerNotes = table.Column<string>(type: "TEXT", nullable: true),
                    BathroomVentilationGood = table.Column<bool>(type: "INTEGER", nullable: false),
                    BathroomVentilationNotes = table.Column<string>(type: "TEXT", nullable: true),
                    HvacSystemGood = table.Column<bool>(type: "INTEGER", nullable: false),
                    HvacSystemNotes = table.Column<string>(type: "TEXT", nullable: true),
                    ElectricalSystemGood = table.Column<bool>(type: "INTEGER", nullable: false),
                    ElectricalSystemNotes = table.Column<string>(type: "TEXT", nullable: true),
                    PlumbingSystemGood = table.Column<bool>(type: "INTEGER", nullable: false),
                    PlumbingSystemNotes = table.Column<string>(type: "TEXT", nullable: true),
                    SmokeDetectorsGood = table.Column<bool>(type: "INTEGER", nullable: false),
                    SmokeDetectorsNotes = table.Column<string>(type: "TEXT", nullable: true),
                    CarbonMonoxideDetectorsGood = table.Column<bool>(type: "INTEGER", nullable: false),
                    CarbonMonoxideDetectorsNotes = table.Column<string>(type: "TEXT", nullable: true),
                    OverallCondition = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    GeneralNotes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    ActionItemsRequired = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    DocumentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inspections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inspections_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Inspections_Leases_LeaseId",
                        column: x => x.LeaseId,
                        principalTable: "Leases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Inspections_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", maxLength: 100, nullable: false),
                    LeaseId = table.Column<Guid>(type: "TEXT", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    InvoicedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DueOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    PaidOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    LateFeeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LateFeeApplied = table.Column<bool>(type: "INTEGER", nullable: true),
                    LateFeeAppliedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ReminderSent = table.Column<bool>(type: "INTEGER", nullable: true),
                    ReminderSentOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DocumentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoices_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Invoices_Leases_LeaseId",
                        column: x => x.LeaseId,
                        principalTable: "Leases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invoices_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PropertyId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CalendarEventId = table.Column<Guid>(type: "TEXT", nullable: true),
                    LeaseId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    RequestType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Priority = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    RequestedBy = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    RequestedByEmail = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    RequestedByPhone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    RequestedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ScheduledOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CompletedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EstimatedCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ActualCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AssignedTo = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ResolutionNotes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenanceRequests_Leases_LeaseId",
                        column: x => x.LeaseId,
                        principalTable: "Leases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MaintenanceRequests_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SecurityDeposits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", maxLength: 100, nullable: false),
                    LeaseId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TenantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DateReceived = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PaymentMethod = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    TransactionReference = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    InInvestmentPool = table.Column<bool>(type: "INTEGER", nullable: false),
                    PoolEntryDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PoolExitDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RefundProcessedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RefundAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    DeductionsAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    DeductionsReason = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    RefundMethod = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    RefundReference = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityDeposits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SecurityDeposits_Leases_LeaseId",
                        column: x => x.LeaseId,
                        principalTable: "Leases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SecurityDeposits_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", maxLength: 100, nullable: false),
                    InvoiceId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PaidOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentMethod = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    DocumentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Payments_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SecurityDepositDividends",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", maxLength: 100, nullable: false),
                    SecurityDepositId = table.Column<Guid>(type: "TEXT", nullable: false),
                    InvestmentPoolId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LeaseId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TenantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    BaseDividendAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ProrationFactor = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    DividendAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentMethod = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ChoiceMadeOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PaymentProcessedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PaymentReference = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    MailingAddress = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    MonthsInPool = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityDepositDividends", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SecurityDepositDividends_Leases_LeaseId",
                        column: x => x.LeaseId,
                        principalTable: "Leases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SecurityDepositDividends_SecurityDepositInvestmentPools_InvestmentPoolId",
                        column: x => x.InvestmentPoolId,
                        principalTable: "SecurityDepositInvestmentPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SecurityDepositDividends_SecurityDeposits_SecurityDepositId",
                        column: x => x.SecurityDepositId,
                        principalTable: "SecurityDeposits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SecurityDepositDividends_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ChecklistTemplates",
                columns: new[] { "Id", "Category", "CreatedBy", "CreatedOn", "Description", "IsDeleted", "IsSystemTemplate", "LastModifiedBy", "LastModifiedOn", "Name", "OrganizationId" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0001-000000000001"), "Tour", "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Standard property showing checklist", false, true, null, null, "Property Tour", new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("00000000-0000-0000-0001-000000000002"), "MoveIn", "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Move-in inspection checklist", false, true, null, null, "Move-In", new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("00000000-0000-0000-0001-000000000003"), "MoveOut", "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Move-out inspection checklist", false, true, null, null, "Move-Out", new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("00000000-0000-0000-0001-000000000004"), "Tour", "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Open house event checklist", false, true, null, null, "Open House", new Guid("00000000-0000-0000-0000-000000000000") }
                });

            migrationBuilder.InsertData(
                table: "ChecklistTemplateItems",
                columns: new[] { "Id", "AllowsNotes", "CategorySection", "ChecklistTemplateId", "CreatedBy", "CreatedOn", "IsDeleted", "IsRequired", "ItemOrder", "ItemText", "LastModifiedBy", "LastModifiedOn", "OrganizationId", "RequiresValue", "SectionOrder" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0002-000000000001"), true, "Arrival & Introduction", new Guid("00000000-0000-0000-0001-000000000001"), "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 1, "Greeted prospect and verified appointment", null, null, new Guid("00000000-0000-0000-0000-000000000000"), false, 1 },
                    { new Guid("00000000-0000-0000-0002-000000000002"), true, "Arrival & Introduction", new Guid("00000000-0000-0000-0001-000000000001"), "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 2, "Reviewed property exterior and curb appeal", null, null, new Guid("00000000-0000-0000-0000-000000000000"), false, 1 },
                    { new Guid("00000000-0000-0000-0002-000000000003"), true, "Arrival & Introduction", new Guid("00000000-0000-0000-0001-000000000001"), "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 3, "Showed parking area/garage", null, null, new Guid("00000000-0000-0000-0000-000000000000"), false, 1 },
                    { new Guid("00000000-0000-0000-0002-000000000004"), true, "Interior Tour", new Guid("00000000-0000-0000-0001-000000000001"), "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 4, "Toured living room/common areas", null, null, new Guid("00000000-0000-0000-0000-000000000000"), false, 2 },
                    { new Guid("00000000-0000-0000-0002-000000000005"), true, "Interior Tour", new Guid("00000000-0000-0000-0001-000000000001"), "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 5, "Showed all bedrooms", null, null, new Guid("00000000-0000-0000-0000-000000000000"), false, 2 },
                    { new Guid("00000000-0000-0000-0002-000000000006"), true, "Interior Tour", new Guid("00000000-0000-0000-0001-000000000001"), "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 6, "Showed all bathrooms", null, null, new Guid("00000000-0000-0000-0000-000000000000"), false, 2 },
                    { new Guid("00000000-0000-0000-0002-000000000007"), true, "Kitchen & Appliances", new Guid("00000000-0000-0000-0001-000000000001"), "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 7, "Toured kitchen and demonstrated appliances", null, null, new Guid("00000000-0000-0000-0000-000000000000"), false, 3 },
                    { new Guid("00000000-0000-0000-0002-000000000008"), true, "Kitchen & Appliances", new Guid("00000000-0000-0000-0001-000000000001"), "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 8, "Explained which appliances are included", null, null, new Guid("00000000-0000-0000-0000-000000000000"), false, 3 },
                    { new Guid("00000000-0000-0000-0002-000000000009"), true, "Utilities & Systems", new Guid("00000000-0000-0000-0001-000000000001"), "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 9, "Explained HVAC system and thermostat controls", null, null, new Guid("00000000-0000-0000-0000-000000000000"), false, 4 },
                    { new Guid("00000000-0000-0000-0002-000000000010"), true, "Utilities & Systems", new Guid("00000000-0000-0000-0001-000000000001"), "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 10, "Reviewed utility responsibilities (tenant vs landlord)", null, null, new Guid("00000000-0000-0000-0000-000000000000"), false, 4 },
                    { new Guid("00000000-0000-0000-0002-000000000011"), true, "Utilities & Systems", new Guid("00000000-0000-0000-0001-000000000001"), "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 11, "Showed water heater location", null, null, new Guid("00000000-0000-0000-0000-000000000000"), false, 4 },
                    { new Guid("00000000-0000-0000-0002-000000000012"), true, "Storage & Amenities", new Guid("00000000-0000-0000-0001-000000000001"), "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 12, "Showed storage areas (closets, attic, basement)", null, null, new Guid("00000000-0000-0000-0000-000000000000"), false, 5 },
                    { new Guid("00000000-0000-0000-0002-000000000013"), true, "Storage & Amenities", new Guid("00000000-0000-0000-0001-000000000001"), "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 13, "Showed laundry facilities", null, null, new Guid("00000000-0000-0000-0000-000000000000"), false, 5 },
                    { new Guid("00000000-0000-0000-0002-000000000014"), true, "Storage & Amenities", new Guid("00000000-0000-0000-0001-000000000001"), "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 14, "Showed outdoor space (yard, patio, balcony)", null, null, new Guid("00000000-0000-0000-0000-000000000000"), false, 5 },
                    { new Guid("00000000-0000-0000-0002-000000000015"), true, "Lease Terms", new Guid("00000000-0000-0000-0001-000000000001"), "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 15, "Discussed monthly rent amount", null, null, new Guid("00000000-0000-0000-0000-000000000000"), false, 6 },
                    { new Guid("00000000-0000-0000-0002-000000000016"), true, "Lease Terms", new Guid("00000000-0000-0000-0001-000000000001"), "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 16, "Explained security deposit and move-in costs", null, null, new Guid("00000000-0000-0000-0000-000000000000"), false, 6 },
                    { new Guid("00000000-0000-0000-0002-000000000017"), true, "Lease Terms", new Guid("00000000-0000-0000-0001-000000000001"), "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 17, "Reviewed lease term length and start date", null, null, new Guid("00000000-0000-0000-0000-000000000000"), false, 6 },
                    { new Guid("00000000-0000-0000-0002-000000000018"), true, "Lease Terms", new Guid("00000000-0000-0000-0001-000000000001"), "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 18, "Explained pet policy", null, null, new Guid("00000000-0000-0000-0000-000000000000"), false, 6 },
                    { new Guid("00000000-0000-0000-0002-000000000019"), true, "Next Steps", new Guid("00000000-0000-0000-0001-000000000001"), "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 19, "Explained application process and requirements", null, null, new Guid("00000000-0000-0000-0000-000000000000"), false, 7 },
                    { new Guid("00000000-0000-0000-0002-000000000020"), true, "Next Steps", new Guid("00000000-0000-0000-0001-000000000001"), "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 20, "Reviewed screening process (background, credit check)", null, null, new Guid("00000000-0000-0000-0000-000000000000"), false, 7 },
                    { new Guid("00000000-0000-0000-0002-000000000021"), true, "Next Steps", new Guid("00000000-0000-0000-0001-000000000001"), "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 21, "Answered all prospect questions", null, null, new Guid("00000000-0000-0000-0000-000000000000"), false, 7 },
                    { new Guid("00000000-0000-0000-0002-000000000022"), true, "Assessment", new Guid("00000000-0000-0000-0001-000000000001"), "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 22, "Prospect Interest Level", null, null, new Guid("00000000-0000-0000-0000-000000000000"), true, 8 },
                    { new Guid("00000000-0000-0000-0002-000000000023"), true, "Assessment", new Guid("00000000-0000-0000-0001-000000000001"), "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 23, "Overall showing feedback and notes", null, null, new Guid("00000000-0000-0000-0000-000000000000"), false, 8 },
                    { new Guid("00000000-0000-0000-0002-000000000024"), true, "General", new Guid("00000000-0000-0000-0001-000000000002"), "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 1, "Document property condition", null, null, new Guid("00000000-0000-0000-0000-000000000000"), false, 1 },
                    { new Guid("00000000-0000-0000-0002-000000000025"), true, "General", new Guid("00000000-0000-0000-0001-000000000002"), "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 2, "Collect keys and access codes", null, null, new Guid("00000000-0000-0000-0000-000000000000"), false, 1 },
                    { new Guid("00000000-0000-0000-0002-000000000026"), true, "General", new Guid("00000000-0000-0000-0001-000000000002"), "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 3, "Review lease terms with tenant", null, null, new Guid("00000000-0000-0000-0000-000000000000"), false, 1 },
                    { new Guid("00000000-0000-0000-0002-000000000027"), true, "General", new Guid("00000000-0000-0000-0001-000000000003"), "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 1, "Inspect property condition", null, null, new Guid("00000000-0000-0000-0000-000000000000"), false, 1 },
                    { new Guid("00000000-0000-0000-0002-000000000028"), true, "General", new Guid("00000000-0000-0000-0001-000000000003"), "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 2, "Collect all keys and access devices", null, null, new Guid("00000000-0000-0000-0000-000000000000"), false, 1 },
                    { new Guid("00000000-0000-0000-0002-000000000029"), true, "General", new Guid("00000000-0000-0000-0001-000000000003"), "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 3, "Document damages and needed repairs", null, null, new Guid("00000000-0000-0000-0000-000000000000"), false, 1 },
                    { new Guid("00000000-0000-0000-0002-000000000030"), true, "Preparation", new Guid("00000000-0000-0000-0001-000000000004"), "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 1, "Set up signage and directional markers", null, null, new Guid("00000000-0000-0000-0000-000000000000"), false, 1 },
                    { new Guid("00000000-0000-0000-0002-000000000031"), true, "Preparation", new Guid("00000000-0000-0000-0001-000000000004"), "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 2, "Prepare information packets", null, null, new Guid("00000000-0000-0000-0000-000000000000"), false, 1 },
                    { new Guid("00000000-0000-0000-0002-000000000032"), true, "Preparation", new Guid("00000000-0000-0000-0001-000000000004"), "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 3, "Set up visitor sign-in sheet", null, null, new Guid("00000000-0000-0000-0000-000000000000"), false, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationScreenings_OrganizationId",
                table: "ApplicationScreenings",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationScreenings_OverallResult",
                table: "ApplicationScreenings",
                column: "OverallResult");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationScreenings_RentalApplicationId",
                table: "ApplicationScreenings",
                column: "RentalApplicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_EventType",
                table: "CalendarEvents",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_OrganizationId",
                table: "CalendarEvents",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_PropertyId",
                table: "CalendarEvents",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_SourceEntityId",
                table: "CalendarEvents",
                column: "SourceEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_SourceEntityType_SourceEntityId",
                table: "CalendarEvents",
                columns: new[] { "SourceEntityType", "SourceEntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_StartOn",
                table: "CalendarEvents",
                column: "StartOn");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarSettings_OrganizationId",
                table: "CalendarSettings",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarSettings_OrganizationId_EntityType",
                table: "CalendarSettings",
                columns: new[] { "OrganizationId", "EntityType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistItems_ChecklistId",
                table: "ChecklistItems",
                column: "ChecklistId");

            migrationBuilder.CreateIndex(
                name: "IX_Checklists_ChecklistTemplateId",
                table: "Checklists",
                column: "ChecklistTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Checklists_ChecklistType",
                table: "Checklists",
                column: "ChecklistType");

            migrationBuilder.CreateIndex(
                name: "IX_Checklists_CompletedOn",
                table: "Checklists",
                column: "CompletedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Checklists_DocumentId",
                table: "Checklists",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Checklists_LeaseId",
                table: "Checklists",
                column: "LeaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Checklists_PropertyId",
                table: "Checklists",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_Checklists_Status",
                table: "Checklists",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistTemplateItems_ChecklistTemplateId",
                table: "ChecklistTemplateItems",
                column: "ChecklistTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistTemplates_Category",
                table: "ChecklistTemplates",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistTemplates_OrganizationId",
                table: "ChecklistTemplates",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_InvoiceId",
                table: "Documents",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_LeaseId",
                table: "Documents",
                column: "LeaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_OrganizationId",
                table: "Documents",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_PaymentId",
                table: "Documents",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_PropertyId",
                table: "Documents",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_TenantId",
                table: "Documents",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_CompletedOn",
                table: "Inspections",
                column: "CompletedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_DocumentId",
                table: "Inspections",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_LeaseId",
                table: "Inspections",
                column: "LeaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_PropertyId",
                table: "Inspections",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_DocumentId",
                table: "Invoices",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceNumber",
                table: "Invoices",
                column: "InvoiceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_LeaseId",
                table: "Invoices",
                column: "LeaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_OrganizationId",
                table: "Invoices",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaseOffers_PropertyId",
                table: "LeaseOffers",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaseOffers_ProspectiveTenantId",
                table: "LeaseOffers",
                column: "ProspectiveTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaseOffers_RentalApplicationId",
                table: "LeaseOffers",
                column: "RentalApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Leases_DocumentId",
                table: "Leases",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Leases_OrganizationId",
                table: "Leases",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Leases_PropertyId",
                table: "Leases",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_Leases_TenantId",
                table: "Leases",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRequests_LeaseId",
                table: "MaintenanceRequests",
                column: "LeaseId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRequests_Priority",
                table: "MaintenanceRequests",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRequests_PropertyId",
                table: "MaintenanceRequests",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRequests_RequestedOn",
                table: "MaintenanceRequests",
                column: "RequestedOn");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRequests_Status",
                table: "MaintenanceRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationPreferences_OrganizationId",
                table: "NotificationPreferences",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationPreferences_UserId",
                table: "NotificationPreferences",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationPreferences_UserId_OrganizationId",
                table: "NotificationPreferences",
                columns: new[] { "UserId", "OrganizationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Category",
                table: "Notifications",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_IsRead",
                table: "Notifications",
                column: "IsRead");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_OrganizationId",
                table: "Notifications",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RecipientUserId",
                table: "Notifications",
                column: "RecipientUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_SentOn",
                table: "Notifications",
                column: "SentOn");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationEmailSettings_OrganizationId",
                table: "OrganizationEmailSettings",
                column: "OrganizationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_IsActive",
                table: "Organizations",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_OwnerId",
                table: "Organizations",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationSettings_OrganizationId",
                table: "OrganizationSettings",
                column: "OrganizationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationSMSSettings_OrganizationId",
                table: "OrganizationSMSSettings",
                column: "OrganizationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_DocumentId",
                table: "Payments",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_InvoiceId",
                table: "Payments",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrganizationId",
                table: "Payments",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_Address",
                table: "Properties",
                column: "Address");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_OrganizationId",
                table: "Properties",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProspectiveTenants_Email",
                table: "ProspectiveTenants",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_ProspectiveTenants_InterestedPropertyId",
                table: "ProspectiveTenants",
                column: "InterestedPropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProspectiveTenants_OrganizationId",
                table: "ProspectiveTenants",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProspectiveTenants_Status",
                table: "ProspectiveTenants",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_RentalApplications_AppliedOn",
                table: "RentalApplications",
                column: "AppliedOn");

            migrationBuilder.CreateIndex(
                name: "IX_RentalApplications_OrganizationId",
                table: "RentalApplications",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalApplications_PropertyId",
                table: "RentalApplications",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalApplications_ProspectiveTenantId",
                table: "RentalApplications",
                column: "ProspectiveTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalApplications_Status",
                table: "RentalApplications",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityDepositDividends_InvestmentPoolId",
                table: "SecurityDepositDividends",
                column: "InvestmentPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityDepositDividends_LeaseId",
                table: "SecurityDepositDividends",
                column: "LeaseId");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityDepositDividends_SecurityDepositId",
                table: "SecurityDepositDividends",
                column: "SecurityDepositId");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityDepositDividends_Status",
                table: "SecurityDepositDividends",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityDepositDividends_TenantId",
                table: "SecurityDepositDividends",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityDepositDividends_Year",
                table: "SecurityDepositDividends",
                column: "Year");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityDepositInvestmentPools_OrganizationId",
                table: "SecurityDepositInvestmentPools",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityDepositInvestmentPools_Status",
                table: "SecurityDepositInvestmentPools",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityDepositInvestmentPools_Year",
                table: "SecurityDepositInvestmentPools",
                column: "Year",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SecurityDeposits_InInvestmentPool",
                table: "SecurityDeposits",
                column: "InInvestmentPool");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityDeposits_LeaseId",
                table: "SecurityDeposits",
                column: "LeaseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SecurityDeposits_Status",
                table: "SecurityDeposits",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityDeposits_TenantId",
                table: "SecurityDeposits",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Email",
                table: "Tenants",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_IdentificationNumber",
                table: "Tenants",
                column: "IdentificationNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_OrganizationId",
                table: "Tenants",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Tours_ChecklistId",
                table: "Tours",
                column: "ChecklistId");

            migrationBuilder.CreateIndex(
                name: "IX_Tours_OrganizationId",
                table: "Tours",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Tours_PropertyId",
                table: "Tours",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_Tours_ProspectiveTenantId",
                table: "Tours",
                column: "ProspectiveTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Tours_ScheduledOn",
                table: "Tours",
                column: "ScheduledOn");

            migrationBuilder.CreateIndex(
                name: "IX_Tours_Status",
                table: "Tours",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_UserOrganizations_IsActive",
                table: "UserOrganizations",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_UserOrganizations_OrganizationId",
                table: "UserOrganizations",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOrganizations_Role",
                table: "UserOrganizations",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_UserOrganizations_UserId_OrganizationId",
                table: "UserOrganizations",
                columns: new[] { "UserId", "OrganizationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowAuditLogs_Action",
                table: "WorkflowAuditLogs",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowAuditLogs_EntityId",
                table: "WorkflowAuditLogs",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowAuditLogs_EntityType",
                table: "WorkflowAuditLogs",
                column: "EntityType");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowAuditLogs_EntityType_EntityId",
                table: "WorkflowAuditLogs",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowAuditLogs_OrganizationId",
                table: "WorkflowAuditLogs",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowAuditLogs_PerformedBy",
                table: "WorkflowAuditLogs",
                column: "PerformedBy");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowAuditLogs_PerformedOn",
                table: "WorkflowAuditLogs",
                column: "PerformedOn");

            migrationBuilder.AddForeignKey(
                name: "FK_ChecklistItems_Checklists_ChecklistId",
                table: "ChecklistItems",
                column: "ChecklistId",
                principalTable: "Checklists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Checklists_Documents_DocumentId",
                table: "Checklists",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Checklists_Leases_LeaseId",
                table: "Checklists",
                column: "LeaseId",
                principalTable: "Leases",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Invoices_InvoiceId",
                table: "Documents",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Leases_LeaseId",
                table: "Documents",
                column: "LeaseId",
                principalTable: "Leases",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Payments_PaymentId",
                table: "Documents",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Properties_PropertyId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Leases_Properties_PropertyId",
                table: "Leases");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Documents_DocumentId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Leases_Documents_DocumentId",
                table: "Leases");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Documents_DocumentId",
                table: "Payments");

            migrationBuilder.DropTable(
                name: "ApplicationScreenings");

            migrationBuilder.DropTable(
                name: "CalendarEvents");

            migrationBuilder.DropTable(
                name: "CalendarSettings");

            migrationBuilder.DropTable(
                name: "ChecklistItems");

            migrationBuilder.DropTable(
                name: "ChecklistTemplateItems");

            migrationBuilder.DropTable(
                name: "Inspections");

            migrationBuilder.DropTable(
                name: "LeaseOffers");

            migrationBuilder.DropTable(
                name: "MaintenanceRequests");

            migrationBuilder.DropTable(
                name: "Notes");

            migrationBuilder.DropTable(
                name: "NotificationPreferences");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "OrganizationEmailSettings");

            migrationBuilder.DropTable(
                name: "OrganizationSettings");

            migrationBuilder.DropTable(
                name: "OrganizationSMSSettings");

            migrationBuilder.DropTable(
                name: "SchemaVersions");

            migrationBuilder.DropTable(
                name: "SecurityDepositDividends");

            migrationBuilder.DropTable(
                name: "Tours");

            migrationBuilder.DropTable(
                name: "UserOrganizations");

            migrationBuilder.DropTable(
                name: "WorkflowAuditLogs");

            migrationBuilder.DropTable(
                name: "RentalApplications");

            migrationBuilder.DropTable(
                name: "SecurityDepositInvestmentPools");

            migrationBuilder.DropTable(
                name: "SecurityDeposits");

            migrationBuilder.DropTable(
                name: "Checklists");

            migrationBuilder.DropTable(
                name: "ProspectiveTenants");

            migrationBuilder.DropTable(
                name: "ChecklistTemplates");

            migrationBuilder.DropTable(
                name: "Properties");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "Leases");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropTable(
                name: "Organizations");
        }
    }
}
