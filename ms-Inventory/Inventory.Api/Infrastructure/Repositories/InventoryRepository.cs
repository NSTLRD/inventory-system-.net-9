using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inventory.Api.Common.Interfaces;
using Inventory.Api.Domain.Entities;
using Inventory.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Infrastructure.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly InventoryDbContext _ctx;

        public InventoryRepository(InventoryDbContext ctx) 
            => _ctx = ctx;

        public async Task<InventoryItem> GetByIdAsync(Guid id) =>
            await _ctx.InventoryItems.FindAsync(id)
            ?? throw new KeyNotFoundException($"InventoryItem {id} no encontrado");

        public async Task<InventoryItem?> GetByProductIdAsync(Guid productId) =>
            await _ctx.InventoryItems
                .AsTracking()
                .FirstOrDefaultAsync(i => i.ProductId == productId);

        public async Task<IEnumerable<InventoryItem>> GetAllAsync() =>
            await _ctx.InventoryItems.ToListAsync();

        public async Task<IEnumerable<InventoryMovement>> GetMovementsForInventoryAsync(Guid inventoryId) =>
            await _ctx.InventoryMovements
                    .Where(m => m.InventoryItemId == inventoryId)
                    .ToListAsync();

        public async Task AddAsync(InventoryItem inventory) =>
            await _ctx.InventoryItems.AddAsync(inventory);

        public async Task AddMovementAsync(InventoryMovement movement) =>
            await _ctx.InventoryMovements.AddAsync(movement);

        public async Task UpdateAsync(InventoryItem inventory)
        {
            _ctx.Entry(inventory).State = EntityState.Modified;
            
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(InventoryItem inventory)
        {
            _ctx.InventoryItems.Remove(inventory);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync() =>
            await _ctx.SaveChangesAsync();

        public async Task ReloadEntityAsync(InventoryItem item)
        {
            await _ctx.Entry(item).ReloadAsync();
        }
        
        public DbContext GetDbContext()
        {
            return _ctx;
        }
    }
}