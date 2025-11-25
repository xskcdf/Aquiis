using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aquiis.SimpleStart.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddChecklistIdToShowings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChecklistId",
                table: "Showings",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Showings_ChecklistId",
                table: "Showings",
                column: "ChecklistId");

            migrationBuilder.AddForeignKey(
                name: "FK_Showings_Checklists_ChecklistId",
                table: "Showings",
                column: "ChecklistId",
                principalTable: "Checklists",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Showings_Checklists_ChecklistId",
                table: "Showings");

            migrationBuilder.DropIndex(
                name: "IX_Showings_ChecklistId",
                table: "Showings");

            migrationBuilder.DropColumn(
                name: "ChecklistId",
                table: "Showings");
        }
    }
}
