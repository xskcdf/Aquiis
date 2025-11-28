using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aquiis.SimpleStart.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceChecklistItemStatusWithValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "ChecklistItems");

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "ChecklistItems",
                type: "TEXT",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Value",
                table: "ChecklistItems");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "ChecklistItems",
                type: "TEXT",
                maxLength: 50,
                nullable: true);
        }
    }
}
