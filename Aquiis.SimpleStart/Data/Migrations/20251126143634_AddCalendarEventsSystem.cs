using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aquiis.SimpleStart.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCalendarEventsSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CalendarEventId",
                table: "Tours",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CalendarEventId",
                table: "MaintenanceRequests",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CalendarEventId",
                table: "Inspections",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CalendarEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    StartOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DurationMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    EventType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    PropertyId = table.Column<int>(type: "INTEGER", nullable: true),
                    Location = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Color = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    SourceEntityId = table.Column<int>(type: "INTEGER", nullable: true),
                    SourceEntityType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    OrganizationId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalendarEvents_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_EventType",
                table: "CalendarEvents",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_OrganizationId",
                table: "CalendarEvents",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_PropertyId",
                table: "CalendarEvents",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_SourceEntityId",
                table: "CalendarEvents",
                column: "SourceEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_SourceEntityType_SourceEntityId",
                table: "CalendarEvents",
                columns: new[] { "SourceEntityType", "SourceEntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_StartOn",
                table: "CalendarEvents",
                column: "StartOn");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalendarEvents");

            migrationBuilder.DropColumn(
                name: "CalendarEventId",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "CalendarEventId",
                table: "MaintenanceRequests");

            migrationBuilder.DropColumn(
                name: "CalendarEventId",
                table: "Inspections");
        }
    }
}
