
ALTER TABLE Tenants
ADD CreatedBy TEXT NOT NULL DEFAULT '';
GO

UPDATE Tenants
SET CreatedBy = UserId;
GO