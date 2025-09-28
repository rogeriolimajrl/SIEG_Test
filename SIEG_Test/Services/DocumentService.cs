using System.Security.Cryptography;
using System.Text;
using MassTransit;
using MongoDB.Driver;
using SIEG_Test.DTOs;
using SIEG_Test.Models;
using SIEG_Test.Repositories;

namespace SIEG_Test.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IMongoCollection<Document> _collection;
        private readonly IPublishEndpoint _publishEndpoint;

        public DocumentService(IMongoClient mongoClient, IPublishEndpoint publishEndpoint)
        {
            var database = mongoClient.GetDatabase("siegDb");
            _collection = database.GetCollection<Document>("Documents");
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Guid> ProcessXmlDocument(string xml, DocumentType type)
        {
            var hash = ComputeHash(xml);
            var existing = await _collection.Find(d => d.XmlHash == hash).FirstOrDefaultAsync();
            if (existing != null) return existing.Id;

            var doc = new Document
            {
                Id = Guid.NewGuid(),
                XmlRaw = xml,
                XmlHash = hash,
                Type = type,
                CreatedAt = DateTime.UtcNow
            };

            await _collection.InsertOneAsync(doc);

            // Publica evento no RabbitMQ
            await _publishEndpoint.Publish(new DocumentProcessedEvent
            {
                DocumentId = doc.Id,
                Type = doc.Type,
                CreatedAt = doc.CreatedAt
            });

            return doc.Id;
        }

        public async Task<Document?> GetByIdDocument(Guid id)
            => await _collection.Find(d => d.Id == id).FirstOrDefaultAsync();

        public async Task<List<Document>> ListDocument(DocumentQuery query)
        {
            var filter = Builders<Document>.Filter.Empty;

            if (!string.IsNullOrEmpty(query.Cnpj))
                filter &= Builders<Document>.Filter.Eq(d => d.DestCnpj, query.Cnpj);

            if (query.FromDate.HasValue)
                filter &= Builders<Document>.Filter.Gte(d => d.CreatedAt, query.FromDate.Value);

            if (query.ToDate.HasValue)
                filter &= Builders<Document>.Filter.Lte(d => d.CreatedAt, query.ToDate.Value);

            return await _collection.Find(filter)
                                    .Skip((query.Page - 1) * query.PageSize)
                                    .Limit(query.PageSize)
                                    .ToListAsync();
        }

        public async Task<bool> UpdateDocument(Guid id, UpdateDocumentDto dto)
        {
            var update = Builders<Document>.Update
                .Set(d => d.TotalValue, dto.TotalValue)
                .Set(d => d.DestCnpj, dto.DestCnpj);

            var result = await _collection.UpdateOneAsync(d => d.Id == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteDocument(Guid id)
        {
            var result = await _collection.DeleteOneAsync(d => d.Id == id);
            return result.DeletedCount > 0;
        }

        private string ComputeHash(string xml)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(xml);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }

    // Evento para RabbitMQ
    public record DocumentProcessedEvent
    {
        public Guid DocumentId { get; init; }
        public DocumentType Type { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
