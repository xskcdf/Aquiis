using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aquiis.SimpleStart.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddInspectionDocumentId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DocumentId",
                table: "Inspections",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_DocumentId",
                table: "Inspections",
                column: "DocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Inspections_Documents_DocumentId",
                table: "Inspections",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inspections_Documents_DocumentId",
                table: "Inspections");

            migrationBuilder.DropIndex(
                name: "IX_Inspections_DocumentId",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "Inspections");
        }
    }
}
