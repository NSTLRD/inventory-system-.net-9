// Inventory.Api/Application/IntegrationHandlers/ProductCreatedIntegrationHandler.cs
using System;
using System.Threading.Tasks;
using Inventory.Api.Application.Common.Interfaces;
using Inventory.Api.Application.Events;
using Inventory.Api.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Inventory.Api.Application.Commands.Handlers
{
    public class ProductCreatedIntegrationHandler : IIntegrationEventHandler<ProductCreatedEvent>
    {
        private readonly IInventoryRepository _repository;
        private readonly ILogger<ProductCreatedIntegrationHandler> _logger;

        public ProductCreatedIntegrationHandler(
            IInventoryRepository repository,
            ILogger<ProductCreatedIntegrationHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task Handle(ProductCreatedEvent @event)
        {
            _logger.LogInformation("Creating inventory for product {ProductId}", @event.ProductId);
            
            // Crear un nuevo ítem de inventario con el stock inicial
            var inventoryItem = new InventoryItem(@event.ProductId, @event.InitialStock);
            
            // Guardar en el repositorio
            await _repository.AddAsync(inventoryItem);
            await _repository.SaveChangesAsync();
            
            _logger.LogInformation("Inventory created for product {ProductId} with initial stock {InitialStock}", 
                @event.ProductId, @event.InitialStock);
        }
    }
}
