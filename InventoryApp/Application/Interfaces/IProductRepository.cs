﻿using InventoryApp.Models.Entity;

namespace InventoryApp.Application.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<List<Product>> GetByInvoicePurchaseDateAsync(DateTime startDate, DateTime endDate);
        Task<List<Product>> GetByProductIdAsync(int productId);
        Task<List<int>> GetProductIdsByInvoiceIdAsync(int invoiceId);
    }
}
