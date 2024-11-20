using InventoryApp.Application.Interfaces;

namespace InventoryApp.Models.Entity
{
    public class ProductType : ISoftDelete
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Product>? Products { get; set; }
        public bool IsDeleted { get; set; }
    }
}