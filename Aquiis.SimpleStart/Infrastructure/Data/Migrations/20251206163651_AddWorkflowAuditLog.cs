using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aquiis.SimpleStart.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkflowAuditLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkflowAuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EntityType = table.Column<string>(type: "TEXT", nullable: false),
                    EntityId = table.Column<int>(type: "INTEGER", nullable: false),
                    FromStatus = table.Column<string>(type: "TEXT", nullable: true),
                    ToStatus = table.Column<string>(type: "TEXT", nullable: false),
                    Action = table.Column<string>(type: "TEXT", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", nullable: true),
                    PerformedBy = table.Column<string>(type: "TEXT", nullable: false),
                    PerformedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<int>(type: "INTEGER", nullable: false),
                    Metadata = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowAuditLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowAuditLogs_Action",
                table: "WorkflowAuditLogs",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowAuditLogs_EntityId",
                table: "WorkflowAuditLogs",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowAuditLogs_EntityType",
                table: "WorkflowAuditLogs",
                column: "EntityType");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowAuditLogs_EntityType_EntityId",
                table: "WorkflowAuditLogs",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowAuditLogs_OrganizationId",
                table: "WorkflowAuditLogs",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowAuditLogs_PerformedBy",
                table: "WorkflowAuditLogs",
                column: "PerformedBy");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowAuditLogs_PerformedOn",
                table: "WorkflowAuditLogs",
                column: "PerformedOn");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkflowAuditLogs");
        }
    }
}
