using MongoDB.Driver;
using SIEG_Test.Models;

namespace SIEG_Test.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly IMongoCollection<Document> collection;

        public DocumentRepository(IMongoDatabase database)
        {
            collection = database.GetCollection<Document>("documents");
            collection.Indexes.CreateOne(
                new CreateIndexModel<Document>(
                    Builders<Document>.IndexKeys.Ascending(d => d.XmlHash),
                    new CreateIndexOptions { Unique = true }
                )
            );
        }

        public async Task AddDocument(Document doc) => await collection.InsertOneAsync(doc);

        public async Task<Document?> GetByHashDocument(string hash) =>
            await collection.Find(d => d.XmlHash == hash).FirstOrDefaultAsync();

        public async Task<Document?> GetByIdDocument(Guid id) =>
            await collection.Find(d => d.Id == id).FirstOrDefaultAsync();

        public async Task<List<Document>> ListDocument(int page, int pageSize, string? emitCnpj, string? uf, DateTime? from, DateTime? to)
        {
            var filter = Builders<Document>.Filter.Empty;
            if (!string.IsNullOrEmpty(emitCnpj)) filter &= Builders<Document>.Filter.Eq(d => d.EmitCnpj, emitCnpj);
            if (!string.IsNullOrEmpty(uf)) filter &= Builders<Document>.Filter.Eq(d => d.Uf, uf);
            if (from.HasValue) filter &= Builders<Document>.Filter.Gte(d => d.IssueDate, from.Value);
            if (to.HasValue) filter &= Builders<Document>.Filter.Lte(d => d.IssueDate, to.Value);

            return await collection.Find(filter)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        public async Task<bool> UpdateDocument(Guid id, decimal? totalValue, string? destCnpj)
        {
            var update = Builders<Document>.Update;
            var updates = new List<UpdateDefinition<Document>>();
            if (totalValue.HasValue) updates.Add(update.Set(d => d.TotalValue, totalValue.Value));
            if (!string.IsNullOrEmpty(destCnpj)) updates.Add(update.Set(d => d.DestCnpj, destCnpj));
            if (!updates.Any()) return false;

            var result = await collection.UpdateOneAsync(d => d.Id == id, update.Combine(updates));
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteDocument(Guid id)
        {
            var result = await collection.DeleteOneAsync(d => d.Id == id);
            return result.DeletedCount > 0;
        }
    }
}
