using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Inventory.Api.Common.Interfaces;
using Inventory.Api.Domain.Entities;
using Inventory.Api.Application.DTOs;

namespace Inventory.Api.Application.Queries.Handlers
{
    public class GetInventoryHistoryHandler 
        : IRequestHandler<GetInventoryHistory, IEnumerable<InventoryHistoryDto>>
    {
        private readonly IGenericRepository<InventoryItem> _repo;

        public GetInventoryHistoryHandler(IGenericRepository<InventoryItem> repo)
            => _repo = repo;

        public async Task<IEnumerable<InventoryHistoryDto>> Handle(
            GetInventoryHistory req,
            CancellationToken ct)
        {
            var item = await _repo.Query()
                                  .Include(x => x.Movements)
                                  .FirstOrDefaultAsync(x => x.ProductId == req.ProductId, ct)
                       ?? throw new KeyNotFoundException($"No inventory for {req.ProductId}");

            return item.Movements
                       .Select(m => new InventoryHistoryDto(
                           m.Id,
                           m.InventoryItemId,
                           m.QuantityChange,
                           m.Timestamp          
                       ));
        }
    }
}
