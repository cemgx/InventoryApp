using InventoryApp.Application.Interfaces;

namespace InventoryApp.Models.Entity
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public ProductType ProductType { get; set; }
        public int ProductTypeId { get; set; }
        public Invoice Invoice { get; set; }
        public int InvoiceId { get; set; }
        public List<Inventory> Inventories { get; set; }
    }
}