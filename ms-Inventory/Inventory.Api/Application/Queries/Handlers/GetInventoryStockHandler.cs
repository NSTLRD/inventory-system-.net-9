// Inventory.Api/Application/Handlers/Queries/GetInventoryStockHandler.cs

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Inventory.Api.Application.Queries;
using Inventory.Api.Common.DTOs;
using Inventory.Api.Common.Interfaces;
using Inventory.Api.Domain.Entities;

namespace Inventory.Api.Application.Queries.Handlers
{
    public class GetInventoryStockHandler
        : IRequestHandler<GetInventoryStock, InventoryStockDto>
    {
        private readonly IGenericRepository<InventoryItem> _repo;

        public GetInventoryStockHandler(IGenericRepository<InventoryItem> repo)
        {
            _repo = repo;
        }

        public async Task<InventoryStockDto> Handle(
            GetInventoryStock request,
            CancellationToken cancellationToken)
        {
            // 1) Intentamos leer el item
            var item = await _repo.Query()
                                  .AsNoTracking()
                                  .FirstOrDefaultAsync(
                                     i => i.ProductId == request.ProductId,
                                     cancellationToken);

            // 2) Si no existe, devolvemos stock = 0
            var stock = item?.Quantity ?? 0;

            return new InventoryStockDto(request.ProductId, stock);
        }
    }
}
