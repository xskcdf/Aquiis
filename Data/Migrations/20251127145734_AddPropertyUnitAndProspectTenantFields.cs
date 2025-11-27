using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aquiis.SimpleStart.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyUnitAndProspectTenantFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "EmergencyContactPhone",
                table: "Tenants",
                type: "TEXT",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "ProspectiveTenants",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentificationNumber",
                table: "ProspectiveTenants",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentificationState",
                table: "ProspectiveTenants",
                type: "TEXT",
                maxLength: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnitNumber",
                table: "Properties",
                type: "TEXT",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "ProspectiveTenants");

            migrationBuilder.DropColumn(
                name: "IdentificationNumber",
                table: "ProspectiveTenants");

            migrationBuilder.DropColumn(
                name: "IdentificationState",
                table: "ProspectiveTenants");

            migrationBuilder.DropColumn(
                name: "UnitNumber",
                table: "Properties");

            migrationBuilder.AlterColumn<string>(
                name: "EmergencyContactPhone",
                table: "Tenants",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 20,
                oldNullable: true);
        }
    }
}
