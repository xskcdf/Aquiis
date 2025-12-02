using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aquiis.SimpleStart.Migrations
{
    /// <inheritdoc />
    public partial class AddLeaseOfferTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LeaseOfferId",
                table: "Leases",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LeaseOffers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrganizationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RentalApplicationId = table.Column<int>(type: "INTEGER", nullable: false),
                    PropertyId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProspectiveTenantId = table.Column<int>(type: "INTEGER", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MonthlyRent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SecurityDeposit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Terms = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    OfferedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExpiresOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    RespondedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ResponseNotes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ConvertedLeaseId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaseOffers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaseOffers_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeaseOffers_ProspectiveTenants_ProspectiveTenantId",
                        column: x => x.ProspectiveTenantId,
                        principalTable: "ProspectiveTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeaseOffers_RentalApplications_RentalApplicationId",
                        column: x => x.RentalApplicationId,
                        principalTable: "RentalApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LeaseOffers_PropertyId",
                table: "LeaseOffers",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaseOffers_ProspectiveTenantId",
                table: "LeaseOffers",
                column: "ProspectiveTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaseOffers_RentalApplicationId",
                table: "LeaseOffers",
                column: "RentalApplicationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeaseOffers");

            migrationBuilder.DropColumn(
                name: "LeaseOfferId",
                table: "Leases");
        }
    }
}
