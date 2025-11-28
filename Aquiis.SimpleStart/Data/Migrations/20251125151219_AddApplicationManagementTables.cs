using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aquiis.SimpleStart.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationManagementTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProspectiveTenants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Source = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    InterestedPropertyId = table.Column<int>(type: "INTEGER", nullable: true),
                    DesiredMoveInDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FirstContactDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    OrganizationId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
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
                    ProspectiveTenantId = table.Column<int>(type: "INTEGER", nullable: false),
                    PropertyId = table.Column<int>(type: "INTEGER", nullable: false),
                    ApplicationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
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
                    ApplicationFeePaidDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DenialReason = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    DecisionDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DecisionBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    OrganizationId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
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
                name: "Showings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProspectiveTenantId = table.Column<int>(type: "INTEGER", nullable: false),
                    PropertyId = table.Column<int>(type: "INTEGER", nullable: false),
                    ScheduledDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DurationMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Feedback = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    InterestLevel = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    ConductedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    OrganizationId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Showings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Showings_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Showings_ProspectiveTenants_ProspectiveTenantId",
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
                    RentalApplicationId = table.Column<int>(type: "INTEGER", nullable: false),
                    BackgroundCheckRequested = table.Column<bool>(type: "INTEGER", nullable: false),
                    BackgroundCheckRequestedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    BackgroundCheckPassed = table.Column<bool>(type: "INTEGER", nullable: true),
                    BackgroundCheckCompletedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    BackgroundCheckNotes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreditCheckRequested = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreditCheckRequestedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreditScore = table.Column<int>(type: "INTEGER", nullable: true),
                    CreditCheckPassed = table.Column<bool>(type: "INTEGER", nullable: true),
                    CreditCheckCompletedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreditCheckNotes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    OverallResult = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ResultNotes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    OrganizationId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
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
                name: "IX_RentalApplications_ApplicationDate",
                table: "RentalApplications",
                column: "ApplicationDate");

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
                name: "IX_Showings_OrganizationId",
                table: "Showings",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Showings_PropertyId",
                table: "Showings",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_Showings_ProspectiveTenantId",
                table: "Showings",
                column: "ProspectiveTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Showings_ScheduledDateTime",
                table: "Showings",
                column: "ScheduledDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_Showings_Status",
                table: "Showings",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationScreenings");

            migrationBuilder.DropTable(
                name: "Showings");

            migrationBuilder.DropTable(
                name: "RentalApplications");

            migrationBuilder.DropTable(
                name: "ProspectiveTenants");
        }
    }
}
