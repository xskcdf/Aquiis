using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aquiis.Infrastructure.Migrations
{
    /// <summary>
    /// Documentation-only migration: Moved OrganizationId property from individual entity classes to BaseModel.
    /// This is a code refactoring that eliminates duplicate property declarations across 30+ entities.
    /// No database schema changes - OrganizationId columns already existed in all tables and remain unchanged.
    /// UserProfile shadows BaseModel.OrganizationId with 'new' keyword to maintain nullable semantics.
    /// </summary>
    /// <inheritdoc />
    public partial class ConsolidateOrganizationIdToBaseModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
