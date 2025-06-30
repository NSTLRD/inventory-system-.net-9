using System;
using System.Collections.Generic;
using MediatR;
using Inventory.Api.Application.DTOs;

namespace Inventory.Api.Application.Queries
{
    public class GetInventoryMovements : IRequest<IEnumerable<MovementDto>>
    {
        public Guid InventoryId { get; }

        public GetInventoryMovements(Guid inventoryId)
        {
            InventoryId = inventoryId;
        }
    }
}
