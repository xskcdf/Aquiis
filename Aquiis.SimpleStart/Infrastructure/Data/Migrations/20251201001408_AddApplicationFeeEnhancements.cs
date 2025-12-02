using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aquiis.SimpleStart.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationFeeEnhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationFeePaymentMethod",
                table: "RentalApplications",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresOn",
                table: "RentalApplications",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApplicationExpirationDays",
                table: "OrganizationSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "ApplicationFeeEnabled",
                table: "OrganizationSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultApplicationFee",
                table: "OrganizationSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationFeePaymentMethod",
                table: "RentalApplications");

            migrationBuilder.DropColumn(
                name: "ExpiresOn",
                table: "RentalApplications");

            migrationBuilder.DropColumn(
                name: "ApplicationExpirationDays",
                table: "OrganizationSettings");

            migrationBuilder.DropColumn(
                name: "ApplicationFeeEnabled",
                table: "OrganizationSettings");

            migrationBuilder.DropColumn(
                name: "DefaultApplicationFee",
                table: "OrganizationSettings");
        }
    }
}
