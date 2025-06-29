using System;
using MediatR;

namespace Inventory.Api.Domain.Events
{
    public record ProductCreatedEvent(
        Guid Id,
        string Name,
        string Description,
        string Category,
        decimal Price,
        string Sku
    ) : INotification;
}
