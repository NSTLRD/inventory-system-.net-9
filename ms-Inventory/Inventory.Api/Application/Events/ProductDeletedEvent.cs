using System;
using Inventory.Api.Application.Events.Common;

namespace Inventory.Api.Application.Events
{
    public class ProductDeletedEvent : IntegrationEvent
    {
        public Guid ProductId { get; }

        public ProductDeletedEvent(Guid productId)
        {
            ProductId = productId;
        }

        public ProductDeletedEvent() { }
    }
}