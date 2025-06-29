// Inventory.Api/Application/Events/ProductUpdatedEvent.cs
using System;
using Inventory.Api.Application.Events.Common;

namespace Inventory.Api.Application.Events
{
    public class ProductUpdatedEvent : IntegrationEvent
    {
        public Guid   ProductId   { get; }
        public string Name        { get; }
        public string Description { get; }
        public string Category    { get; }
        public decimal Price      { get; }
        public string SKU         { get; }

        public ProductUpdatedEvent(
            Guid productId,
            string name,
            string description,
            string category,
            decimal price,
            string sku)
        {
            ProductId   = productId;
            Name        = name;
            Description = description;
            Category    = category;
            Price       = price;
            SKU         = sku;
        }
    }
}
