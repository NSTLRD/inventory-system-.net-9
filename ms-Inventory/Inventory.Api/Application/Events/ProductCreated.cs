using System;
using Inventory.Api.Application.Events.Common; // <-- CORRECCIÓN: Añadir using

namespace Inventory.Api.Application.Events
{
    public class ProductCreatedIntegrationEvent : IntegrationEvent
    {
        public Guid ProductId { get; }
        public int InitialStock { get; }

        public ProductCreatedIntegrationEvent(Guid productId, int initialStock)
        {
            ProductId = productId;
            InitialStock = initialStock;
        }
    }
}
