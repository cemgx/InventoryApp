using InventoryApp.Models.Entity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace InventoryApp.Application.Extensions
{
    public static class QueryableExtensions
    {
        // Name e göre filtreleme
        public static IQueryable<T> FilterByProperty<T>(
            this IQueryable<T> query,
            string propertyName,
            string searchValue)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentException("Property adı boş olamaz.", nameof(propertyName));

            if (string.IsNullOrWhiteSpace(searchValue))
                throw new ArgumentException("Arama kriteri boş olamaz.", nameof(searchValue));

            return query.Where(e => EF.Functions.Like(
                EF.Property<string>(e, propertyName),
                $"%{searchValue}%"));
        }

        // Sadece "Name" 
        public static IQueryable<T> FilterByName<T>(
            this IQueryable<T> query,
            string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Arama kriteri boş olamaz.", nameof(name));

            return query.FilterByProperty("Name", name);
        }

        public static IQueryable<T> FilterById<T>(
            this IQueryable<T> query,
            Expression<Func<T, int>> idSelector,
            int id)
        {
            return query.Where(entity => idSelector.Compile()(entity) == id);
        }

        // Inventory
        public static IQueryable<Inventory> FilterByProductIdWithIsTaken(
            this IQueryable<Inventory> query,
            int productId)
        {
            return query.Where(i => i.ProductId == productId && i.IsTaken);
        }

        public static IQueryable<Inventory> FilterByProductId(
            this IQueryable<Inventory> query,
            int productId)
        {
            return query.Where(i => i.ProductId == productId);
        }

        public static IQueryable<Inventory> FilterByDeliveredDate(
            this IQueryable<Inventory> query,
            DateTime startDate,
            DateTime endDate)
        {
            return query.Where(i => i.DeliveredDate >= startDate && i.DeliveredDate <= endDate);
        }

        // Product
        public static IQueryable<Product> FilterByInvoicePurchaseDate(
            this IQueryable<Product> query,
            DateTime startDate,
            DateTime endDate)
        {
            return query
                .Include(p => p.Invoice)
                .Where(p => p.Invoice != null &&
                            p.Invoice.PurchaseDate >= startDate &&
                            p.Invoice.PurchaseDate <= endDate);
        }
    }
}
