namespace InventoryApp.Models.Entity
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public ProductType ProductType { get; set; }
        public int ProductTypeId { get; set; }
        public decimal PurchasePrice { get; set; }
        public string? InvoiceInfo { get; set; }
        public DateTime PurchaseDate { get; set; }
        public List<Inventory> Inventories { get; set; }
    }
}
