-- =============================================
-- Script: 50_Insert-DefaultChecklistTemplates.sql
-- Description: Seeds default system checklist templates and their items
-- Author: System
-- Date: 2025-11-23
-- =============================================

-- Move-In Checklist Template
INSERT INTO ChecklistTemplates (Name, Description, Category, IsSystemTemplate, OrganizationId, CreatedOn, CreatedBy, IsDeleted)
VALUES ('Move-In Checklist', 'Standard move-in inspection checklist for new tenants', 'Move-In', 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0);

-- Get the template ID
-- SQLite uses last_insert_rowid() to get the last inserted ID
-- For Move-In template, we'll use ID 1 (assuming first insert)

-- Move-In Template Items
INSERT INTO ChecklistTemplateItems (ChecklistTemplateId, ItemText, ItemOrder, CategorySection, IsRequired, AllowsNotes, OrganizationId, CreatedOn, CreatedBy, IsDeleted)
VALUES
  (1, 'Keys provided to tenant', 1, 'Keys & Access', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (1, 'Remote controls provided (garage, gate, etc.)', 2, 'Keys & Access', 0, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (1, 'Parking pass provided', 3, 'Keys & Access', 0, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (1, 'Electric meter reading recorded', 4, 'Utilities', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (1, 'Gas meter reading recorded', 5, 'Utilities', 0, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (1, 'Water meter reading recorded', 6, 'Utilities', 0, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (1, 'Security deposit amount confirmed', 7, 'Financial', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (1, 'Walls and ceilings in good condition', 8, 'Interior Condition', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (1, 'Floors and carpets clean and undamaged', 9, 'Interior Condition', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (1, 'Windows and doors functioning properly', 10, 'Interior Condition', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (1, 'Kitchen appliances operational', 11, 'Appliances', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (1, 'Bathroom fixtures functioning', 12, 'Plumbing', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (1, 'HVAC system operational', 13, 'Systems', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (1, 'Smoke detectors present and functional', 14, 'Safety', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (1, 'Carbon monoxide detectors present and functional', 15, 'Safety', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0);

-- Move-Out Checklist Template
INSERT INTO ChecklistTemplates (Name, Description, Category, IsSystemTemplate, OrganizationId, CreatedOn, CreatedBy, IsDeleted)
VALUES ('Move-Out Checklist', 'Standard move-out inspection checklist for departing tenants', 'Move-Out', 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0);

-- Move-Out Template Items (assumes template ID 2)
INSERT INTO ChecklistTemplateItems (ChecklistTemplateId, ItemText, ItemOrder, CategorySection, IsRequired, AllowsNotes, OrganizationId, CreatedOn, CreatedBy, IsDeleted)
VALUES
  (2, 'All keys returned', 1, 'Keys & Access', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (2, 'All remote controls returned', 2, 'Keys & Access', 0, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (2, 'Parking pass returned', 3, 'Keys & Access', 0, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (2, 'Final electric meter reading recorded', 4, 'Utilities', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (2, 'Final gas meter reading recorded', 5, 'Utilities', 0, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (2, 'Final water meter reading recorded', 6, 'Utilities', 0, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (2, 'Forwarding address obtained', 7, 'Tenant Information', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (2, 'Walls and ceilings condition assessed', 8, 'Interior Condition', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (2, 'Floors and carpets condition assessed', 9, 'Interior Condition', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (2, 'Windows and doors inspected', 10, 'Interior Condition', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (2, 'Kitchen cleaned and appliances checked', 11, 'Cleaning', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (2, 'Bathrooms cleaned and fixtures checked', 12, 'Cleaning', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (2, 'All trash and belongings removed', 13, 'Cleaning', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (2, 'Determine if professional cleaning needed', 14, 'Assessment', 0, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (2, 'Determine if repairs needed', 15, 'Assessment', 0, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (2, 'Calculate security deposit deductions', 16, 'Financial', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0);

-- Open House Checklist Template
INSERT INTO ChecklistTemplates (Name, Description, Category, IsSystemTemplate, OrganizationId, CreatedOn, CreatedBy, IsDeleted)
VALUES ('Open House Checklist', 'Preparation checklist for property open house events', 'Open House', 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0);

-- Open House Template Items (assumes template ID 3)
INSERT INTO ChecklistTemplateItems (ChecklistTemplateId, ItemText, ItemOrder, CategorySection, IsRequired, AllowsNotes, OrganizationId, CreatedOn, CreatedBy, IsDeleted)
VALUES
  (3, 'Property exterior cleaned and landscaping tidy', 1, 'Exterior', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (3, 'Walkways and entry clear and safe', 2, 'Exterior', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (3, 'Interior thoroughly cleaned', 3, 'Interior', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (3, 'All lights working and turned on', 4, 'Interior', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (3, 'Windows clean and curtains/blinds open', 5, 'Interior', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (3, 'Temperature comfortable (heating/cooling)', 6, 'Interior', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (3, 'Kitchen counters clear and clean', 7, 'Staging', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (3, 'Bathrooms staged with fresh towels', 8, 'Staging', 0, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (3, 'Personal items and clutter removed', 9, 'Staging', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (3, 'Property information sheets prepared', 10, 'Materials', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (3, 'Sign-in sheet ready', 11, 'Materials', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (3, 'Application forms available', 12, 'Materials', 1, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (3, 'Directional signs posted', 13, 'Marketing', 0, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (3, 'Refreshments prepared (if offering)', 14, 'Hospitality', 0, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0),
  (3, 'Music playing softly (if appropriate)', 15, 'Ambiance', 0, 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0);

-- Custom/Blank Checklist Template
INSERT INTO ChecklistTemplates (Name, Description, Category, IsSystemTemplate, OrganizationId, CreatedOn, CreatedBy, IsDeleted)
VALUES ('Custom Checklist', 'Start with a blank checklist and add your own items', 'Custom', 1, 'SYSTEM', datetime('now'), 'SYSTEM', 0);

-- Custom template starts with no items - users will add their own
