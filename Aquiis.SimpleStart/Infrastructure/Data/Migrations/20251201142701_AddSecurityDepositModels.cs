using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aquiis.SimpleStart.Migrations
{
    /// <inheritdoc />
    public partial class AddSecurityDepositModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SecurityDepositInvestmentPools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
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
                name: "SecurityDeposits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrganizationId = table.Column<Guid>(type: "TEXT", nullable: false),
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
                name: "SecurityDepositDividends",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrganizationId = table.Column<Guid>(type: "TEXT", nullable: false),
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SecurityDepositDividends");

            migrationBuilder.DropTable(
                name: "SecurityDepositInvestmentPools");

            migrationBuilder.DropTable(
                name: "SecurityDeposits");
        }
    }
}
