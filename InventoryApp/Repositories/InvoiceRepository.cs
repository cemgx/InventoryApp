using InventoryApp.Application.Interfaces;
using InventoryApp.Models.Context;
using InventoryApp.Models.Entity;

namespace InventoryApp.Repositories
{
    public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(InventoryAppDbContext context) : base(context)
        {
        }
    }
}
