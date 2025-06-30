using System;

namespace Inventory.Api.Application.DTOs
{
    public class MovementDto
    {
        public Guid Id { get; set; }
        public Guid InventoryItemId { get; set; }
        public int QuantityChange { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }

        public MovementDto(Guid id, Guid inventoryItemId, int quantityChange, DateTime timestamp)
        {
            Id = id;
            InventoryItemId = inventoryItemId;
            QuantityChange = quantityChange;
            Timestamp = timestamp;
        }
        
        public MovementDto() { }
    }
}