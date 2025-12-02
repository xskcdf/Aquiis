using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aquiis.SimpleStart.Migrations
{
    /// <inheritdoc />
    public partial class AddSecurityDepositSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowTenantDividendChoice",
                table: "OrganizationSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AutoCalculateSecurityDeposit",
                table: "OrganizationSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DefaultDividendPaymentMethod",
                table: "OrganizationSettings",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DividendDistributionMonth",
                table: "OrganizationSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "OrganizationSharePercentage",
                table: "OrganizationSettings",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "RefundProcessingDays",
                table: "OrganizationSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SecurityDepositInvestmentEnabled",
                table: "OrganizationSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "SecurityDepositMultiplier",
                table: "OrganizationSettings",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowTenantDividendChoice",
                table: "OrganizationSettings");

            migrationBuilder.DropColumn(
                name: "AutoCalculateSecurityDeposit",
                table: "OrganizationSettings");

            migrationBuilder.DropColumn(
                name: "DefaultDividendPaymentMethod",
                table: "OrganizationSettings");

            migrationBuilder.DropColumn(
                name: "DividendDistributionMonth",
                table: "OrganizationSettings");

            migrationBuilder.DropColumn(
                name: "OrganizationSharePercentage",
                table: "OrganizationSettings");

            migrationBuilder.DropColumn(
                name: "RefundProcessingDays",
                table: "OrganizationSettings");

            migrationBuilder.DropColumn(
                name: "SecurityDepositInvestmentEnabled",
                table: "OrganizationSettings");

            migrationBuilder.DropColumn(
                name: "SecurityDepositMultiplier",
                table: "OrganizationSettings");
        }
    }
}
