using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aquiis.SimpleStart.Migrations
{
    /// <inheritdoc />
    public partial class AddProspectToTenantConversion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProspectiveTenantId",
                table: "Tenants",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeclinedOn",
                table: "Leases",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresOn",
                table: "Leases",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OfferedOn",
                table: "Leases",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SignedOn",
                table: "Leases",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProspectiveTenantId",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "DeclinedOn",
                table: "Leases");

            migrationBuilder.DropColumn(
                name: "ExpiresOn",
                table: "Leases");

            migrationBuilder.DropColumn(
                name: "OfferedOn",
                table: "Leases");

            migrationBuilder.DropColumn(
                name: "SignedOn",
                table: "Leases");
        }
    }
}
