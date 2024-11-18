using InventoryApp.Application.Interfaces;
using InventoryApp.Models.Context;
using InventoryApp.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(InventoryAppDbContext context) : base(context)
        {
        }
        public async Task<List<Product>> GetByInvoicePurchaseDateAsync(DateTime startDate, DateTime endDate)
        {
            return await context.Products
                .Include(p => p.Invoice).AsNoTracking()
                .Where(p => p.Invoice != null &&
                            p.Invoice.PurchaseDate >= startDate &&
                            p.Invoice.PurchaseDate <= endDate)
                .ToListAsync();
        }
    }
}
