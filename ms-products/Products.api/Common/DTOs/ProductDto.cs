using System;
using System.Collections.Generic;

namespace Products.Api.Common.DTOs
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public string Sku { get; set; }
        public IEnumerable<(decimal OldPrice, decimal NewPrice, DateTime At)> PriceHistory { get; set; }

        // Constructor con todos los parámetros
        public ProductDto(Guid id, string name, string description, string category, decimal price, string sku, 
            IEnumerable<(decimal OldPrice, decimal NewPrice, DateTime At)> priceHistory)
        {
            Id = id;
            Name = name;
            Description = description;
            Category = category;
            Price = price;
            Sku = sku;
            PriceHistory = priceHistory;
        }

        // Constructor sin parámetros para serialización/deserialización
        public ProductDto()
        {
            Name = string.Empty;
            Description = string.Empty;
            Category = string.Empty;
            Sku = string.Empty;
            PriceHistory = new List<(decimal OldPrice, decimal NewPrice, DateTime At)>();
        }
    }
}