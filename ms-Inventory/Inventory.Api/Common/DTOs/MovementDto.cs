using System;

namespace Inventory.Api.Common.DTOs
{
    public record MovementDto(
        Guid MovementId,
        Guid ProductId,
        int Delta,
        DateTime When
    );
}
