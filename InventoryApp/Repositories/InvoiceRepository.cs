using InventoryApp.Application.Extensions;
using InventoryApp.Application.Interfaces;
using InventoryApp.Models.Context;
using InventoryApp.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Repositories
{
    public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
    {
        private readonly InventoryAppDbContext _context;
        public InvoiceRepository(InventoryAppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Invoice>> GetByInvoiceIdAsync(int invoiceId, CancellationToken cancellationToken)
        {
            return await _context.Set<Invoice>()
                .AsNoTracking()
                .FilterById(i => i.Id, invoiceId)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Invoice>> GetByFirmNameAsync(string name, CancellationToken cancellationToken)
        {
            return await _context.Set<Invoice>()
                .AsNoTracking()
                .FilterByProperty("FirmName", name)
                .ToListAsync(cancellationToken);
        }
    }
}
