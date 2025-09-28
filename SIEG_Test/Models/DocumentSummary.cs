using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SIEG_Test.Models
{
    public class DocumentSummary
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        public Guid DocumentId { get; set; }
        public string SummaryText { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}