using InventoryApp.Models.Entity;

namespace InventoryApp.Application.Interfaces
{
    public interface IInventoryRepository : IRepository<Inventory>
    {
        Task<Inventory> GetByProductIdWithIsTakenAsync(int productId);

        Task<List<Inventory>> GetByProductIdAsync(int productId);
    }
}
