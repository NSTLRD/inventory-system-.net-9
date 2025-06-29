using System;
using MediatR;
using Inventory.Api.Common.DTOs;
using Inventory.Api.Application.DTOs;

namespace Inventory.Api.Application.Queries
{
    /// <summary>
    /// Solicita el stock actual de un producto por su ID.
    /// </summary>
    public record GetInventoryStock(Guid ProductId)
        : IRequest<InventoryStockDto>;
}
