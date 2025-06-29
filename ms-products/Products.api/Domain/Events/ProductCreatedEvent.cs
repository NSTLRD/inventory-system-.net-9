// Products.Api/Domain/Events/ProductCreatedEvent.cs
using System;
using Products.Api.Infrastructure.Messaging;

namespace Products.Api.Domain.Events
{
    public class ProductCreatedEvent : IntegrationEvent
    {
        public Guid    ProductId    { get; }
        public string  Name         { get; }
        public string  Description  { get; }
        public string  Category     { get; }
        public decimal Price        { get; }
        public string  SKU          { get; }
        public int     InitialStock { get; }

        public ProductCreatedEvent(
            Guid    productId,
            string  name,
            string  description,
            string  category,
            decimal price,
            string  sku,
            int     initialStock)
        {
            ProductId    = productId;
            Name         = name;
            Description  = description;
            Category     = category;
            Price        = price;
            SKU          = sku;
            InitialStock = initialStock;
        }
    }
}
