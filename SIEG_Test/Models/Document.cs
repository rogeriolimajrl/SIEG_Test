using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SIEG_Test.Models
{
    public class Document
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        public DocumentType Type { get; set; }
        public string XmlRaw { get; set; }
        public string XmlHash { get; set; } // Para evitar duplicidade de documentos
        public string? EmitCnpj { get; set; }
        public string? DestCnpj { get; set; }
        public string? Uf { get; set; }
        public DateTime IssueDate { get; set; }
        public decimal? TotalValue { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
