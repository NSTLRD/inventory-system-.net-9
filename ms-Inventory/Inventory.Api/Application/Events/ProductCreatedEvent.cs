using System;
using Inventory.Api.Application.Events.Common;

namespace Inventory.Api.Application.Events
{
    public class ProductCreatedEvent : IntegrationEvent
    {
        public Guid ProductId { get; }
        public int InitialStock { get; }

        public ProductCreatedEvent(Guid productId, int initialStock)
        {
            ProductId = productId;
            InitialStock = initialStock;
        }
    }
}