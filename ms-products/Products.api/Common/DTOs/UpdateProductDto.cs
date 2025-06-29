namespace Products.Api.Common.DTOs
{
    public class UpdateProductDto
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Category { get; set; } = null!;
        public decimal Price { get; set; }
        public string Sku { get; set; } = null!;
    }
}
