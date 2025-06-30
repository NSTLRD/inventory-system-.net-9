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
        
        public decimal? ConvertedPrice { get; set; }
        public string? Currency { get; set; }
        public decimal? ExchangeRate { get; set; }
        
        public IEnumerable<(decimal OldPrice, decimal NewPrice, DateTime At)> PriceHistory { get; set; }

      
        public ProductDto(Guid id, string name, string description, string category, decimal price, string sku, 
            IEnumerable<(decimal OldPrice, decimal NewPrice, DateTime At)> priceHistory,
            decimal? convertedPrice = null, string? currency = null, decimal? exchangeRate = null)
        {
            Id = id;
            Name = name;
            Description = description;
            Category = category;
            Price = price;
            Sku = sku;
            PriceHistory = priceHistory;
            ConvertedPrice = convertedPrice;
            Currency = currency;
            ExchangeRate = exchangeRate;
        }

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