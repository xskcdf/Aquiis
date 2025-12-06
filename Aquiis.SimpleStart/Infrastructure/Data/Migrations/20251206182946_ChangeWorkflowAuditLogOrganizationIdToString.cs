using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aquiis.SimpleStart.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeWorkflowAuditLogOrganizationIdToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OrganizationId",
                table: "WorkflowAuditLogs",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "OrganizationId",
                table: "WorkflowAuditLogs",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }
    }
}
