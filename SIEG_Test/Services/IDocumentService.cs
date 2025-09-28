using SIEG_Test.DTOs;
using SIEG_Test.Models;

namespace SIEG_Test.Services
{
    public interface IDocumentService
    {
        Task<Guid> ProcessXmlDocument(string xml, DocumentType type);
        Task<Document?> GetByIdDocument(Guid id);
        Task<List<Document>> ListDocument(DocumentQuery query);
        Task<bool> UpdateDocument(Guid id, UpdateDocumentDto dto);
        Task<bool> DeleteDocument(Guid id);
    }
}
