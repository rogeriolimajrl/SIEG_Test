namespace SIEG_Test.DTOs
{
    public class DocumentQuery
    {
        public string? Cnpj { get; set; }
        public string? Uf { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
