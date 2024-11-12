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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.Product)
                .WithMany(p => p.Inventories)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.GivenByEmployee)
                .WithMany(e => e.InventoriesGiven)
                .HasForeignKey(i => i.GivenByEmployeeId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.ReceivedByEmployee)
                .WithMany(e => e.InventoriesReceived)
                .HasForeignKey(i => i.ReceivedByEmployeeId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}