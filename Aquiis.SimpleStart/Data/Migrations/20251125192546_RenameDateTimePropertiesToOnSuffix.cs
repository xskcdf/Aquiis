using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aquiis.SimpleStart.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameDateTimePropertiesToOnSuffix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DecisionDate",
                table: "RentalApplications",
                newName: "DecidedOn");

            migrationBuilder.RenameColumn(
                name: "ApplicationFeePaidDate",
                table: "RentalApplications",
                newName: "ApplicationFeePaidOn");

            migrationBuilder.RenameColumn(
                name: "ApplicationDate",
                table: "RentalApplications",
                newName: "AppliedOn");

            migrationBuilder.RenameIndex(
                name: "IX_RentalApplications_ApplicationDate",
                table: "RentalApplications",
                newName: "IX_RentalApplications_AppliedOn");

            migrationBuilder.RenameColumn(
                name: "FirstContactDate",
                table: "ProspectiveTenants",
                newName: "FirstContactedOn");

            migrationBuilder.RenameColumn(
                name: "PaymentDate",
                table: "Payments",
                newName: "PaidOn");

            migrationBuilder.RenameColumn(
                name: "ReminderSentDate",
                table: "Invoices",
                newName: "ReminderSentOn");

            migrationBuilder.RenameColumn(
                name: "LateFeeAppliedDate",
                table: "Invoices",
                newName: "LateFeeAppliedOn");

            migrationBuilder.RenameColumn(
                name: "InvoiceDate",
                table: "Invoices",
                newName: "InvoicedOn");

            migrationBuilder.RenameColumn(
                name: "DueDate",
                table: "Invoices",
                newName: "DueOn");

            migrationBuilder.RenameColumn(
                name: "InspectionDate",
                table: "Inspections",
                newName: "CompletedOn");

            migrationBuilder.RenameIndex(
                name: "IX_Inspections_InspectionDate",
                table: "Inspections",
                newName: "IX_Inspections_CompletedOn");

            migrationBuilder.RenameColumn(
                name: "CreditCheckRequestedDate",
                table: "ApplicationScreenings",
                newName: "CreditCheckRequestedOn");

            migrationBuilder.RenameColumn(
                name: "CreditCheckCompletedDate",
                table: "ApplicationScreenings",
                newName: "CreditCheckCompletedOn");

            migrationBuilder.RenameColumn(
                name: "BackgroundCheckRequestedDate",
                table: "ApplicationScreenings",
                newName: "BackgroundCheckRequestedOn");

            migrationBuilder.RenameColumn(
                name: "BackgroundCheckCompletedDate",
                table: "ApplicationScreenings",
                newName: "BackgroundCheckCompletedOn");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DecidedOn",
                table: "RentalApplications",
                newName: "DecisionDate");

            migrationBuilder.RenameColumn(
                name: "AppliedOn",
                table: "RentalApplications",
                newName: "ApplicationDate");

            migrationBuilder.RenameColumn(
                name: "ApplicationFeePaidOn",
                table: "RentalApplications",
                newName: "ApplicationFeePaidDate");

            migrationBuilder.RenameIndex(
                name: "IX_RentalApplications_AppliedOn",
                table: "RentalApplications",
                newName: "IX_RentalApplications_ApplicationDate");

            migrationBuilder.RenameColumn(
                name: "FirstContactedOn",
                table: "ProspectiveTenants",
                newName: "FirstContactDate");

            migrationBuilder.RenameColumn(
                name: "PaidOn",
                table: "Payments",
                newName: "PaymentDate");

            migrationBuilder.RenameColumn(
                name: "ReminderSentOn",
                table: "Invoices",
                newName: "ReminderSentDate");

            migrationBuilder.RenameColumn(
                name: "LateFeeAppliedOn",
                table: "Invoices",
                newName: "LateFeeAppliedDate");

            migrationBuilder.RenameColumn(
                name: "InvoicedOn",
                table: "Invoices",
                newName: "InvoiceDate");

            migrationBuilder.RenameColumn(
                name: "DueOn",
                table: "Invoices",
                newName: "DueDate");

            migrationBuilder.RenameColumn(
                name: "CompletedOn",
                table: "Inspections",
                newName: "InspectionDate");

            migrationBuilder.RenameIndex(
                name: "IX_Inspections_CompletedOn",
                table: "Inspections",
                newName: "IX_Inspections_InspectionDate");

            migrationBuilder.RenameColumn(
                name: "CreditCheckRequestedOn",
                table: "ApplicationScreenings",
                newName: "CreditCheckRequestedDate");

            migrationBuilder.RenameColumn(
                name: "CreditCheckCompletedOn",
                table: "ApplicationScreenings",
                newName: "CreditCheckCompletedDate");

            migrationBuilder.RenameColumn(
                name: "BackgroundCheckRequestedOn",
                table: "ApplicationScreenings",
                newName: "BackgroundCheckRequestedDate");

            migrationBuilder.RenameColumn(
                name: "BackgroundCheckCompletedOn",
                table: "ApplicationScreenings",
                newName: "BackgroundCheckCompletedDate");
        }
    }
}
