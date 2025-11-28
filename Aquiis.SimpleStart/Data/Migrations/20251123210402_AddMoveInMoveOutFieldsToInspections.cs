using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aquiis.SimpleStart.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMoveInMoveOutFieldsToInspections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CleaningCost",
                table: "Inspections",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CleaningRequired",
                table: "Inspections",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DeductionDetails",
                table: "Inspections",
                type: "TEXT",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ElectricMeterReading",
                table: "Inspections",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EstimatedRepairCost",
                table: "Inspections",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ForwardingAddress",
                table: "Inspections",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ForwardingAddressProvided",
                table: "Inspections",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "GasMeterReading",
                table: "Inspections",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "KeyCount",
                table: "Inspections",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KeyTypes",
                table: "Inspections",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "KeysProvided",
                table: "Inspections",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "MeterReadingDate",
                table: "Inspections",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParkingPassNumber",
                table: "Inspections",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ParkingPassProvided",
                table: "Inspections",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RemoteControlTypes",
                table: "Inspections",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RemoteControlsProvided",
                table: "Inspections",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RepairsRequired",
                table: "Inspections",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "SecurityDepositAmount",
                table: "Inspections",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SecurityDepositDeductions",
                table: "Inspections",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SecurityDepositRefundAmount",
                table: "Inspections",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "WaterMeterReading",
                table: "Inspections",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CleaningCost",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "CleaningRequired",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "DeductionDetails",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "ElectricMeterReading",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "EstimatedRepairCost",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "ForwardingAddress",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "ForwardingAddressProvided",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "GasMeterReading",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "KeyCount",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "KeyTypes",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "KeysProvided",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "MeterReadingDate",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "ParkingPassNumber",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "ParkingPassProvided",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "RemoteControlTypes",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "RemoteControlsProvided",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "RepairsRequired",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "SecurityDepositAmount",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "SecurityDepositDeductions",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "SecurityDepositRefundAmount",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "WaterMeterReading",
                table: "Inspections");
        }
    }
}
