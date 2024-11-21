using InventoryApp.Application.Interfaces;

namespace InventoryApp.Models.Entity
{
    public class ProductType : BaseEntity
    {
        public string Name { get; set; }
        public List<Product>? Products { get; set; }
    }
}