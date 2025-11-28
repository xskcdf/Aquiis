UPDATE AspNetUsers
SET OrganizationId = 'f3e86ee7-e70c-4fe4-805a-ac3044e55b01'
WHERE OrganizationId IS NULL;

UPDATE Properties
SET OrganizationId = 'f3e86ee7-e70c-4fe4-805a-ac3044e55b01'
WHERE OrganizationId IS NULL;

UPDATE Tenants
SET OrganizationId = 'f3e86ee7-e70c-4fe4-805a-ac3044e55b01'
WHERE OrganizationId IS NULL;

UPDATE Documents
SET OrganizationId = 'f3e86ee7-e70c-4fe4-805a-ac3044e55b01'
WHERE OrganizationId IS NULL;