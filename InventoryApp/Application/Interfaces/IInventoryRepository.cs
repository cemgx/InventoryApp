using InventoryApp.Models.Entity;

namespace InventoryApp.Application.Interfaces
{
    public interface IInventoryRepository : IRepository<Inventory>
    {
        Task<Inventory> GetByProductIdWithIsTakenAsync(int productId, CancellationToken cancellationToken);
        Task<List<Inventory>> GetByProductIdAsync(int productId, CancellationToken cancellationToken);
        Task<List<Inventory>> GetByDeliveredDateAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken);
        Task UpdateReturnDateAsync(int id, DateTime? returnDate, CancellationToken cancellationToken);
    }
}
