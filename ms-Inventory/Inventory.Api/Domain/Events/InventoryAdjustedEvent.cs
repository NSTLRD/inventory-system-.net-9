using System;
using MediatR;

namespace Inventory.Api.Domain.Events
{
    public record InventoryAdjustedEvent(Guid ProductId, int Delta, DateTime When)
        : INotification;
}
