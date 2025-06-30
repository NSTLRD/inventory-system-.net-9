using System;

namespace Inventory.Api.Application.DTOs
{
    public class InventoryDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Stock { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}