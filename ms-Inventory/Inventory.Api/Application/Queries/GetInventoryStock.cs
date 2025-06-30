using System;
using MediatR;
using Inventory.Api.Application.DTOs;

namespace Inventory.Api.Application.Queries
{
    /// <summary>
    /// Solicita el stock actual de un producto por su ID.
    /// </summary>
    public class GetInventoryStock : IRequest<InventoryStockDto>
    {
        public Guid ProductId { get; }

        public GetInventoryStock(Guid productId)
        {
            ProductId = productId;
        }
    }
}
