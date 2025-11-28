
using Aquiis.SimpleStart.Components.PropertyManagement.Documents;
using Aquiis.SimpleStart.Components.PropertyManagement.Leases;
using Aquiis.SimpleStart.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Aquiis.SimpleStart.Services
{
    public class DocumentService
    {
        private readonly ApplicationDbContext _dbContext;

        public DocumentService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Document> UploadDocumentAsync(Document document)
        {
            _dbContext.Documents.Add(document);
            await _dbContext.SaveChangesAsync();
            return document;
        }

        public async Task DeleteDocumentAsync(int documentId)
        {
            var document = await _dbContext.Documents.FindAsync(documentId);
            if (document != null)
            {
                _dbContext.Documents.Remove(document);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<Document?> GetDocumentByIdAsync(int documentId)
        {
            return await _dbContext.Documents.FindAsync(documentId);
        }

        public async Task<byte[]> GenerateLeaseDocumentAsync(Lease lease)
        {
            // Implementation for generating lease document
            return await LeasePdfGenerator.GenerateLeasePdf(lease);
        }
    }
}