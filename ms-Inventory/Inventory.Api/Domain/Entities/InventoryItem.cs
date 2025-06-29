using System;
using System.Collections.Generic;

namespace Inventory.Api.Domain.Entities
{
    public class InventoryItem : Entity
    {
        public Guid ProductId { get; private set; }
        public int Stock { get; private set; }
        public List<InventoryMovement> Movements { get; private set; } = new List<InventoryMovement>();

        // Propiedad para compatibilidad
        public int Quantity => Stock;

        public InventoryItem(Guid productId, int initialStock)
        {
            ProductId = productId;
            Stock = initialStock;
            
            // Registrar el movimiento inicial
            RegisterMovement(initialStock, "Initial stock");
        }

        // Constructor para EF Core
        private InventoryItem() 
        {
            Movements = new List<InventoryMovement>();
        }
        
        // Método para establecer el stock directamente (para AdjustInventoryHandler)
        public void SetStock(int newStock, string reason)
        {
            int delta = newStock - Stock;
            Stock = newStock;
            RegisterMovement(delta, reason ?? "Stock adjustment");
        }
        
        // Método para ajustar el inventario
        public void AdjustStock(int quantity, string reason)
        {
            int delta = quantity - Stock;
            Stock = quantity;
            RegisterMovement(delta, reason);
        }
        
        // Método auxiliar para registrar movimientos
        private void RegisterMovement(int quantityChange, string reason)
        {
            var movement = new InventoryMovement(
                Id,
                quantityChange,
                reason,
                DateTime.UtcNow
            );
            
            Movements.Add(movement);
        }
    }
}
