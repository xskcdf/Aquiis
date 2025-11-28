-- Test data for late fee functionality
-- Creates sample invoices to test late fee application and payment reminders

-- Find an existing lease to use for test invoices
-- We'll use this in the INSERT statements below

-- Insert test invoices with different scenarios:

-- 1. Invoice 10 days overdue (should get late fee)
INSERT INTO Invoices (
    Id, InvoiceNumber, InvoiceDate, DueDate, Amount, AmountPaid, Status, Description, Notes,
    LeaseId, UserId, CreatedBy, CreatedOn, IsDeleted
)
SELECT 
    '10000000-0000-0000-0000-000000000001', -- Id
    'TEST-INV-001', -- InvoiceNumber
    date('now', '-40 days'), -- InvoiceDate (40 days ago)
    date('now', '-10 days'), -- DueDate (10 days overdue)
    1000.00, -- Amount
    0.00, -- AmountPaid (unpaid)
    'Pending', -- Status
    'Test invoice - 10 days overdue', -- Description
    'Created for late fee testing', -- Notes
    Id, -- LeaseId (from existing lease)
    UserId, -- UserId
    'admin@aquiis.com', -- CreatedBy
    datetime('now'), -- CreatedOn
    0 -- IsDeleted
FROM Leases 
WHERE NOT IsDeleted 
LIMIT 1;

-- 2. Invoice 4 days overdue (should get late fee - past 3 day grace period)
INSERT INTO Invoices (
    Id, InvoiceNumber, InvoiceDate, DueDate, Amount, AmountPaid, Status, Description, Notes,
    LeaseId, UserId, CreatedBy, CreatedOn, IsDeleted
)
SELECT 
    '10000000-0000-0000-0000-000000000002',
    'TEST-INV-002',
    date('now', '-34 days'),
    date('now', '-4 days'), -- 4 days overdue
    500.00,
    0.00,
    'Pending',
    'Test invoice - 4 days overdue',
    'Created for late fee testing',
    Id,
    UserId,
    'admin@aquiis.com',
    datetime('now'),
    0
FROM Leases 
WHERE NOT IsDeleted 
LIMIT 1;

-- 3. Invoice 2 days overdue (should NOT get late fee - within grace period)
INSERT INTO Invoices (
    Id, InvoiceNumber, InvoiceDate, DueDate, Amount, AmountPaid, Status, Description, Notes,
    LeaseId, UserId, CreatedBy, CreatedOn, IsDeleted
)
SELECT 
    '10000000-0000-0000-0000-000000000003',
    'TEST-INV-003',
    date('now', '-32 days'),
    date('now', '-2 days'), -- 2 days overdue (within grace period)
    750.00,
    0.00,
    'Pending',
    'Test invoice - 2 days overdue (within grace)',
    'Created for late fee testing',
    Id,
    UserId,
    'admin@aquiis.com',
    datetime('now'),
    0
FROM Leases 
WHERE NOT IsDeleted 
LIMIT 1;

-- 4. Invoice due in 3 days (should get payment reminder)
INSERT INTO Invoices (
    Id, InvoiceNumber, InvoiceDate, DueDate, Amount, AmountPaid, Status, Description, Notes,
    LeaseId, UserId, CreatedBy, CreatedOn, IsDeleted
)
SELECT 
    '10000000-0000-0000-0000-000000000004',
    'TEST-INV-004',
    date('now', '-27 days'),
    date('now', '+3 days'), -- Due in 3 days
    800.00,
    0.00,
    'Pending',
    'Test invoice - due in 3 days',
    'Created for payment reminder testing',
    Id,
    UserId,
    'admin@aquiis.com',
    datetime('now'),
    0
FROM Leases 
WHERE NOT IsDeleted 
LIMIT 1;

-- 5. Large invoice overdue (tests late fee cap of $50)
INSERT INTO Invoices (
    Id, InvoiceNumber, InvoiceDate, DueDate, Amount, AmountPaid, Status, Description, Notes,
    LeaseId, UserId, CreatedBy, CreatedOn, IsDeleted
)
SELECT 
    '10000000-0000-0000-0000-000000000005',
    'TEST-INV-005',
    date('now', '-45 days'),
    date('now', '-15 days'), -- 15 days overdue
    2000.00, -- 5% would be $100, but capped at $50
    0.00,
    'Pending',
    'Test invoice - large amount to test fee cap',
    'Created for late fee cap testing (5% = $100, but capped at $50)',
    Id,
    UserId,
    'admin@aquiis.com',
    datetime('now'),
    0
FROM Leases 
WHERE NOT IsDeleted 
LIMIT 1;

-- Verify test data inserted
SELECT 
    InvoiceNumber,
    InvoiceDate,
    DueDate,
    Amount,
    Status,
    Description,
    julianday('now') - julianday(DueDate) as DaysOverdue
FROM Invoices
WHERE InvoiceNumber LIKE 'TEST-INV-%'
ORDER BY DueDate;
