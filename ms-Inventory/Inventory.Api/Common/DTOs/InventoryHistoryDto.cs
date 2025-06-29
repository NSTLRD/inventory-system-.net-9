using System;

namespace Inventory.Api.Application.DTOs
{
    public record InventoryHistoryDto(
        Guid MovementId,
        Guid ProductId,
        int Delta,
        DateTime When
    );
}
