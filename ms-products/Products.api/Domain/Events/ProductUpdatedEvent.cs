using System;
using Products.Api.Domain.Events.Common;

namespace Products.Api.Domain.Events
{
    public class ProductUpdatedEvent : BaseEvent
    {
        public Guid ProductId { get; }
        public string Name { get; }
        public string Description { get; }
        public string Category { get; }
        public decimal Price { get; }
        public string SKU { get; }

        public ProductUpdatedEvent(Guid productId, string name, string description, string category, decimal price, string sku)
        {
            ProductId = productId;
            Name = name;
            Description = description;
            Category = category;
            Price = price;
            SKU = sku;
        }
    }
}
