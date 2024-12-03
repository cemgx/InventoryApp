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
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Log> Logs { get; set; }

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
            //x =>
            //{
            //x.HasData(
                    //new Inventory { Id = 1, GivenByEmployeeId = 1, ReceivedByEmployeeId = 2, ProductId = 1, DeliveredDate = DateTime.UtcNow },
                    //new Inventory { Id = 2, GivenByEmployeeId = 2, ReceivedByEmployeeId = 1, ProductId = 2, DeliveredDate = DateTime.UtcNow, ReturnDate = DateTime.Now });

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Invoice)
                .WithMany(i => i.Products)
                .HasForeignKey(p => p.InvoiceId)
                .OnDelete(DeleteBehavior.NoAction);
            //new Product { Id = 1, Name = "Klavye", ProductTypeId = 1, InvoiceId = 1 },
            //new Product { Id = 2, Name = "Kürek", ProductTypeId = 2, InvoiceId = 2 });

            //modelBuilder.Entity<Employee>(x =>
            //{
            //    x.Property(e => e.Email).IsRequired();
            //    x.HasData(
            //        new Employee { Id = 1, Email = "a@a.com", Name = "A", Password = "111", Salt = "111", IsVerified = true, IsDeleted = false },
            //        new Employee { Id = 2, Email = "s@s.com", Name = "S", Password = "111", Salt = "111", IsVerified = true, IsDeleted = false });
            //});
            //modelBuilder.Entity<ProductType>(x =>
            //{
            //    x.HasData(
            //        new ProductType { Id = 1, Name = "Elektronik" },
            //        new ProductType { Id = 2, Name = "Bahçe" });
            //});
            //modelBuilder.Entity<Invoice>(x =>
            //{
            //    x.HasData(
            //        new Invoice { Id = 1, FirmName = "Tekno", Price = "6000", InvoiceNo = 4321, PurchaseDate = DateTime.Now },
            //        new Invoice { Id = 2, FirmName = "GardenShop", Price = "5000", InvoiceNo = 1234, PurchaseDate = DateTime.Now });
            //});
        }
    }
}