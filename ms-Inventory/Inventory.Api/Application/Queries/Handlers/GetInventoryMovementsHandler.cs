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
    public class GetInventoryMovementsHandler
        : IRequestHandler<GetInventoryMovements, IEnumerable<MovementDto>>
    {
        private readonly IGenericRepository<InventoryItem> _repo;

        public GetInventoryMovementsHandler(IGenericRepository<InventoryItem> repo)
            => _repo = repo;

        public async Task<IEnumerable<MovementDto>> Handle(
            GetInventoryMovements request,
            CancellationToken cancellationToken)
        {
            var items = await _repo.Query()
                                   .Include(i => i.Movements)
                                   .ToListAsync(cancellationToken);

            return items
                .SelectMany(i => i.Movements.Select(m =>
                    new MovementDto(
                        m.Id,
                        m.InventoryItemId,
                        m.QuantityChange,
                        m.Timestamp
                    )
                ));
        }
    }
}
