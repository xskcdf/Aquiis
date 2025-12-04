using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Aquiis.SimpleStart.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    ActiveOrganizationId = table.Column<string>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<string>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    LastLoginDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PreviousLoginDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LoginCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LastLoginIP = table.Column<string>(type: "TEXT", nullable: true),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CalendarSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrganizationId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
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
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrganizationId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
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
                name: "OrganizationSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
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
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrganizationId = table.Column<string>(type: "TEXT", nullable: false),
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
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderKey = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrganizationId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Content = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: false),
                    EntityType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EntityId = table.Column<int>(type: "INTEGER", nullable: false),
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
                    table.ForeignKey(
                        name: "FK_Notes_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
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
                    table.ForeignKey(
                        name: "FK_Organizations_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistTemplateItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrganizationId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ChecklistTemplateId = table.Column<int>(type: "INTEGER", nullable: false),
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
                name: "Properties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrganizationId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
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
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrganizationId = table.Column<string>(type: "TEXT", nullable: false),
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
                    ProspectiveTenantId = table.Column<int>(type: "INTEGER", nullable: true),
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
                    Id = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<string>(type: "TEXT", nullable: false),
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
                        name: "FK_UserOrganizations_AspNetUsers_GrantedBy",
                        column: x => x.GrantedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserOrganizations_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrganizationId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    StartOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DurationMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    EventType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    PropertyId = table.Column<int>(type: "INTEGER", nullable: true),
                    Location = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Color = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    SourceEntityId = table.Column<int>(type: "INTEGER", nullable: true),
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
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrganizationId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
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
                    InterestedPropertyId = table.Column<int>(type: "INTEGER", nullable: true),
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
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrganizationId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ProspectiveTenantId = table.Column<int>(type: "INTEGER", nullable: false),
                    PropertyId = table.Column<int>(type: "INTEGER", nullable: false),
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
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrganizationId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    RentalApplicationId = table.Column<int>(type: "INTEGER", nullable: false),
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
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrganizationId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    RentalApplicationId = table.Column<int>(type: "INTEGER", nullable: false),
                    PropertyId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProspectiveTenantId = table.Column<int>(type: "INTEGER", nullable: false),
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
                    ConvertedLeaseId = table.Column<int>(type: "INTEGER", nullable: true),
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
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrganizationId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ChecklistId = table.Column<int>(type: "INTEGER", nullable: false),
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
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrganizationId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PropertyId = table.Column<int>(type: "INTEGER", nullable: true),
                    LeaseId = table.Column<int>(type: "INTEGER", nullable: true),
                    ChecklistTemplateId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ChecklistType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CompletedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CompletedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DocumentId = table.Column<int>(type: "INTEGER", nullable: true),
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
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrganizationId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ProspectiveTenantId = table.Column<int>(type: "INTEGER", nullable: false),
                    PropertyId = table.Column<int>(type: "INTEGER", nullable: false),
                    ScheduledOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DurationMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Feedback = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    InterestLevel = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    ConductedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ChecklistId = table.Column<int>(type: "INTEGER", nullable: true),
                    CalendarEventId = table.Column<int>(type: "INTEGER", nullable: true),
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
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrganizationId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    FileName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    FileExtension = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    FileData = table.Column<byte[]>(type: "BLOB", nullable: false),
                    FilePath = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    FileType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    FileSize = table.Column<long>(type: "INTEGER", nullable: false),
                    DocumentType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    PropertyId = table.Column<int>(type: "INTEGER", nullable: true),
                    TenantId = table.Column<int>(type: "INTEGER", nullable: true),
                    LeaseId = table.Column<int>(type: "INTEGER", nullable: true),
                    InvoiceId = table.Column<int>(type: "INTEGER", nullable: true),
                    PaymentId = table.Column<int>(type: "INTEGER", nullable: true),
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
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrganizationId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PropertyId = table.Column<int>(type: "INTEGER", nullable: false),
                    TenantId = table.Column<int>(type: "INTEGER", nullable: false),
                    LeaseOfferId = table.Column<int>(type: "INTEGER", nullable: true),
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
                    PreviousLeaseId = table.Column<int>(type: "INTEGER", nullable: true),
                    DocumentId = table.Column<int>(type: "INTEGER", nullable: true),
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
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrganizationId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PropertyId = table.Column<int>(type: "INTEGER", nullable: false),
                    CalendarEventId = table.Column<int>(type: "INTEGER", nullable: true),
                    LeaseId = table.Column<int>(type: "INTEGER", nullable: true),
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
                    DocumentId = table.Column<int>(type: "INTEGER", nullable: true),
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
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrganizationId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LeaseId = table.Column<int>(type: "INTEGER", nullable: false),
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
                    DocumentId = table.Column<int>(type: "INTEGER", nullable: true),
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
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrganizationId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PropertyId = table.Column<int>(type: "INTEGER", nullable: false),
                    CalendarEventId = table.Column<int>(type: "INTEGER", nullable: true),
                    LeaseId = table.Column<int>(type: "INTEGER", nullable: true),
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
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrganizationId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LeaseId = table.Column<int>(type: "INTEGER", nullable: false),
                    TenantId = table.Column<int>(type: "INTEGER", nullable: false),
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
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrganizationId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    InvoiceId = table.Column<int>(type: "INTEGER", nullable: false),
                    PaidOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentMethod = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    DocumentId = table.Column<int>(type: "INTEGER", nullable: true),
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
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrganizationId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SecurityDepositId = table.Column<int>(type: "INTEGER", nullable: false),
                    InvestmentPoolId = table.Column<int>(type: "INTEGER", nullable: false),
                    LeaseId = table.Column<int>(type: "INTEGER", nullable: false),
                    TenantId = table.Column<int>(type: "INTEGER", nullable: false),
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
                    { 1, "Tour", "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Standard property showing checklist", false, true, "", null, "Property Tour", "" },
                    { 2, "MoveIn", "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Move-in inspection checklist", false, true, "", null, "Move-In", "" },
                    { 3, "MoveOut", "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Move-out inspection checklist", false, true, "", null, "Move-Out", "" },
                    { 4, "Tour", "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Open house event checklist", false, true, "", null, "Open House", "" }
                });

            migrationBuilder.InsertData(
                table: "ChecklistTemplateItems",
                columns: new[] { "Id", "AllowsNotes", "CategorySection", "ChecklistTemplateId", "CreatedBy", "CreatedOn", "IsDeleted", "IsRequired", "ItemOrder", "ItemText", "LastModifiedBy", "LastModifiedOn", "OrganizationId", "RequiresValue", "SectionOrder" },
                values: new object[,]
                {
                    { 1, true, "Arrival & Introduction", 1, "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 1, "Greeted prospect and verified appointment", "", null, "", false, 1 },
                    { 2, true, "Arrival & Introduction", 1, "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 2, "Reviewed property exterior and curb appeal", "", null, "", false, 1 },
                    { 3, true, "Arrival & Introduction", 1, "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 3, "Showed parking area/garage", "", null, "", false, 1 },
                    { 4, true, "Interior Tour", 1, "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 4, "Toured living room/common areas", "", null, "", false, 2 },
                    { 5, true, "Interior Tour", 1, "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 5, "Showed all bedrooms", "", null, "", false, 2 },
                    { 6, true, "Interior Tour", 1, "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 6, "Showed all bathrooms", "", null, "", false, 2 },
                    { 7, true, "Kitchen & Appliances", 1, "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 7, "Toured kitchen and demonstrated appliances", "", null, "", false, 3 },
                    { 8, true, "Kitchen & Appliances", 1, "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 8, "Explained which appliances are included", "", null, "", false, 3 },
                    { 9, true, "Utilities & Systems", 1, "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 9, "Explained HVAC system and thermostat controls", "", null, "", false, 4 },
                    { 10, true, "Utilities & Systems", 1, "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 10, "Reviewed utility responsibilities (tenant vs landlord)", "", null, "", false, 4 },
                    { 11, true, "Utilities & Systems", 1, "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 11, "Showed water heater location", "", null, "", false, 4 },
                    { 12, true, "Storage & Amenities", 1, "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 12, "Showed storage areas (closets, attic, basement)", "", null, "", false, 5 },
                    { 13, true, "Storage & Amenities", 1, "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 13, "Showed laundry facilities", "", null, "", false, 5 },
                    { 14, true, "Storage & Amenities", 1, "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 14, "Showed outdoor space (yard, patio, balcony)", "", null, "", false, 5 },
                    { 15, true, "Lease Terms", 1, "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 15, "Discussed monthly rent amount", "", null, "", true, 6 },
                    { 16, true, "Lease Terms", 1, "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 16, "Explained security deposit and move-in costs", "", null, "", true, 6 },
                    { 17, true, "Lease Terms", 1, "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 17, "Reviewed lease term length and start date", "", null, "", false, 6 },
                    { 18, true, "Lease Terms", 1, "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 18, "Explained pet policy", "", null, "", false, 6 },
                    { 19, true, "Next Steps", 1, "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 19, "Explained application process and requirements", "", null, "", false, 7 },
                    { 20, true, "Next Steps", 1, "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 20, "Reviewed screening process (background, credit check)", "", null, "", false, 7 },
                    { 21, true, "Next Steps", 1, "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 21, "Answered all prospect questions", "", null, "", false, 7 },
                    { 22, true, "Assessment", 1, "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 22, "Prospect Interest Level", "", null, "", true, 8 },
                    { 23, true, "Assessment", 1, "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 23, "Overall showing feedback and notes", "", null, "", true, 8 },
                    { 24, true, "General", 2, "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 1, "Document property condition", "", null, "", false, 1 },
                    { 25, true, "General", 2, "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 2, "Collect keys and access codes", "", null, "", false, 1 },
                    { 26, true, "General", 2, "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 3, "Review lease terms with tenant", "", null, "", false, 1 },
                    { 27, true, "General", 3, "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 1, "Inspect property condition", "", null, "", false, 1 },
                    { 28, true, "General", 3, "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 2, "Collect all keys and access devices", "", null, "", false, 1 },
                    { 29, true, "General", 3, "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 3, "Document damages and needed repairs", "", null, "", false, 1 },
                    { 30, true, "Preparation", 4, "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 1, "Set up signage and directional markers", "", null, "", false, 1 },
                    { 31, true, "Preparation", 4, "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 2, "Prepare information packets", "", null, "", false, 1 },
                    { 32, true, "Preparation", 4, "", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 3, "Set up visitor sign-in sheet", "", null, "", false, 1 }
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
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
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
                name: "IX_Notes_CreatedBy",
                table: "Notes",
                column: "CreatedBy");

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
                column: "ProspectiveTenantId",
                unique: true);

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
                name: "IX_UserOrganizations_GrantedBy",
                table: "UserOrganizations",
                column: "GrantedBy");

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
                name: "FK_Organizations_AspNetUsers_OwnerId",
                table: "Organizations");

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
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

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
                name: "OrganizationSettings");

            migrationBuilder.DropTable(
                name: "SchemaVersions");

            migrationBuilder.DropTable(
                name: "SecurityDepositDividends");

            migrationBuilder.DropTable(
                name: "Tours");

            migrationBuilder.DropTable(
                name: "UserOrganizations");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

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
                name: "AspNetUsers");

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
