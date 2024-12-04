using InventoryApp.Application.Extensions;
using InventoryApp.Application.Interfaces;
using InventoryApp.Models.Context;
using InventoryApp.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Repositories
{
    public class InventoryRepository : Repository<Inventory>, IInventoryRepository
    {
        private readonly InventoryAppDbContext _context;

        public InventoryRepository(InventoryAppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Inventory> GetByProductIdWithIsTakenAsync(int productId, CancellationToken cancellationToken)
        {
            return await _context.Set<Inventory>()
                .FilterByProductIdWithIsTaken(productId)
                .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<List<Inventory>> GetByProductIdAsync(int productId, CancellationToken cancellationToken)
        {
            return await _context.Set<Inventory>()
                .AsNoTracking()
                .FilterByProductId(productId)
                .ToListAsync(cancellationToken);
        }
        public async Task<List<Inventory>> GetByDeliveredDateAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
        {
            return await _context.Set<Inventory>()
                .AsNoTracking()
                .FilterByDeliveredDate(startDate, endDate)
                .ToListAsync(cancellationToken);
        }
        public async Task UpdateReturnDateAsync(int id, DateTime? returnDate, CancellationToken cancellationToken)
        {
            var inventory = await _context.Inventories.FindAsync(id, cancellationToken);
            if (inventory != null)
            {
                inventory.ReturnDate = returnDate;

                inventory.IsTaken = inventory.DeliveredDate.HasValue && !returnDate.HasValue;

                await _context.SaveChangesAsync(cancellationToken);
            }
        }

    }
}
