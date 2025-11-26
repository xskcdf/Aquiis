using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aquiis.SimpleStart.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSectionOrderToChecklistTemplateItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SectionOrder",
                table: "ChecklistTemplateItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SectionOrder",
                table: "ChecklistTemplateItems");
        }
    }
}
