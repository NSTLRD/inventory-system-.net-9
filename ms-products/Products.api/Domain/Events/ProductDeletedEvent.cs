using System;
using Products.Api.Domain.Events.Common;

namespace Products.Api.Domain.Events
{
    public class ProductDeletedEvent : BaseEvent 
    {
        public Guid ProductId { get; }

        public ProductDeletedEvent(Guid productId)
        {
            ProductId = productId;
        }
    }
}
