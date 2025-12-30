using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aquiis.SimpleStart.Migrations
{
    /// <inheritdoc />
    public partial class OrganizationEmailSMSSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganizationEmailSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", nullable: false),
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

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationEmailSettings_OrganizationId",
                table: "OrganizationEmailSettings",
                column: "OrganizationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationSMSSettings_OrganizationId",
                table: "OrganizationSMSSettings",
                column: "OrganizationId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationEmailSettings");

            migrationBuilder.DropTable(
                name: "OrganizationSMSSettings");
        }
    }
}
