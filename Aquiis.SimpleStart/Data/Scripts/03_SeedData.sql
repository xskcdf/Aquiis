-- Aquiis Database Seed Data Script
-- This script inserts initial data for testing and development

-- Insert default roles
INSERT OR IGNORE INTO "AspNetRoles" ("Id", "Name", "NormalizedName", "ConcurrencyStamp")
VALUES 
    ('1', 'Administrator', 'ADMINISTRATOR', lower(hex(randomblob(16)))),
    ('2', 'PropertyManager', 'PROPERTYMANAGER', lower(hex(randomblob(16)))),
    ('3', 'Tenant', 'TENANT', lower(hex(randomblob(16)))),
    ('4', 'Maintenance', 'MAINTENANCE', lower(hex(randomblob(16))));

-- Sample property types and data (for development/testing)
-- Note: In production, you would typically not seed with sample properties
-- This is included for development and testing purposes only

-- Uncomment the following lines for development seed data:
/*
INSERT OR IGNORE INTO "Properties" (
    "UserId", "Address", "City", "State", "ZipCode", "PropertyType", 
    "MonthlyRent", "Bedrooms", "Bathrooms", "SquareFeet", "Description", 
    "IsAvailable", "CreatedDate"
) VALUES 
    ('sample-user-id', '123 Main Street', 'Anytown', 'CA', '12345', 'House', 
     2500.00, 3, 2.0, 1500, 'Beautiful 3-bedroom house with garden', 1, datetime('now')),
    ('sample-user-id', '456 Oak Avenue', 'Somewhere', 'TX', '67890', 'Apartment', 
     1800.00, 2, 1.5, 900, 'Modern 2-bedroom apartment downtown', 1, datetime('now')),
    ('sample-user-id', '789 Pine Boulevard', 'Elsewhere', 'FL', '11111', 'Condo', 
     2200.00, 2, 2.0, 1100, 'Luxury condo with ocean view', 0, datetime('now'));
*/