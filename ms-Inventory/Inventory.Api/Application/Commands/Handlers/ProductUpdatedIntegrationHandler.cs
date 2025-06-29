// Inventory.Api/Application/IntegrationHandlers/ProductUpdatedIntegrationHandler.cs
using System.Threading.Tasks;
using Inventory.Api.Application.Common.Interfaces;
using Inventory.Api.Application.Events;
using Microsoft.Extensions.Logging;

namespace Inventory.Api.Application.Commands.Handlers
{
    public class ProductUpdatedIntegrationHandler : IIntegrationEventHandler<ProductUpdatedEvent>
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly ILogger<ProductUpdatedIntegrationHandler> _logger;

        public ProductUpdatedIntegrationHandler(IInventoryRepository inventoryRepository, ILogger<ProductUpdatedIntegrationHandler> logger)
        {
            _inventoryRepository = inventoryRepository;
            _logger = logger;
        }

        // CORRECCIÓN: Implementar el método Handle con la firma correcta
        public Task Handle(ProductUpdatedEvent @event)
        {
            _logger.LogInformation("Evento de producto actualizado recibido para {ProductId}", @event.ProductId);
            // Aquí iría la lógica para manejar la actualización, si fuera necesaria.
            // Por ahora, solo registramos el evento.
            return Task.CompletedTask;
        }
    }
}
