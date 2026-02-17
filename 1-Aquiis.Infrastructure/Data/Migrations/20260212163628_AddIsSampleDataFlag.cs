using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aquiis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsSampleDataFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payments_OrganizationId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_InvoiceNumber",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_OrganizationId",
                table: "Invoices");

            migrationBuilder.AddColumn<bool>(
                name: "IsSampleData",
                table: "WorkflowAuditLogs",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSampleData",
                table: "UserProfiles",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSampleData",
                table: "Tours",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSampleData",
                table: "Tenants",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSampleData",
                table: "SecurityDeposits",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSampleData",
                table: "SecurityDepositInvestmentPools",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSampleData",
                table: "SecurityDepositDividends",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSampleData",
                table: "Repairs",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSampleData",
                table: "RentalApplications",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSampleData",
                table: "ProspectiveTenants",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSampleData",
                table: "Properties",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSampleData",
                table: "Payments",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSampleData",
                table: "OrganizationSMSSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSampleData",
                table: "OrganizationSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSampleData",
                table: "OrganizationEmailSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSampleData",
                table: "Notifications",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSampleData",
                table: "NotificationPreferences",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSampleData",
                table: "Notes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSampleData",
                table: "MaintenanceRequests",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSampleData",
                table: "Leases",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSampleData",
                table: "LeaseOffers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSampleData",
                table: "Invoices",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSampleData",
                table: "Inspections",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSampleData",
                table: "Documents",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSampleData",
                table: "ChecklistTemplates",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSampleData",
                table: "ChecklistTemplateItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSampleData",
                table: "Checklists",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSampleData",
                table: "ChecklistItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSampleData",
                table: "CalendarSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSampleData",
                table: "CalendarEvents",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSampleData",
                table: "ApplicationScreenings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000001"),
                column: "IsSampleData",
                value: false);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000002"),
                column: "IsSampleData",
                value: false);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000003"),
                column: "IsSampleData",
                value: false);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000004"),
                column: "IsSampleData",
                value: false);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000005"),
                column: "IsSampleData",
                value: false);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000006"),
                column: "IsSampleData",
                value: false);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000007"),
                column: "IsSampleData",
                value: false);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000008"),
                column: "IsSampleData",
                value: false);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000009"),
                column: "IsSampleData",
                value: false);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000010"),
                column: "IsSampleData",
                value: false);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000011"),
                column: "IsSampleData",
                value: false);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000012"),
                column: "IsSampleData",
                value: false);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000013"),
                column: "IsSampleData",
                value: false);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000014"),
                column: "IsSampleData",
                value: false);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000015"),
                column: "IsSampleData",
                value: false);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000016"),
                column: "IsSampleData",
                value: false);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000017"),
                column: "IsSampleData",
                value: false);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000018"),
                column: "IsSampleData",
                value: false);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000019"),
                column: "IsSampleData",
                value: false);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000020"),
                column: "IsSampleData",
                value: false);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000021"),
                column: "IsSampleData",
                value: false);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000022"),
                column: "IsSampleData",
                value: false);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000023"),
                column: "IsSampleData",
                value: false);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000024"),
                column: "IsSampleData",
                value: false);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000025"),
                column: "IsSampleData",
                value: false);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000026"),
                column: "IsSampleData",
                value: false);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000027"),
                column: "IsSampleData",
                value: false);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000028"),
                column: "IsSampleData",
                value: false);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000029"),
                column: "IsSampleData",
                value: false);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000030"),
                column: "IsSampleData",
                value: false);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000031"),
                column: "IsSampleData",
                value: false);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0002-000000000032"),
                column: "IsSampleData",
                value: false);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplates",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000001"),
                column: "IsSampleData",
                value: false);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplates",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000002"),
                column: "IsSampleData",
                value: false);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplates",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000003"),
                column: "IsSampleData",
                value: false);

            migrationBuilder.UpdateData(
                table: "ChecklistTemplates",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0001-000000000004"),
                column: "IsSampleData",
                value: false);

            migrationBuilder.CreateIndex(
                name: "IX_Payment_OrgId_PaymentNumber",
                table: "Payments",
                columns: new[] { "OrganizationId", "PaymentNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_OrgId_InvoiceNumber",
                table: "Invoices",
                columns: new[] { "OrganizationId", "InvoiceNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payment_OrgId_PaymentNumber",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Invoice_OrgId_InvoiceNumber",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "IsSampleData",
                table: "WorkflowAuditLogs");

            migrationBuilder.DropColumn(
                name: "IsSampleData",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "IsSampleData",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "IsSampleData",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "IsSampleData",
                table: "SecurityDeposits");

            migrationBuilder.DropColumn(
                name: "IsSampleData",
                table: "SecurityDepositInvestmentPools");

            migrationBuilder.DropColumn(
                name: "IsSampleData",
                table: "SecurityDepositDividends");

            migrationBuilder.DropColumn(
                name: "IsSampleData",
                table: "Repairs");

            migrationBuilder.DropColumn(
                name: "IsSampleData",
                table: "RentalApplications");

            migrationBuilder.DropColumn(
                name: "IsSampleData",
                table: "ProspectiveTenants");

            migrationBuilder.DropColumn(
                name: "IsSampleData",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "IsSampleData",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "IsSampleData",
                table: "OrganizationSMSSettings");

            migrationBuilder.DropColumn(
                name: "IsSampleData",
                table: "OrganizationSettings");

            migrationBuilder.DropColumn(
                name: "IsSampleData",
                table: "OrganizationEmailSettings");

            migrationBuilder.DropColumn(
                name: "IsSampleData",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "IsSampleData",
                table: "NotificationPreferences");

            migrationBuilder.DropColumn(
                name: "IsSampleData",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "IsSampleData",
                table: "MaintenanceRequests");

            migrationBuilder.DropColumn(
                name: "IsSampleData",
                table: "Leases");

            migrationBuilder.DropColumn(
                name: "IsSampleData",
                table: "LeaseOffers");

            migrationBuilder.DropColumn(
                name: "IsSampleData",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "IsSampleData",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "IsSampleData",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "IsSampleData",
                table: "ChecklistTemplates");

            migrationBuilder.DropColumn(
                name: "IsSampleData",
                table: "ChecklistTemplateItems");

            migrationBuilder.DropColumn(
                name: "IsSampleData",
                table: "Checklists");

            migrationBuilder.DropColumn(
                name: "IsSampleData",
                table: "ChecklistItems");

            migrationBuilder.DropColumn(
                name: "IsSampleData",
                table: "CalendarSettings");

            migrationBuilder.DropColumn(
                name: "IsSampleData",
                table: "CalendarEvents");

            migrationBuilder.DropColumn(
                name: "IsSampleData",
                table: "ApplicationScreenings");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrganizationId",
                table: "Payments",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceNumber",
                table: "Invoices",
                column: "InvoiceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_OrganizationId",
                table: "Invoices",
                column: "OrganizationId");
        }
    }
}
