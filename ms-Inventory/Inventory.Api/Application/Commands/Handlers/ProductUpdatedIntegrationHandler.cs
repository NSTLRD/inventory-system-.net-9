
using System.Threading.Tasks;
using Inventory.Api.Common.Interfaces;
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


        public Task Handle(ProductUpdatedEvent @event)
        {
            _logger.LogInformation("Evento de producto actualizado recibido para {ProductId}", @event.ProductId);
            return Task.CompletedTask;
        }
    }
}
