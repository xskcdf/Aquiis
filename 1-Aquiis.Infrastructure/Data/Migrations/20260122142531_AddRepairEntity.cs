using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aquiis.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRepairEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Repairs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PropertyId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MaintenanceRequestId = table.Column<Guid>(type: "TEXT", nullable: true),
                    LeaseId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    RepairType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CompletedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DurationMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    CompletedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ContractorName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ContractorPhone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    ContactId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    PartsReplaced = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    WarrantyApplies = table.Column<bool>(type: "INTEGER", nullable: false),
                    WarrantyExpiresOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repairs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Repairs_Leases_LeaseId",
                        column: x => x.LeaseId,
                        principalTable: "Leases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Repairs_MaintenanceRequests_MaintenanceRequestId",
                        column: x => x.MaintenanceRequestId,
                        principalTable: "MaintenanceRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Repairs_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Repairs_CompletedOn",
                table: "Repairs",
                column: "CompletedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Repairs_LeaseId",
                table: "Repairs",
                column: "LeaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Repairs_MaintenanceRequestId",
                table: "Repairs",
                column: "MaintenanceRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Repairs_OrganizationId",
                table: "Repairs",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Repairs_PropertyId",
                table: "Repairs",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_Repairs_RepairType",
                table: "Repairs",
                column: "RepairType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Repairs");
        }
    }
}
