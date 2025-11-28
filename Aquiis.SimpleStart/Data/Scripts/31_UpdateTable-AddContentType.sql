
ALTER TABLE Documents
ADD ContentType TEXT;
GO


UPDATE Documents
SET ContentType = 'application/pdf'
WHERE ContentType IS NULL;

GO