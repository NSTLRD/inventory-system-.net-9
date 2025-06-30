using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Inventory.Api.Common.Interfaces;
using Inventory.Api.Application.DTOs;
using Inventory.Api.Domain.Entities;

namespace Inventory.Api.Application.Queries.Handlers
{
    public class GetInventoryHandler : IRequestHandler<GetInventory, InventoryDto>
    {
        private readonly IGenericRepository<InventoryItem> _repository;
        
        public GetInventoryHandler(IGenericRepository<InventoryItem> repository)
        {
            _repository = repository;
        }
        
        public async Task<InventoryDto> Handle(GetInventory request, CancellationToken cancellationToken)
        {
            var item = await _repository.Query()
                                        .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);
            
            if (item == null)
                throw new KeyNotFoundException($"Inventory with id {request.Id} not found");
                
            return new InventoryDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                Stock = item.Stock,
                LastUpdated = item.LastUpdated
            };
        }
    }
}
