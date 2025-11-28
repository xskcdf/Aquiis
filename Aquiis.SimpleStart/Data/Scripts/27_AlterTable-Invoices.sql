
ALTER TABLE Invoices
ADD PaidOn TEXT NULL;

ALTER TABLE Invoices
ADD AmountPaid TEXT NULL;

UPDATE Invoices
SET PaidOn = PaidDate,
    AmountPaid = PaidAmount;

ALTER TABLE Invoices
DROP COLUMN PaidDate;

ALTER TABLE Invoices
DROP COLUMN PaidAmount;