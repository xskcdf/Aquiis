using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aquiis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateExistingSampleDataFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Set IsSampleData = true for all existing records created by SystemUser
            // SystemUser.Id = '00000000-0000-0000-0000-000000000001'
            var systemUserId = "00000000-0000-0000-0000-000000000001";

            // Update Properties
            migrationBuilder.Sql(
                $"UPDATE Properties SET IsSampleData = 1 WHERE CreatedBy = '{systemUserId}';");

            // Update Tenants
            migrationBuilder.Sql(
                $"UPDATE Tenants SET IsSampleData = 1 WHERE CreatedBy = '{systemUserId}';");

            // Update Leases
            migrationBuilder.Sql(
                $"UPDATE Leases SET IsSampleData = 1 WHERE CreatedBy = '{systemUserId}';");

            // Update Invoices
            migrationBuilder.Sql(
                $"UPDATE Invoices SET IsSampleData = 1 WHERE CreatedBy = '{systemUserId}';");

            // Update Payments
            migrationBuilder.Sql(
                $"UPDATE Payments SET IsSampleData = 1 WHERE CreatedBy = '{systemUserId}';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reset IsSampleData = false for records that were marked as sample data
            var systemUserId = "00000000-0000-0000-0000-000000000001";

            // Reset Properties
            migrationBuilder.Sql(
                $"UPDATE Properties SET IsSampleData = 0 WHERE CreatedBy = '{systemUserId}';");

            // Reset Tenants
            migrationBuilder.Sql(
                $"UPDATE Tenants SET IsSampleData = 0 WHERE CreatedBy = '{systemUserId}';");

            // Reset Leases
            migrationBuilder.Sql(
                $"UPDATE Leases SET IsSampleData = 0 WHERE CreatedBy = '{systemUserId}';");

            // Reset Invoices
            migrationBuilder.Sql(
                $"UPDATE Invoices SET IsSampleData = 0 WHERE CreatedBy = '{systemUserId}';");

            // Reset Payments
            migrationBuilder.Sql(
                $"UPDATE Payments SET IsSampleData = 0 WHERE CreatedBy = '{systemUserId}';");
        }
    }
}
