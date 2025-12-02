using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Aquiis.SimpleStart.Migrations
{
    /// <inheritdoc />
    public partial class SeedChecklistTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ChecklistTemplates",
                columns: new[] { "Id", "Category", "CreatedBy", "CreatedOn", "Description", "IsDeleted", "IsSystemTemplate", "LastModifiedBy", "LastModifiedOn", "Name", "OrganizationId" },
                values: new object[,]
                {
                    { 1, "Showing", "SYSTEM", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Standard property showing checklist", false, true, null, null, "Property Tour", "SYSTEM" },
                    { 2, "MoveIn", "SYSTEM", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Move-in inspection checklist", false, true, null, null, "Move-In", "SYSTEM" },
                    { 3, "MoveOut", "SYSTEM", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Move-out inspection checklist", false, true, null, null, "Move-Out", "SYSTEM" },
                    { 4, "Showing", "SYSTEM", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Open house event checklist", false, true, null, null, "Open House", "SYSTEM" }
                });

            migrationBuilder.InsertData(
                table: "ChecklistTemplateItems",
                columns: new[] { "Id", "AllowsNotes", "CategorySection", "ChecklistTemplateId", "CreatedBy", "CreatedOn", "IsDeleted", "IsRequired", "ItemOrder", "ItemText", "LastModifiedBy", "LastModifiedOn", "OrganizationId", "RequiresValue", "SectionOrder" },
                values: new object[,]
                {
                    { 1, true, "Arrival & Introduction", 1, "SYSTEM", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 1, "Greeted prospect and verified appointment", null, null, "SYSTEM", false, 1 },
                    { 2, true, "Arrival & Introduction", 1, "SYSTEM", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 2, "Reviewed property exterior and curb appeal", null, null, "SYSTEM", false, 1 },
                    { 3, true, "Arrival & Introduction", 1, "SYSTEM", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 3, "Showed parking area/garage", null, null, "SYSTEM", false, 1 },
                    { 4, true, "Interior Tour", 1, "SYSTEM", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 4, "Toured living room/common areas", null, null, "SYSTEM", false, 2 },
                    { 5, true, "Interior Tour", 1, "SYSTEM", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 5, "Showed all bedrooms", null, null, "SYSTEM", false, 2 },
                    { 6, true, "Interior Tour", 1, "SYSTEM", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 6, "Showed all bathrooms", null, null, "SYSTEM", false, 2 },
                    { 7, true, "Kitchen & Appliances", 1, "SYSTEM", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 7, "Toured kitchen and demonstrated appliances", null, null, "SYSTEM", false, 3 },
                    { 8, true, "Kitchen & Appliances", 1, "SYSTEM", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 8, "Explained which appliances are included", null, null, "SYSTEM", false, 3 },
                    { 9, true, "Utilities & Systems", 1, "SYSTEM", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 9, "Explained HVAC system and thermostat controls", null, null, "SYSTEM", false, 4 },
                    { 10, true, "Utilities & Systems", 1, "SYSTEM", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 10, "Reviewed utility responsibilities (tenant vs landlord)", null, null, "SYSTEM", false, 4 },
                    { 11, true, "Utilities & Systems", 1, "SYSTEM", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 11, "Showed water heater location", null, null, "SYSTEM", false, 4 },
                    { 12, true, "Storage & Amenities", 1, "SYSTEM", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 12, "Showed storage areas (closets, attic, basement)", null, null, "SYSTEM", false, 5 },
                    { 13, true, "Storage & Amenities", 1, "SYSTEM", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 13, "Showed laundry facilities", null, null, "SYSTEM", false, 5 },
                    { 14, true, "Storage & Amenities", 1, "SYSTEM", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 14, "Showed outdoor space (yard, patio, balcony)", null, null, "SYSTEM", false, 5 },
                    { 15, true, "Lease Terms", 1, "SYSTEM", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 15, "Discussed monthly rent amount", null, null, "SYSTEM", true, 6 },
                    { 16, true, "Lease Terms", 1, "SYSTEM", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 16, "Explained security deposit and move-in costs", null, null, "SYSTEM", true, 6 },
                    { 17, true, "Lease Terms", 1, "SYSTEM", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 17, "Reviewed lease term length and start date", null, null, "SYSTEM", false, 6 },
                    { 18, true, "Lease Terms", 1, "SYSTEM", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 18, "Explained pet policy", null, null, "SYSTEM", false, 6 },
                    { 19, true, "Next Steps", 1, "SYSTEM", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 19, "Explained application process and requirements", null, null, "SYSTEM", false, 7 },
                    { 20, true, "Next Steps", 1, "SYSTEM", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 20, "Reviewed screening process (background, credit check)", null, null, "SYSTEM", false, 7 },
                    { 21, true, "Next Steps", 1, "SYSTEM", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 21, "Answered all prospect questions", null, null, "SYSTEM", false, 7 },
                    { 22, true, "Assessment", 1, "SYSTEM", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 22, "Prospect Interest Level", null, null, "SYSTEM", true, 8 },
                    { 23, true, "Assessment", 1, "SYSTEM", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 23, "Overall showing feedback and notes", null, null, "SYSTEM", true, 8 },
                    { 24, true, "General", 2, "SYSTEM", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 1, "Document property condition", null, null, "SYSTEM", false, 1 },
                    { 25, true, "General", 2, "SYSTEM", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 2, "Collect keys and access codes", null, null, "SYSTEM", false, 1 },
                    { 26, true, "General", 2, "SYSTEM", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 3, "Review lease terms with tenant", null, null, "SYSTEM", false, 1 },
                    { 27, true, "General", 3, "SYSTEM", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 1, "Inspect property condition", null, null, "SYSTEM", false, 1 },
                    { 28, true, "General", 3, "SYSTEM", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 2, "Collect all keys and access devices", null, null, "SYSTEM", false, 1 },
                    { 29, true, "General", 3, "SYSTEM", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 3, "Document damages and needed repairs", null, null, "SYSTEM", false, 1 },
                    { 30, true, "Preparation", 4, "SYSTEM", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 1, "Set up signage and directional markers", null, null, "SYSTEM", false, 1 },
                    { 31, true, "Preparation", 4, "SYSTEM", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 2, "Prepare information packets", null, null, "SYSTEM", false, 1 },
                    { 32, true, "Preparation", 4, "SYSTEM", new DateTime(2025, 11, 30, 0, 0, 0, 0, DateTimeKind.Utc), false, true, 3, "Set up visitor sign-in sheet", null, null, "SYSTEM", false, 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "ChecklistTemplates",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ChecklistTemplates",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ChecklistTemplates",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ChecklistTemplates",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
