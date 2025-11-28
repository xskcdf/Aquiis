-- Test data for lease renewal tracking functionality
-- Creates sample leases expiring at different intervals

-- Find existing property and tenant to use for test leases
-- We'll use these in the INSERT statements below

-- 1. Lease expiring in 25 days (should trigger 30-day reminder)
INSERT INTO Leases (
    Id, PropertyId, TenantId, StartDate, EndDate, MonthlyRent, SecurityDeposit, 
    Status, Terms, Notes, UserId, CreatedBy, CreatedOn, IsDeleted
)
SELECT 
    (SELECT COALESCE(MAX(Id), 0) + 1 FROM Leases), -- Auto-increment ID
    Id, -- PropertyId from existing property
    (SELECT Id FROM Tenants WHERE NOT IsDeleted LIMIT 1), -- TenantId
    date('now', '-11 months'), -- StartDate (11 months ago)
    date('now', '+25 days'), -- EndDate (expires in 25 days)
    1500.00, -- MonthlyRent
    1500.00, -- SecurityDeposit
    'Active', -- Status
    'Standard 12-month lease agreement', -- Terms
    'Test lease - expires in 25 days', -- Notes
    UserId,
    'admin@aquiis.com',
    datetime('now'),
    0
FROM Properties 
WHERE NOT IsDeleted AND IsAvailable = 0
LIMIT 1;

-- 2. Lease expiring in 55 days (should trigger 60-day reminder)
INSERT INTO Leases (
    Id, PropertyId, TenantId, StartDate, EndDate, MonthlyRent, SecurityDeposit, 
    Status, Terms, Notes, UserId, CreatedBy, CreatedOn, IsDeleted
)
SELECT 
    (SELECT COALESCE(MAX(Id), 0) + 1 FROM Leases),
    Id,
    (SELECT Id FROM Tenants WHERE NOT IsDeleted ORDER BY Id DESC LIMIT 1),
    date('now', '-10 months', '-5 days'),
    date('now', '+55 days'), -- Expires in 55 days
    1800.00,
    1800.00,
    'Active',
    'Standard 12-month lease agreement',
    'Test lease - expires in 55 days',
    UserId,
    'admin@aquiis.com',
    datetime('now'),
    0
FROM Properties 
WHERE NOT IsDeleted AND IsAvailable = 0
ORDER BY Id DESC
LIMIT 1;

-- 3. Lease expiring in 85 days (should trigger 90-day initial notification)
INSERT INTO Leases (
    Id, PropertyId, TenantId, StartDate, EndDate, MonthlyRent, SecurityDeposit, 
    Status, Terms, Notes, UserId, CreatedBy, CreatedOn, IsDeleted
)
SELECT 
    (SELECT COALESCE(MAX(Id), 0) + 1 FROM Leases),
    Id,
    (SELECT Id FROM Tenants WHERE NOT IsDeleted ORDER BY Id LIMIT 1 OFFSET 1),
    date('now', '-9 months', '-5 days'),
    date('now', '+85 days'), -- Expires in 85 days
    2000.00,
    2000.00,
    'Active',
    'Standard 12-month lease agreement',
    'Test lease - expires in 85 days',
    UserId,
    'admin@aquiis.com',
    datetime('now'),
    0
FROM Properties 
WHERE NOT IsDeleted
ORDER BY Id
LIMIT 1 OFFSET 2;

-- 4. Lease expiring in 15 days (urgent - past all reminder periods)
INSERT INTO Leases (
    Id, PropertyId, TenantId, StartDate, EndDate, MonthlyRent, SecurityDeposit, 
    Status, Terms, Notes, UserId, CreatedBy, CreatedOn, IsDeleted
)
SELECT 
    (SELECT COALESCE(MAX(Id), 0) + 1 FROM Leases),
    Id,
    (SELECT Id FROM Tenants WHERE NOT IsDeleted ORDER BY Id LIMIT 1 OFFSET 2),
    date('now', '-11 months', '-15 days'),
    date('now', '+15 days'), -- Expires in 15 days
    1600.00,
    1600.00,
    'Active',
    'Standard 12-month lease agreement',
    'Test lease - expires in 15 days (URGENT)',
    UserId,
    'admin@aquiis.com',
    datetime('now'),
    0
FROM Properties 
WHERE NOT IsDeleted
ORDER BY Id
LIMIT 1 OFFSET 3;

-- 5. Lease that already has a renewal offer sent
INSERT INTO Leases (
    Id, PropertyId, TenantId, StartDate, EndDate, MonthlyRent, SecurityDeposit, 
    Status, Terms, Notes, UserId, CreatedBy, CreatedOn, IsDeleted,
    RenewalStatus, RenewalNotificationSent, RenewalNotificationSentOn, 
    ProposedRenewalRent, RenewalOfferedOn, RenewalNotes
)
SELECT 
    (SELECT COALESCE(MAX(Id), 0) + 1 FROM Leases),
    Id,
    (SELECT Id FROM Tenants WHERE NOT IsDeleted ORDER BY Id LIMIT 1 OFFSET 3),
    date('now', '-10 months', '-20 days'),
    date('now', '+40 days'), -- Expires in 40 days
    1700.00,
    1700.00,
    'Active',
    'Standard 12-month lease agreement',
    'Test lease - renewal offer already sent',
    UserId,
    'admin@aquiis.com',
    datetime('now'),
    0,
    'Offered', -- RenewalStatus
    1, -- RenewalNotificationSent
    datetime('now', '-10 days'), -- RenewalNotificationSentOn
    1785.00, -- ProposedRenewalRent (5% increase)
    datetime('now', '-5 days'), -- RenewalOfferedOn
    'Proposed 5% rent increase for renewal. Property improvements completed.' -- RenewalNotes
FROM Properties 
WHERE NOT IsDeleted
ORDER BY Id
LIMIT 1 OFFSET 4;

-- Verify test data inserted
SELECT 
    l.Id,
    p.Address as Property,
    t.FirstName || ' ' || t.LastName as Tenant,
    l.StartDate,
    l.EndDate,
    CAST((julianday(l.EndDate) - julianday('now')) AS INTEGER) as DaysUntilExpiration,
    l.MonthlyRent,
    l.ProposedRenewalRent,
    l.RenewalStatus,
    l.Status,
    l.Notes
FROM Leases l
INNER JOIN Properties p ON l.PropertyId = p.Id
INNER JOIN Tenants t ON l.TenantId = t.Id
WHERE l.Notes LIKE 'Test lease%'
ORDER BY l.EndDate;
