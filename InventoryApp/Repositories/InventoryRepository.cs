using InventoryApp.Application.Interfaces;
using InventoryApp.Models.Context;
using InventoryApp.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Repositories
{
    public class InventoryRepository : Repository<Inventory>, IInventoryRepository
    {
        public InventoryRepository(InventoryAppDbContext context) : base(context)
        {
        }
        public async Task<Inventory> GetByProductIdWithIsTakenAsync(int productId)
        {
            return await this.context.Set<Inventory>()
                .Where(i => i.ProductId == productId && i.IsTaken)
                .FirstOrDefaultAsync();
        }
        public async Task<List<Inventory>> GetByProductIdAsync(int productId)
        {
            return await this.context.Set<Inventory>().AsNoTracking()
                .Where(i => i.ProductId == productId)
                .ToListAsync();
        }
        public async Task<List<Inventory>> GetByDeliveredDateAsync(DateTime startDate, DateTime endDate)
        {
            return await context.Set<Inventory>()
                .Where(i => i.DeliveredDate >= startDate && i.DeliveredDate <= endDate)
                .ToListAsync();
        }
        public async Task UpdateReturnDateAsync(int id, DateTime? returnDate)
        {
            var inventory = await context.Inventories.FindAsync(id);
            if (inventory != null)
            {
                inventory.ReturnDate = returnDate;

                inventory.IsTaken = inventory.DeliveredDate.HasValue && !returnDate.HasValue;

                await context.SaveChangesAsync();
            }
        }

    }
}
