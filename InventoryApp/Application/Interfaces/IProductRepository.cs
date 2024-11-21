using InventoryApp.Models.Entity;

namespace InventoryApp.Application.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<List<Product>> GetByInvoicePurchaseDateAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken);
        Task<List<Product>> GetByProductIdAsync(int productId, CancellationToken cancellationToken);
        Task<List<int>> GetProductIdsByInvoiceIdAsync(int invoiceId, CancellationToken cancellationToken);
    }
}
