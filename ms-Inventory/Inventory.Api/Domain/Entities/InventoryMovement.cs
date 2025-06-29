using System;

namespace Inventory.Api.Domain.Entities
{
    public class InventoryMovement : Entity
    {
        // Cambiado de InventoryId a InventoryItemId para mantener consistencia
        public Guid InventoryItemId { get; private set; }
        
        // Cambiado de QuantityChanged a QuantityChange para mantener consistencia
        public int QuantityChange { get; private set; }
        
        public string Reason { get; private set; }
        public DateTime Timestamp { get; private set; }

        public InventoryMovement(Guid inventoryItemId, int quantityChange, string reason, DateTime timestamp)
        {
            InventoryItemId = inventoryItemId;
            QuantityChange = quantityChange;
            Reason = reason ?? "No reason provided"; // Evitar nulls
            Timestamp = timestamp;
        }

        // Constructor sin parámetros para EF Core
        private InventoryMovement() { }
    }
}
