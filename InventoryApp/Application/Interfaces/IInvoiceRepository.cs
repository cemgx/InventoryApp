using InventoryApp.Models.Entity;

namespace InventoryApp.Application.Interfaces
{
    public interface IInvoiceRepository : IRepository<Invoice>
    {
        Task<List<Invoice>> GetByFirmNameAsync(string name);
    }
}
