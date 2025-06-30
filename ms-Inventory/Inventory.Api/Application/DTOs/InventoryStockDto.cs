using System;

namespace Inventory.Api.Application.DTOs
{
    public class InventoryStockDto
    {
        public Guid ProductId { get; set; }
        public int Stock { get; set; }

        public InventoryStockDto(Guid productId, int stock)
        {
            ProductId = productId;
            Stock = stock;
        }
    }
}