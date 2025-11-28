using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aquiis.SimpleStart.Data.Migrations
{
    /// <inheritdoc />
    public partial class SetSystemTemplateSectionOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Set logical section orders for system templates
            // Lower numbers appear first
            
            // Property Tour Template (ID 1): Preparation -> Tour -> Follow-Up
            migrationBuilder.Sql(@"
                UPDATE ChecklistTemplateItems SET SectionOrder = 1 WHERE ChecklistTemplateId = 1 AND CategorySection = 'Preparation';
                UPDATE ChecklistTemplateItems SET SectionOrder = 2 WHERE ChecklistTemplateId = 1 AND CategorySection = 'Tour';
                UPDATE ChecklistTemplateItems SET SectionOrder = 3 WHERE ChecklistTemplateId = 1 AND CategorySection = 'Follow-Up';
            ");

            // Move-In Template (ID 2): Entry -> Interior -> Safety -> Kitchen -> Bathroom -> Systems -> Move-In
            migrationBuilder.Sql(@"
                UPDATE ChecklistTemplateItems SET SectionOrder = 1 WHERE ChecklistTemplateId = 2 AND CategorySection = 'Entry';
                UPDATE ChecklistTemplateItems SET SectionOrder = 2 WHERE ChecklistTemplateId = 2 AND CategorySection = 'Interior';
                UPDATE ChecklistTemplateItems SET SectionOrder = 3 WHERE ChecklistTemplateId = 2 AND CategorySection = 'Safety';
                UPDATE ChecklistTemplateItems SET SectionOrder = 4 WHERE ChecklistTemplateId = 2 AND CategorySection = 'Kitchen';
                UPDATE ChecklistTemplateItems SET SectionOrder = 5 WHERE ChecklistTemplateId = 2 AND CategorySection = 'Bathroom';
                UPDATE ChecklistTemplateItems SET SectionOrder = 6 WHERE ChecklistTemplateId = 2 AND CategorySection = 'Systems';
                UPDATE ChecklistTemplateItems SET SectionOrder = 7 WHERE ChecklistTemplateId = 2 AND CategorySection = 'Move-In';
            ");

            // Move-Out Template (ID 3): General -> Interior -> Kitchen -> Bathroom -> Repairs -> Assessment -> Move-Out
            migrationBuilder.Sql(@"
                UPDATE ChecklistTemplateItems SET SectionOrder = 1 WHERE ChecklistTemplateId = 3 AND CategorySection = 'General';
                UPDATE ChecklistTemplateItems SET SectionOrder = 2 WHERE ChecklistTemplateId = 3 AND CategorySection = 'Interior';
                UPDATE ChecklistTemplateItems SET SectionOrder = 3 WHERE ChecklistTemplateId = 3 AND CategorySection = 'Kitchen';
                UPDATE ChecklistTemplateItems SET SectionOrder = 4 WHERE ChecklistTemplateId = 3 AND CategorySection = 'Bathroom';
                UPDATE ChecklistTemplateItems SET SectionOrder = 5 WHERE ChecklistTemplateId = 3 AND CategorySection = 'Repairs';
                UPDATE ChecklistTemplateItems SET SectionOrder = 6 WHERE ChecklistTemplateId = 3 AND CategorySection = 'Assessment';
                UPDATE ChecklistTemplateItems SET SectionOrder = 7 WHERE ChecklistTemplateId = 3 AND CategorySection = 'Move-Out';
            ");

            // Open House Template (ID 4): Preparation -> Setup -> Event -> Closing -> Follow-Up
            migrationBuilder.Sql(@"
                UPDATE ChecklistTemplateItems SET SectionOrder = 1 WHERE ChecklistTemplateId = 4 AND CategorySection = 'Preparation';
                UPDATE ChecklistTemplateItems SET SectionOrder = 2 WHERE ChecklistTemplateId = 4 AND CategorySection = 'Setup';
                UPDATE ChecklistTemplateItems SET SectionOrder = 3 WHERE ChecklistTemplateId = 4 AND CategorySection = 'Event';
                UPDATE ChecklistTemplateItems SET SectionOrder = 4 WHERE ChecklistTemplateId = 4 AND CategorySection = 'Closing';
                UPDATE ChecklistTemplateItems SET SectionOrder = 5 WHERE ChecklistTemplateId = 4 AND CategorySection = 'Follow-Up';
            ");

            // Tenant On-Boarding Template (ID 5): Documentation -> Financial -> Information -> Utilities -> Insurance -> Inspection -> Access -> Safety -> Welcome
            migrationBuilder.Sql(@"
                UPDATE ChecklistTemplateItems SET SectionOrder = 1 WHERE ChecklistTemplateId = 5 AND CategorySection = 'Documentation';
                UPDATE ChecklistTemplateItems SET SectionOrder = 2 WHERE ChecklistTemplateId = 5 AND CategorySection = 'Financial';
                UPDATE ChecklistTemplateItems SET SectionOrder = 3 WHERE ChecklistTemplateId = 5 AND CategorySection = 'Information';
                UPDATE ChecklistTemplateItems SET SectionOrder = 4 WHERE ChecklistTemplateId = 5 AND CategorySection = 'Utilities';
                UPDATE ChecklistTemplateItems SET SectionOrder = 5 WHERE ChecklistTemplateId = 5 AND CategorySection = 'Insurance';
                UPDATE ChecklistTemplateItems SET SectionOrder = 6 WHERE ChecklistTemplateId = 5 AND CategorySection = 'Inspection';
                UPDATE ChecklistTemplateItems SET SectionOrder = 7 WHERE ChecklistTemplateId = 5 AND CategorySection = 'Access';
                UPDATE ChecklistTemplateItems SET SectionOrder = 8 WHERE ChecklistTemplateId = 5 AND CategorySection = 'Safety';
                UPDATE ChecklistTemplateItems SET SectionOrder = 9 WHERE ChecklistTemplateId = 5 AND CategorySection = 'Welcome';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reset all section orders to 0
            migrationBuilder.Sql(@"
                UPDATE ChecklistTemplateItems SET SectionOrder = 0 WHERE OrganizationId = 'System';
            ");
        }
    }
}
