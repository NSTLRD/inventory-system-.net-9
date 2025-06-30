
using MediatR;
using System;

namespace Inventory.Api.Application.Commands
{

    public record AdjustInventory(
        Guid   ProductId,
        int    Quantity,
        string Reason
    ) : IRequest<Unit>;
}
