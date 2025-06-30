using System;
using MediatR;
using Inventory.Api.Application.DTOs;

namespace Inventory.Api.Application.Queries
{
    public class GetInventory : IRequest<InventoryDto>
    {
        public Guid Id { get; }

        public GetInventory(Guid id)
        {
            Id = id;
        }
    }
}
