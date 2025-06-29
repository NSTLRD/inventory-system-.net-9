using System;

namespace Products.Api.Application.DTOs
{
    public class UpdateProductDto
    {
        public Guid Id { get; set; } // Añadir Id
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public string Sku { get; set; }
    }
}