using InventoryApp.Application.Extensions;
using InventoryApp.Application.Interfaces;
using InventoryApp.Models.Context;
using InventoryApp.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Repositories
{
    public class ProductTypeRepository : Repository<ProductType>, IProductTypeRepository
    {
        private readonly InventoryAppDbContext _context;

        public ProductTypeRepository(InventoryAppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<ProductType>> GetByProductTypeIdAsync(int productTypeId, CancellationToken cancellationToken)
        {
            return await _context.Set<ProductType>()
                .AsNoTracking()
                .FilterById(i => i.Id, productTypeId)
                .ToListAsync(cancellationToken);
        }
    }
}
