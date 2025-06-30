using System;
using System.Collections.Generic;
using MediatR;
using Inventory.Api.Application.DTOs;

namespace Inventory.Api.Application.Queries
{
    public class GetInventoryHistory : IRequest<IEnumerable<InventoryHistoryDto>>
    {
        public Guid ProductId { get; }

        public GetInventoryHistory(Guid productId)
        {
            ProductId = productId;
        }
    }
}
