using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aquiis.SimpleStart.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameShowingsToTours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create new Tours table
            migrationBuilder.CreateTable(
                name: "Tours",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProspectiveTenantId = table.Column<int>(type: "INTEGER", nullable: false),
                    PropertyId = table.Column<int>(type: "INTEGER", nullable: false),
                    ScheduledDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DurationMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Feedback = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    InterestLevel = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    ConductedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ChecklistId = table.Column<int>(type: "INTEGER", nullable: true),
                    OrganizationId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tours_Checklists_ChecklistId",
                        column: x => x.ChecklistId,
                        principalTable: "Checklists",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tours_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tours_ProspectiveTenants_ProspectiveTenantId",
                        column: x => x.ProspectiveTenantId,
                        principalTable: "ProspectiveTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // Copy data from Showings to Tours
            migrationBuilder.Sql(@"
                INSERT INTO Tours (Id, ProspectiveTenantId, PropertyId, ScheduledDateTime, DurationMinutes, 
                    Status, Feedback, InterestLevel, ConductedBy, ChecklistId, OrganizationId, 
                    CreatedOn, CreatedBy, LastModifiedOn, LastModifiedBy, IsDeleted)
                SELECT Id, ProspectiveTenantId, PropertyId, ScheduledDateTime, DurationMinutes, 
                    Status, Feedback, InterestLevel, ConductedBy, ChecklistId, OrganizationId, 
                    CreatedOn, CreatedBy, LastModifiedOn, LastModifiedBy, IsDeleted
                FROM Showings;
            ");

            // Drop old Showings table
            migrationBuilder.DropTable(
                name: "Showings");

            // Create indexes on Tours table
            migrationBuilder.CreateIndex(
                name: "IX_Tours_ChecklistId",
                table: "Tours",
                column: "ChecklistId");

            migrationBuilder.CreateIndex(
                name: "IX_Tours_OrganizationId",
                table: "Tours",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Tours_PropertyId",
                table: "Tours",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_Tours_ProspectiveTenantId",
                table: "Tours",
                column: "ProspectiveTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Tours_ScheduledDateTime",
                table: "Tours",
                column: "ScheduledDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_Tours_Status",
                table: "Tours",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverse: Rename table back to Showings
            migrationBuilder.RenameTable(
                name: "Tours",
                newName: "Showings");
        }
    }
}
