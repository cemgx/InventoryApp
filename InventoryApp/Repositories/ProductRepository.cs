﻿using InventoryApp.Application.Extensions;
using InventoryApp.Application.Interfaces;
using InventoryApp.Models.Context;
using InventoryApp.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly InventoryAppDbContext context;
        public ProductRepository(InventoryAppDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<Product>> GetByProductIdAsync(int productId, CancellationToken cancellationToken)
        {
            return await this.context.Set<Product>()
                .AsNoTracking()
                .FilterById(i => i.Id, productId)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Product>> GetByInvoicePurchaseDateAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
        {
            return await this.context.Products
                .AsNoTracking()
                .FilterByInvoicePurchaseDate(startDate, endDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<int>> GetProductIdsByInvoiceIdAsync(int invoiceId, CancellationToken cancellationToken)
        {
            return await this.context.Products
                .AsNoTracking()
                .FilterProductIdsByInvoiceId(invoiceId)
                .ToListAsync(cancellationToken);
        }
    }
}
