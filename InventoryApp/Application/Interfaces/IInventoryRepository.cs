using InventoryApp.Models.Entity;

namespace InventoryApp.Application.Interfaces
{
    public interface IInventoryRepository : IRepository<Inventory>
    {
        Task<Inventory> GetByProductIdWithIsTakenAsync(int productId);
        Task<List<Inventory>> GetByProductIdAsync(int productId);
        Task<List<Inventory>> GetByDeliveredDateAsync(DateTime startDate, DateTime endDate);
        Task UpdateReturnDateAsync(int id, DateTime? returnDate);
    }
}
