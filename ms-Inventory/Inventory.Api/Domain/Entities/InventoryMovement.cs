using System;

namespace Inventory.Api.Domain.Entities
{
    public class InventoryMovement
    {
        
        public Guid Id { get; private set; }
        public Guid InventoryItemId { get; private set; }
        public int QuantityChange { get; private set; }
        public string Reason { get; private set; } = string.Empty;
        public DateTime Timestamp { get; private set; }

        private InventoryMovement() { }
        
        public static InventoryMovement Create(
            Guid inventoryItemId,
            int quantityChange,
            string reason)
        {
            return new InventoryMovement
            {
                Id = Guid.NewGuid(),
                InventoryItemId = inventoryItemId,
                QuantityChange = quantityChange,
                Reason = reason ?? "Sin motivo especificado",
                Timestamp = DateTime.UtcNow
            };
        }
    }
}
