using System;
using MediatR;
using Inventory.Api.Application.DTOs;

namespace Inventory.Api.Application.Queries
{
    public record GetInventoryHistory(Guid ProductId)
        : IRequest<IEnumerable<InventoryHistoryDto>>;
}
