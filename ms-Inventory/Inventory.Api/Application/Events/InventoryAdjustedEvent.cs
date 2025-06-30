using System;
using MediatR;

namespace Inventory.Api.Application.Events
{
    public class InventoryAdjustedEvent : INotification
    {
        public Guid InventoryId { get; }
        public Guid ProductId { get; }
        public int NewStock { get; }
        public string Reason { get; }
        public DateTime Timestamp { get; }

        public InventoryAdjustedEvent(
            Guid inventoryId,
            Guid productId,
            int newStock,
            string reason,
            DateTime timestamp)
        {
            InventoryId = inventoryId;
            ProductId = productId;
            NewStock = newStock;
            Reason = reason;
            Timestamp = timestamp;
        }
    }
}