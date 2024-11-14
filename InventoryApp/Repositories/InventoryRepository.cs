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
            return await this.context.Set<Inventory>()
                .Where(i => i.ProductId == productId)
                .ToListAsync();
        }
    }
}
