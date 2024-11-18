using InventoryApp.Application.Extensions;
using InventoryApp.Application.Interfaces;
using InventoryApp.Models.Context;
using InventoryApp.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Repositories
{
    public class InventoryRepository : Repository<Inventory>, IInventoryRepository
    {
        private readonly InventoryAppDbContext context;

        public InventoryRepository(InventoryAppDbContext context) : base(context)
        {
            this.context = context;
        }
        public async Task<Inventory> GetByProductIdWithIsTakenAsync(int productId)
        {
            return await this.context.Set<Inventory>()
                .FilterByProductIdWithIsTaken(productId)
                .FirstOrDefaultAsync();
        }
        public async Task<List<Inventory>> GetByProductIdAsync(int productId)
        {
            return await this.context.Set<Inventory>()
                .AsNoTracking()
                .FilterByProductId(productId)
                .ToListAsync();
        }
        public async Task<List<Inventory>> GetByDeliveredDateAsync(DateTime startDate, DateTime endDate)
        {
            return await this.context.Set<Inventory>()
                .AsNoTracking()
                .FilterByDeliveredDate(startDate, endDate)
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
