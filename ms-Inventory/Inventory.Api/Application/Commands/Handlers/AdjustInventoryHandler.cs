// Inventory.Api/Application/Commands/Handlers/AdjustInventoryHandler.cs
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Inventory.Api.Application.Commands;
using Inventory.Api.Application.Common.Interfaces;

namespace Inventory.Api.Application.Commands.Handlers
{
    public class AdjustInventoryHandler
        : IRequestHandler<AdjustInventory, Unit>
    {
        private readonly IInventoryRepository _repository;
        private readonly ILogger<AdjustInventoryHandler> _logger;

        public AdjustInventoryHandler(
            IInventoryRepository repository,
            ILogger<AdjustInventoryHandler> logger)
        {
            _repository = repository;
            _logger     = logger;
        }

        public async Task<Unit> Handle(
            AdjustInventory request,
            CancellationToken cancellationToken)
        {
            var inventory = await _repository
                .GetByProductIdAsync(request.ProductId);

            if (inventory is null)
            {
                _logger.LogWarning(
                  "Inventory item for product {ProductId} not found",
                  request.ProductId);
                return Unit.Value;
            }

            inventory.SetStock(request.Quantity, request.Reason);

            await _repository.UpdateAsync(inventory);
            await _repository.SaveChangesAsync();

            _logger.LogInformation(
              "Inventory adjusted for product {ProductId}. New stock: {Stock}",
              request.ProductId, request.Quantity);

            return Unit.Value;
        }
    }
}
