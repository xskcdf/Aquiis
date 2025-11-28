using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aquiis.SimpleStart.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameScheduledDateTimeToScheduledOn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ScheduledDateTime",
                table: "Tours",
                newName: "ScheduledOn");

            migrationBuilder.RenameIndex(
                name: "IX_Tours_ScheduledDateTime",
                table: "Tours",
                newName: "IX_Tours_ScheduledOn");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ScheduledOn",
                table: "Tours",
                newName: "ScheduledDateTime");

            migrationBuilder.RenameIndex(
                name: "IX_Tours_ScheduledOn",
                table: "Tours",
                newName: "IX_Tours_ScheduledDateTime");
        }
    }
}
