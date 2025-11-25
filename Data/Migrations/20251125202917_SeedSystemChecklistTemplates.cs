using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aquiis.SimpleStart.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedSystemChecklistTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var now = DateTime.UtcNow;
            var systemOrgId = "System";
            var systemUser = "System";

            // Insert System Checklist Templates
            migrationBuilder.InsertData(
                table: "ChecklistTemplates",
                columns: new[] { "Id", "Name", "Description", "Category", "IsSystemTemplate", "OrganizationId", "CreatedOn", "CreatedBy", "LastModifiedOn", "LastModifiedBy", "IsDeleted" },
                values: new object[,]
                {
                    { 1, "Property Tour Checklist", "Comprehensive checklist for property showings and tours", "Tour", true, systemOrgId, now, systemUser, null, null, false },
                    { 2, "Move-In Checklist", "Detailed inspection checklist for tenant move-in", "Move-In", true, systemOrgId, now, systemUser, null, null, false },
                    { 3, "Move-Out Checklist", "Detailed inspection checklist for tenant move-out", "Move-Out", true, systemOrgId, now, systemUser, null, null, false },
                    { 4, "Open House Checklist", "Preparation and execution checklist for open house events", "Open House", true, systemOrgId, now, systemUser, null, null, false },
                    { 5, "Tenant On-Boarding Checklist", "Complete checklist for new tenant on-boarding process", "On-Boarding", true, systemOrgId, now, systemUser, null, null, false }
                });

            // Property Tour Checklist Items
            migrationBuilder.InsertData(
                table: "ChecklistTemplateItems",
                columns: new[] { "Id", "ChecklistTemplateId", "ItemText", "ItemOrder", "CategorySection", "IsRequired", "RequiresValue", "AllowsNotes", "OrganizationId", "CreatedOn", "CreatedBy", "LastModifiedOn", "LastModifiedBy", "IsDeleted" },
                values: new object[,]
                {
                    { 1, 1, "Verify prospect appointment details", 1, "Preparation", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 2, 1, "Review property features and amenities", 2, "Preparation", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 3, 1, "Prepare property tour route", 3, "Preparation", false, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 4, 1, "Greet prospect and confirm their requirements", 4, "Tour", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 5, 1, "Showcase exterior and curb appeal", 5, "Tour", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 6, 1, "Tour all rooms and living spaces", 6, "Tour", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 7, 1, "Demonstrate appliances and utilities", 7, "Tour", false, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 8, 1, "Highlight storage and parking", 8, "Tour", false, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 9, 1, "Discuss lease terms and rental amount", 9, "Follow-Up", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 10, 1, "Answer prospect questions", 10, "Follow-Up", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 11, 1, "Provide application information", 11, "Follow-Up", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 12, 1, "Schedule follow-up contact", 12, "Follow-Up", false, false, true, systemOrgId, now, systemUser, null, null, false }
                });

            // Move-In Checklist Items
            migrationBuilder.InsertData(
                table: "ChecklistTemplateItems",
                columns: new[] { "Id", "ChecklistTemplateId", "ItemText", "ItemOrder", "CategorySection", "IsRequired", "RequiresValue", "AllowsNotes", "OrganizationId", "CreatedOn", "CreatedBy", "LastModifiedOn", "LastModifiedBy", "IsDeleted" },
                values: new object[,]
                {
                    { 13, 2, "Front door and locks functioning", 13, "Entry", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 14, 2, "Windows open/close properly", 14, "Interior", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 15, 2, "Window screens intact", 15, "Interior", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 16, 2, "Walls free of damage", 16, "Interior", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 17, 2, "Floors and carpeting condition", 17, "Interior", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 18, 2, "Light fixtures operational", 18, "Interior", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 19, 2, "Electrical outlets working", 19, "Interior", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 20, 2, "Smoke detectors installed and working", 20, "Safety", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 21, 2, "Carbon monoxide detectors working", 21, "Safety", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 22, 2, "Kitchen appliances operational", 22, "Kitchen", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 23, 2, "Kitchen cabinets and drawers functional", 23, "Kitchen", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 24, 2, "Kitchen sink and faucet working", 24, "Kitchen", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 25, 2, "Bathroom fixtures functional", 25, "Bathroom", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 26, 2, "Bathroom plumbing working properly", 26, "Bathroom", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 27, 2, "HVAC system operational", 27, "Systems", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 28, 2, "Water heater working", 28, "Systems", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 29, 2, "Provide keys and access devices", 29, "Move-In", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 30, 2, "Review lease terms with tenant", 30, "Move-In", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 31, 2, "Document existing damages with photos", 31, "Move-In", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 32, 2, "Tenant signature on move-in inspection", 32, "Move-In", true, false, true, systemOrgId, now, systemUser, null, null, false }
                });

            // Move-Out Checklist Items
            migrationBuilder.InsertData(
                table: "ChecklistTemplateItems",
                columns: new[] { "Id", "ChecklistTemplateId", "ItemText", "ItemOrder", "CategorySection", "IsRequired", "RequiresValue", "AllowsNotes", "OrganizationId", "CreatedOn", "CreatedBy", "LastModifiedOn", "LastModifiedBy", "IsDeleted" },
                values: new object[,]
                {
                    { 33, 3, "Property cleaned to original condition", 33, "General", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 34, 3, "All trash and debris removed", 34, "General", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 35, 3, "Walls repaired and painted if needed", 35, "Interior", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 36, 3, "Carpets cleaned professionally", 36, "Interior", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 37, 3, "Windows and blinds cleaned", 37, "Interior", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 38, 3, "Kitchen deep cleaned", 38, "Kitchen", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 39, 3, "Appliances cleaned inside and out", 39, "Kitchen", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 40, 3, "Bathroom thoroughly sanitized", 40, "Bathroom", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 41, 3, "Light bulbs replaced if burned out", 41, "Repairs", false, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 42, 3, "Damages beyond normal wear documented", 42, "Assessment", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 43, 3, "All keys and access devices returned", 43, "Move-Out", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 44, 3, "Forwarding address collected", 44, "Move-Out", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 45, 3, "Final utility readings recorded", 45, "Move-Out", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 46, 3, "Security deposit disposition explained", 46, "Move-Out", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 47, 3, "Tenant signature on move-out inspection", 47, "Move-Out", true, false, true, systemOrgId, now, systemUser, null, null, false }
                });

            // Open House Checklist Items
            migrationBuilder.InsertData(
                table: "ChecklistTemplateItems",
                columns: new[] { "Id", "ChecklistTemplateId", "ItemText", "ItemOrder", "CategorySection", "IsRequired", "RequiresValue", "AllowsNotes", "OrganizationId", "CreatedOn", "CreatedBy", "LastModifiedOn", "LastModifiedBy", "IsDeleted" },
                values: new object[,]
                {
                    { 48, 4, "Schedule and advertise open house", 48, "Preparation", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 49, 4, "Deep clean entire property", 49, "Preparation", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 50, 4, "Stage property for showing", 50, "Preparation", false, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 51, 4, "Prepare information packets", 51, "Preparation", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 52, 4, "Post directional signage", 52, "Setup", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 53, 4, "Turn on all lights", 53, "Setup", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 54, 4, "Adjust temperature for comfort", 54, "Setup", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 55, 4, "Set up sign-in sheet", 55, "Setup", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 56, 4, "Greet and register visitors", 56, "Event", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 57, 4, "Provide property tours and information", 57, "Event", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 58, 4, "Answer questions and collect feedback", 58, "Event", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 59, 4, "Collect contact information for follow-up", 59, "Event", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 60, 4, "Remove signage and secure property", 60, "Closing", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 61, 4, "Follow up with interested prospects", 61, "Follow-Up", true, false, true, systemOrgId, now, systemUser, null, null, false }
                });

            // Tenant On-Boarding Checklist Items
            migrationBuilder.InsertData(
                table: "ChecklistTemplateItems",
                columns: new[] { "Id", "ChecklistTemplateId", "ItemText", "ItemOrder", "CategorySection", "IsRequired", "RequiresValue", "AllowsNotes", "OrganizationId", "CreatedOn", "CreatedBy", "LastModifiedOn", "LastModifiedBy", "IsDeleted" },
                values: new object[,]
                {
                    { 62, 5, "Review and sign lease agreement", 62, "Documentation", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 63, 5, "Collect first month's rent", 63, "Financial", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 64, 5, "Collect security deposit", 64, "Financial", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 65, 5, "Provide receipt for all payments", 65, "Financial", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 66, 5, "Setup rent payment method", 66, "Financial", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 67, 5, "Provide copy of lease and move-in inspection", 67, "Documentation", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 68, 5, "Provide property rules and regulations", 68, "Information", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 69, 5, "Explain maintenance request process", 69, "Information", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 70, 5, "Provide emergency contact information", 70, "Information", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 71, 5, "Explain trash and recycling procedures", 71, "Information", false, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 72, 5, "Review parking arrangements", 72, "Information", false, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 73, 5, "Provide utility company contact information", 73, "Utilities", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 74, 5, "Confirm renter's insurance obtained", 74, "Insurance", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 75, 5, "Complete move-in inspection checklist", 75, "Inspection", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 76, 5, "Provide all keys and access devices", 76, "Access", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 77, 5, "Test smoke and CO detectors with tenant", 77, "Safety", true, false, true, systemOrgId, now, systemUser, null, null, false },
                    { 78, 5, "Welcome tenant and answer final questions", 78, "Welcome", true, false, true, systemOrgId, now, systemUser, null, null, false }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Delete checklist template items
            migrationBuilder.DeleteData(
                table: "ChecklistTemplateItems",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78 });

            // Delete checklist templates
            migrationBuilder.DeleteData(
                table: "ChecklistTemplates",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2, 3, 4, 5 });
        }
    }
}
