using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aquiis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixInvoicePaymentUniqueIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the incorrect single-column unique index on Invoices.InvoiceNumber
            // This index was preventing different organizations from using the same invoice numbers
            migrationBuilder.DropIndex(
                name: "IX_Invoices_InvoiceNumber",
                table: "Invoices");

            // Create correct composite unique index on Invoices(OrganizationId, InvoiceNumber)
            // This allows different organizations to have the same invoice number (multi-tenant safe)
            migrationBuilder.CreateIndex(
                name: "IX_Invoice_OrgId_InvoiceNumber",
                table: "Invoices",
                columns: new[] { "OrganizationId", "InvoiceNumber" },
                unique: true);

            // Create composite unique index on Payments(OrganizationId, PaymentNumber)
            // This ensures payment numbers are unique within each organization
            migrationBuilder.CreateIndex(
                name: "IX_Payment_OrgId_PaymentNumber",
                table: "Payments",
                columns: new[] { "OrganizationId", "PaymentNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the composite unique indexes
            migrationBuilder.DropIndex(
                name: "IX_Invoice_OrgId_InvoiceNumber",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Payment_OrgId_PaymentNumber",
                table: "Payments");

            // Restore the original (incorrect) single-column index
            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceNumber",
                table: "Invoices",
                column: "InvoiceNumber",
                unique: true);
        }
    }
}
