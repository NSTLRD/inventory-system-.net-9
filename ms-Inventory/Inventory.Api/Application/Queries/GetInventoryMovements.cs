using MediatR;
using Inventory.Api.Common.DTOs;
using System.Collections.Generic;

namespace Inventory.Api.Application.Queries
{
    public record GetInventoryMovements() : IRequest<IEnumerable<MovementDto>>;
}
