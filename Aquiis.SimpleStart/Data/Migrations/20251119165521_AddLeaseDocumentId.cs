using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aquiis.SimpleStart.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLeaseDocumentId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DocumentId",
                table: "Leases",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Leases_DocumentId",
                table: "Leases",
                column: "DocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Leases_Documents_DocumentId",
                table: "Leases",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Leases_Documents_DocumentId",
                table: "Leases");

            migrationBuilder.DropIndex(
                name: "IX_Leases_DocumentId",
                table: "Leases");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "Leases");
        }
    }
}
