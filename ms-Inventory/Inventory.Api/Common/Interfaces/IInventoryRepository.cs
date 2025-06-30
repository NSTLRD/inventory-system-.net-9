using Inventory.Api.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Common.Interfaces
{
    public interface IInventoryRepository
    {
        Task<InventoryItem> GetByIdAsync(Guid id);
        Task<InventoryItem?> GetByProductIdAsync(Guid productId);
        Task<IEnumerable<InventoryItem>> GetAllAsync();
        Task<IEnumerable<InventoryMovement>> GetMovementsForInventoryAsync(Guid inventoryId);
        Task AddAsync(InventoryItem inventory);
        Task AddMovementAsync(InventoryMovement movement);
        Task UpdateAsync(InventoryItem inventory);
        Task DeleteAsync(InventoryItem inventory);
        Task SaveChangesAsync();
        Task ReloadEntityAsync(InventoryItem item);
        DbContext GetDbContext();
    }
}
