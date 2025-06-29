using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Inventory.Api.Application.Queries;
using Inventory.Api.Common.DTOs;
using Inventory.Api.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Inventory.Api.Domain.Entities;

namespace Inventory.Api.Application.Handlers.Queries
{
    public class GetInventoryHandler : IRequestHandler<GetInventory, IEnumerable<InventoryDto>>
    {
        private readonly IGenericRepository<InventoryItem> _repo;

        public GetInventoryHandler(IGenericRepository<InventoryItem> repo)
            => _repo = repo;

        public async Task<IEnumerable<InventoryDto>> Handle(
            GetInventory request,
            CancellationToken cancellationToken)
        {
            var list = await _repo.Query()
                                  .AsNoTracking()
                                  .ToListAsync(cancellationToken);

            return list
                .Select(i => new InventoryDto(i.ProductId, i.Quantity));
        }
    }
}
