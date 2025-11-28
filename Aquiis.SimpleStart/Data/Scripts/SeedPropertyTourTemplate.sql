-- Seed Property Tour Checklist Template
-- This creates a system-wide template for property showing tours

-- Insert the template
INSERT INTO ChecklistTemplates (Name, Description, Category, IsSystemTemplate, OrganizationId, CreatedBy, CreatedOn, IsDeleted)
VALUES ('Property Tour', 'Standard checklist for conducting property showings with prospective tenants', 'Showing', 1, 'SYSTEM', 'SYSTEM', datetime('now'), 0);

-- Insert template items
INSERT INTO ChecklistTemplateItems (ChecklistTemplateId, ItemText, ItemOrder, CategorySection, IsRequired, AllowsNotes, OrganizationId, CreatedOn, CreatedBy, IsDeleted, RequiresValue) 
SELECT Id, 'Greeted prospect and verified appointment', 1, 'Arrival & Introduction', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0, 0 FROM ChecklistTemplates WHERE Name = 'Property Tour' AND IsSystemTemplate = 1;

INSERT INTO ChecklistTemplateItems (ChecklistTemplateId, ItemText, ItemOrder, CategorySection, IsRequired, AllowsNotes, OrganizationId, CreatedOn, CreatedBy, IsDeleted, RequiresValue) 
SELECT Id, 'Reviewed property exterior and curb appeal', 2, 'Arrival & Introduction', 0, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0, 0 FROM ChecklistTemplates WHERE Name = 'Property Tour' AND IsSystemTemplate = 1;

INSERT INTO ChecklistTemplateItems (ChecklistTemplateId, ItemText, ItemOrder, CategorySection, IsRequired, AllowsNotes, OrganizationId, CreatedOn, CreatedBy, IsDeleted, RequiresValue) 
SELECT Id, 'Showed parking area/garage', 3, 'Arrival & Introduction', 0, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0, 0 FROM ChecklistTemplates WHERE Name = 'Property Tour' AND IsSystemTemplate = 1;

INSERT INTO ChecklistTemplateItems (ChecklistTemplateId, ItemText, ItemOrder, CategorySection, IsRequired, AllowsNotes, OrganizationId, CreatedOn, CreatedBy, IsDeleted, RequiresValue) 
SELECT Id, 'Toured living room/common areas', 4, 'Interior Tour', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0, 0 FROM ChecklistTemplates WHERE Name = 'Property Tour' AND IsSystemTemplate = 1;

INSERT INTO ChecklistTemplateItems (ChecklistTemplateId, ItemText, ItemOrder, CategorySection, IsRequired, AllowsNotes, OrganizationId, CreatedOn, CreatedBy, IsDeleted, RequiresValue) 
SELECT Id, 'Showed all bedrooms', 5, 'Interior Tour', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0, 0 FROM ChecklistTemplates WHERE Name = 'Property Tour' AND IsSystemTemplate = 1;

INSERT INTO ChecklistTemplateItems (ChecklistTemplateId, ItemText, ItemOrder, CategorySection, IsRequired, AllowsNotes, OrganizationId, CreatedOn, CreatedBy, IsDeleted, RequiresValue) 
SELECT Id, 'Showed all bathrooms', 6, 'Interior Tour', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0, 0 FROM ChecklistTemplates WHERE Name = 'Property Tour' AND IsSystemTemplate = 1;

INSERT INTO ChecklistTemplateItems (ChecklistTemplateId, ItemText, ItemOrder, CategorySection, IsRequired, AllowsNotes, OrganizationId, CreatedOn, CreatedBy, IsDeleted, RequiresValue) 
SELECT Id, 'Toured kitchen and demonstrated appliances', 7, 'Kitchen & Appliances', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0, 0 FROM ChecklistTemplates WHERE Name = 'Property Tour' AND IsSystemTemplate = 1;

INSERT INTO ChecklistTemplateItems (ChecklistTemplateId, ItemText, ItemOrder, CategorySection, IsRequired, AllowsNotes, OrganizationId, CreatedOn, CreatedBy, IsDeleted, RequiresValue) 
SELECT Id, 'Explained which appliances are included', 8, 'Kitchen & Appliances', 0, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0, 0 FROM ChecklistTemplates WHERE Name = 'Property Tour' AND IsSystemTemplate = 1;

INSERT INTO ChecklistTemplateItems (ChecklistTemplateId, ItemText, ItemOrder, CategorySection, IsRequired, AllowsNotes, OrganizationId, CreatedOn, CreatedBy, IsDeleted, RequiresValue) 
SELECT Id, 'Explained HVAC system and thermostat controls', 9, 'Utilities & Systems', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0, 0 FROM ChecklistTemplates WHERE Name = 'Property Tour' AND IsSystemTemplate = 1;

INSERT INTO ChecklistTemplateItems (ChecklistTemplateId, ItemText, ItemOrder, CategorySection, IsRequired, AllowsNotes, OrganizationId, CreatedOn, CreatedBy, IsDeleted, RequiresValue) 
SELECT Id, 'Reviewed utility responsibilities (tenant vs landlord)', 10, 'Utilities & Systems', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0, 0 FROM ChecklistTemplates WHERE Name = 'Property Tour' AND IsSystemTemplate = 1;

INSERT INTO ChecklistTemplateItems (ChecklistTemplateId, ItemText, ItemOrder, CategorySection, IsRequired, AllowsNotes, OrganizationId, CreatedOn, CreatedBy, IsDeleted, RequiresValue) 
SELECT Id, 'Showed water heater location', 11, 'Utilities & Systems', 0, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0, 0 FROM ChecklistTemplates WHERE Name = 'Property Tour' AND IsSystemTemplate = 1;

INSERT INTO ChecklistTemplateItems (ChecklistTemplateId, ItemText, ItemOrder, CategorySection, IsRequired, AllowsNotes, OrganizationId, CreatedOn, CreatedBy, IsDeleted, RequiresValue) 
SELECT Id, 'Showed storage areas (closets, attic, basement)', 12, 'Storage & Amenities', 0, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0, 0 FROM ChecklistTemplates WHERE Name = 'Property Tour' AND IsSystemTemplate = 1;

INSERT INTO ChecklistTemplateItems (ChecklistTemplateId, ItemText, ItemOrder, CategorySection, IsRequired, AllowsNotes, OrganizationId, CreatedOn, CreatedBy, IsDeleted, RequiresValue) 
SELECT Id, 'Showed laundry facilities', 13, 'Storage & Amenities', 0, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0, 0 FROM ChecklistTemplates WHERE Name = 'Property Tour' AND IsSystemTemplate = 1;

INSERT INTO ChecklistTemplateItems (ChecklistTemplateId, ItemText, ItemOrder, CategorySection, IsRequired, AllowsNotes, OrganizationId, CreatedOn, CreatedBy, IsDeleted, RequiresValue) 
SELECT Id, 'Showed outdoor space (yard, patio, balcony)', 14, 'Storage & Amenities', 0, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0, 0 FROM ChecklistTemplates WHERE Name = 'Property Tour' AND IsSystemTemplate = 1;

INSERT INTO ChecklistTemplateItems (ChecklistTemplateId, ItemText, ItemOrder, CategorySection, IsRequired, AllowsNotes, OrganizationId, CreatedOn, CreatedBy, IsDeleted, RequiresValue) 
SELECT Id, 'Discussed monthly rent amount', 15, 'Lease Terms', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0, 0 FROM ChecklistTemplates WHERE Name = 'Property Tour' AND IsSystemTemplate = 1;

INSERT INTO ChecklistTemplateItems (ChecklistTemplateId, ItemText, ItemOrder, CategorySection, IsRequired, AllowsNotes, OrganizationId, CreatedOn, CreatedBy, IsDeleted, RequiresValue) 
SELECT Id, 'Explained security deposit and move-in costs', 16, 'Lease Terms', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0, 0 FROM ChecklistTemplates WHERE Name = 'Property Tour' AND IsSystemTemplate = 1;

INSERT INTO ChecklistTemplateItems (ChecklistTemplateId, ItemText, ItemOrder, CategorySection, IsRequired, AllowsNotes, OrganizationId, CreatedOn, CreatedBy, IsDeleted, RequiresValue) 
SELECT Id, 'Reviewed lease term length and start date', 17, 'Lease Terms', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0, 0 FROM ChecklistTemplates WHERE Name = 'Property Tour' AND IsSystemTemplate = 1;

INSERT INTO ChecklistTemplateItems (ChecklistTemplateId, ItemText, ItemOrder, CategorySection, IsRequired, AllowsNotes, OrganizationId, CreatedOn, CreatedBy, IsDeleted, RequiresValue) 
SELECT Id, 'Explained pet policy', 18, 'Lease Terms', 0, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0, 0 FROM ChecklistTemplates WHERE Name = 'Property Tour' AND IsSystemTemplate = 1;

INSERT INTO ChecklistTemplateItems (ChecklistTemplateId, ItemText, ItemOrder, CategorySection, IsRequired, AllowsNotes, OrganizationId, CreatedOn, CreatedBy, IsDeleted, RequiresValue) 
SELECT Id, 'Explained application process and requirements', 19, 'Next Steps', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0, 0 FROM ChecklistTemplates WHERE Name = 'Property Tour' AND IsSystemTemplate = 1;

INSERT INTO ChecklistTemplateItems (ChecklistTemplateId, ItemText, ItemOrder, CategorySection, IsRequired, AllowsNotes, OrganizationId, CreatedOn, CreatedBy, IsDeleted, RequiresValue) 
SELECT Id, 'Reviewed screening process (background, credit check)', 20, 'Next Steps', 0, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0, 0 FROM ChecklistTemplates WHERE Name = 'Property Tour' AND IsSystemTemplate = 1;

INSERT INTO ChecklistTemplateItems (ChecklistTemplateId, ItemText, ItemOrder, CategorySection, IsRequired, AllowsNotes, OrganizationId, CreatedOn, CreatedBy, IsDeleted, RequiresValue) 
SELECT Id, 'Answered all prospect questions', 21, 'Next Steps', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0, 0 FROM ChecklistTemplates WHERE Name = 'Property Tour' AND IsSystemTemplate = 1;

INSERT INTO ChecklistTemplateItems (ChecklistTemplateId, ItemText, ItemOrder, CategorySection, IsRequired, AllowsNotes, OrganizationId, CreatedOn, CreatedBy, IsDeleted, RequiresValue) 
SELECT Id, 'Prospect Interest Level', 22, 'Assessment', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0, 1 FROM ChecklistTemplates WHERE Name = 'Property Tour' AND IsSystemTemplate = 1;

INSERT INTO ChecklistTemplateItems (ChecklistTemplateId, ItemText, ItemOrder, CategorySection, IsRequired, AllowsNotes, OrganizationId, CreatedOn, CreatedBy, IsDeleted, RequiresValue) 
SELECT Id, 'Overall showing feedback and notes', 23, 'Assessment', 0, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0, 1 FROM ChecklistTemplates WHERE Name = 'Property Tour' AND IsSystemTemplate = 1;
