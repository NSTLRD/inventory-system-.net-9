using System;
using System.Threading;
using System.Threading.Tasks;
using Inventory.Api.Common.Interfaces;
using Inventory.Api.Domain.Entities;
using Inventory.Api.Application.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Inventory.Api.Application.Commands.Handlers
{
    public class AdjustInventoryHandler : IRequestHandler<AdjustInventory, Unit>
    {
        private readonly IInventoryRepository _repo;
        private readonly ILogger<AdjustInventoryHandler> _logger;
        
        private readonly IMediator _mediator;

        public AdjustInventoryHandler(
            IInventoryRepository repo,
            ILogger<AdjustInventoryHandler> logger,
            IMediator mediator) 
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<Unit> Handle(AdjustInventory request, CancellationToken ct)
        {
            
            await ExecuteAdjustmentAsync(request, ct);
            return Unit.Value;
        }

        private async Task ExecuteAdjustmentAsync(AdjustInventory request, CancellationToken ct)
        {
            try
            {
               
                InventoryItem? item; 
                
                string reason = string.IsNullOrWhiteSpace(request.Reason)
                            ? "Ajuste sin motivo"
                            : request.Reason;

                item = await _repo.GetByProductIdAsync(request.ProductId);
                
                if (item == null)
                {
                    _logger.LogInformation("Creando nuevo inventario para producto {ProductId}", request.ProductId);
                    item = new InventoryItem(request.ProductId, initialStock: request.Quantity);
                    
                    await _repo.AddAsync(item);
                    
                    await _repo.SaveChangesAsync();
                }
                else
                {
                    
                    var previousStock = item.Stock;
                    var change = request.Quantity - previousStock;
                    
                    item.UpdateStockDirectly(request.Quantity);
                    
                    var movement = InventoryMovement.Create(
                        item.Id,
                        change,
                        reason
                    );
                    
                    await _repo.AddMovementAsync(movement);
                    
                    await _repo.SaveChangesAsync();
                }
                
                await _mediator.Publish(new InventoryAdjustedEvent(
                    item.Id,
                    item.ProductId,
                    item.Stock,
                    reason,
                    DateTime.UtcNow
                ), ct);
                
                _logger.LogInformation(
                    "Inventario ajustado → Producto={ProductId}, NuevoStock={Stock}",
                    request.ProductId, item.Stock
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al ajustar inventario para producto {ProductId}", request.ProductId);
                throw;
            }
        }
    }
}
