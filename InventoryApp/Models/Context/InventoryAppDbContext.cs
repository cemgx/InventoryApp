using InventoryApp.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Models.Context
{
    public class InventoryAppDbContext : DbContext 
    {
        public InventoryAppDbContext(DbContextOptions<InventoryAppDbContext> options) : base(options) { }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
    }
}
