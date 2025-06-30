

using System.Threading;
using System.Threading.Tasks;
using Inventory.Api.Common.Interfaces;
using Inventory.Api.Domain.Entities;
using Inventory.Api.Application.Queries;
using Inventory.Api.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Application.Queries.Handlers
{
    public class GetInventoryStockHandler : IRequestHandler<GetInventoryStock, InventoryStockDto>
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
            var item = await _repo.Query()
                                  .FirstOrDefaultAsync(
                                     i => i.ProductId == request.ProductId,
                                     cancellationToken);

            var stock = item?.Stock ?? 0;

            return new InventoryStockDto(request.ProductId, stock);
        }
    }
}
