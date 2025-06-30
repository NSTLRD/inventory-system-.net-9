using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Interfaces
{
    // DTOs
    public interface IInventoryStockDto
    {
        Guid ProductId { get; }
        int Stock { get; }
    }

    public interface IInventoryHistoryDto
    {
        Guid Id { get; }
        Guid InventoryItemId { get; }
        int QuantityChanged { get; }
        DateTime Timestamp { get; }
    }

    public interface IAdjustInventoryDto
    {
        Guid ProductId { get; }
        int Quantity { get; }
        string Reason { get; }
    }

    // Commands and Queries
    public interface IAdjustInventory
    {
        Guid ProductId { get; }
        int Quantity { get; }
        string Reason { get; }
    }

    public interface IGetInventoryStock
    {
        Guid ProductId { get; }
    }

    public interface IGetInventoryHistory
    {
        Guid ProductId { get; }
    }

    public interface IGetAllInventory
    {
    }

    // Definimos interfaces para las clases necesarias para los tests del controlador
    public interface IInventoryRepository { }
    public interface IUnitOfWork { }
    public interface IInventoryItem { }
    public interface IInventoryMovement { }

    // MediatR interfaces
    public interface IRequest { }

    public interface IRequest<out TResponse> { }
    
    public interface IMediator
    {
        Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
        Task Send(IRequest request, CancellationToken cancellationToken = default);
    }
}
