using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aquiis.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameCompletedByToContactPersonAndAddContractorId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CompletedBy",
                table: "Repairs",
                newName: "ContactPerson");

            migrationBuilder.AddColumn<Guid>(
                name: "ContractorId",
                table: "Repairs",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContractorId",
                table: "Repairs");

            migrationBuilder.RenameColumn(
                name: "ContactPerson",
                table: "Repairs",
                newName: "CompletedBy");
        }
    }
}
