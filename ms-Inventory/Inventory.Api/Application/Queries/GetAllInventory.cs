using System.Collections.Generic;
using Inventory.Api.Domain.Entities;
using MediatR;

namespace Inventory.Api.Application.Queries
{
    public class GetAllInventory : IRequest<IEnumerable<InventoryItem>>
    {
        // Esta es una consulta simple sin parámetros
    }
}