using System;
using Inventory.Api.Application.Events.Common;

namespace Inventory.Api.Application.Events
{
    public class ProductDeletedIntegrationEvent : IntegrationEvent
    {
        public Guid ProductId { get; set; }
    }
}