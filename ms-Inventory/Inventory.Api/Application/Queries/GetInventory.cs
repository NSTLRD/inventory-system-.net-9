using MediatR;
using Inventory.Api.Common.DTOs;
using System.Collections.Generic;

namespace Inventory.Api.Application.Queries
{
    public record GetInventory() : IRequest<IEnumerable<InventoryDto>>;
}
