using System;
using System.Threading.Tasks;
using Inventory.Api.Common.Interfaces;
using Inventory.Api.Application.Events;
using Microsoft.Extensions.Logging;

namespace Inventory.Api.Application.Commands.Handlers
{
    public class ProductDeletedIntegrationHandler : IIntegrationEventHandler<ProductDeletedEvent>
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly ILogger<ProductDeletedIntegrationHandler> _logger;

        public ProductDeletedIntegrationHandler(
            IInventoryRepository inventoryRepository,
            ILogger<ProductDeletedIntegrationHandler> logger)
        {
            _inventoryRepository = inventoryRepository;
            _logger = logger;
        }

        public async Task Handle(ProductDeletedEvent @event)
        {
            _logger.LogInformation("Processing product deleted event for {ProductId}", @event.ProductId);
            
            var inventory = await _inventoryRepository.GetByProductIdAsync(@event.ProductId);
            if (inventory != null)
            {
                await _inventoryRepository.DeleteAsync(inventory);
                await _inventoryRepository.SaveChangesAsync();
                _logger.LogInformation("Inventory for product {ProductId} has been deleted", @event.ProductId);
            }
            else
            {
                _logger.LogWarning("No inventory found for product {ProductId}", @event.ProductId);
            }
        }
    }
}