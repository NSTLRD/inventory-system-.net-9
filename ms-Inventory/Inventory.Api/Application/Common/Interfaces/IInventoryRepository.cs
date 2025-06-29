using Inventory.Api.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.Api.Application.Common.Interfaces
{
    public interface IInventoryRepository
    {
        Task<InventoryItem> GetByIdAsync(Guid id);
        Task<InventoryItem> GetByProductIdAsync(Guid productId);
        Task<IEnumerable<InventoryItem>> GetAllAsync();
        Task<IEnumerable<InventoryMovement>> GetMovementsForInventoryAsync(Guid inventoryId);
        Task AddAsync(InventoryItem inventory);
        Task UpdateAsync(InventoryItem inventory);
        Task DeleteAsync(InventoryItem inventory);
        Task SaveChangesAsync(); // Añadido método para guardar cambios
    }
}