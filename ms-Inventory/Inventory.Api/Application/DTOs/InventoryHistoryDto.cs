using System;
using System.Collections.Generic;

namespace Inventory.Api.Application.DTOs
{
    public class InventoryHistoryDto
    {
        public Guid Id { get; }
        public Guid InventoryItemId { get; }
        public int QuantityChange { get; }
        public DateTime Timestamp { get; }

        public InventoryHistoryDto(Guid id, Guid inventoryItemId, int quantityChange, DateTime timestamp)
        {
            Id = id;
            InventoryItemId = inventoryItemId;
            QuantityChange = quantityChange;
            Timestamp = timestamp;
        }
    }
}