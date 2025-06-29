using System;

namespace Inventory.Api.Common.DTOs
{
    public record InventoryStockDto(Guid ProductId, int Stock);
}
