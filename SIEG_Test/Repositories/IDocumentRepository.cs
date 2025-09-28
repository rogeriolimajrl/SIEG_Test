using SIEG_Test.Models;

namespace SIEG_Test.Repositories
{
    public interface IDocumentRepository
    {
        Task AddDocument(Document doc);
        Task<Document?> GetByHashDocument(string hash);
        Task<Document?> GetByIdDocument(Guid id);
        Task<List<Document>> ListDocument(int page, int pageSize, string? emitCnpj, string? uf, DateTime? from, DateTime? to);
        Task<bool> UpdateDocument(Guid id, decimal? totalValue, string? destCnpj);
        Task<bool> DeleteDocument(Guid id);
    }
}
