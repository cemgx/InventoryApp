using InventoryApp.Application.Interfaces;
using InventoryApp.Models.Context;
using InventoryApp.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Repositories
{
    public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(InventoryAppDbContext context) : base(context)
        {
        }

        public async Task<List<Invoice>> GetByFirmNameAsync(string name)
        {
            return await this.context.Set<Invoice>()
                    .Where(e => EF.Property<string>(e, "FirmName").ToLower().Contains(name.ToLower()))
                    .ToListAsync();
        }
    }
}
