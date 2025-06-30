using System;
using System.Collections.Generic;

namespace Inventory.Api.Domain.Entities
{
    public class InventoryItem
    {
        public Guid Id { get; private set; }
        public Guid ProductId { get; private set; }
        public int Stock { get; private set; }
        public DateTime LastUpdated { get; private set; }
        
        public byte[]? RowVersion { get; set; }

        public List<InventoryMovement> Movements { get; private set; } = new List<InventoryMovement>();

        private InventoryItem() { }

        public InventoryItem(Guid productId, int initialStock = 0)
        {
            Id = Guid.NewGuid();
            ProductId = productId;
            Stock = initialStock;
            LastUpdated = DateTime.UtcNow;
            
            if (initialStock != 0)
            {
                RecordMovement(initialStock, "Stock inicial");
            }
        }

        public void SetStock(int newStock, string reason)
        {
            var change = newStock - Stock;
            if (change != 0)
            {
                Stock = newStock;
                LastUpdated = DateTime.UtcNow;
                RecordMovement(change, reason);
            }
        }

        public void UpdateStockDirectly(int newStock)
        {
            Stock = newStock;
            LastUpdated = DateTime.UtcNow;
        }

        private void RecordMovement(int quantityChange, string reason)
        {
            var movement = InventoryMovement.Create(
                inventoryItemId: Id,
                quantityChange: quantityChange,
                reason: reason
            );
            
            Movements.Add(movement);
        }
    }
}
