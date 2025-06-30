using System;
using System.Collections.Generic;

namespace Inventory.Api.Domain.Entities
{
    public class Inventory : Entity
    {
        public Guid ProductId { get; private set; }
        public int Stock { get; set; }
        public List<InventoryMovement> Movements { get; private set; } = new List<InventoryMovement>();

        public Inventory(Guid productId, int initialStock)
        {
            ProductId = productId;
            Stock = initialStock;
        }

        private Inventory() { }
    }
}