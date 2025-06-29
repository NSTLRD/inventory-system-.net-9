using System;

namespace Inventory.Api.Common.DTOs
{
    public record InventoryDto(Guid ProductId, int Quantity);
}
