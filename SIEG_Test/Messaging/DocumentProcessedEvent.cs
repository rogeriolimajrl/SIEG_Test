namespace SIEG_Test.Messaging
{
    public record DocumentProcessedEvent
    {
        public Guid DocumentId { get; init; }
        public string XmlHash { get; init; }
        public string Type { get; init; }
        public string EmitCnpj { get; init; }
        public DateTime IssueDate { get; init; }
    }
}
