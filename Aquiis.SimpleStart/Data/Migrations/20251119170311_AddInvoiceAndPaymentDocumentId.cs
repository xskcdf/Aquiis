using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aquiis.SimpleStart.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoiceAndPaymentDocumentId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DocumentId",
                table: "Payments",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DocumentId",
                table: "Invoices",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_DocumentId",
                table: "Payments",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_DocumentId",
                table: "Invoices",
                column: "DocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Documents_DocumentId",
                table: "Invoices",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Documents_DocumentId",
                table: "Payments",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Documents_DocumentId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Documents_DocumentId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_DocumentId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_DocumentId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "Invoices");
        }
    }
}
