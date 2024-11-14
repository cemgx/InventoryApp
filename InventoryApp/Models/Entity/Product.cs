namespace InventoryApp.Models.Entity
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ProductType ProductType { get; set; }
        public int ProductTypeId { get; set; }
        public decimal PurchasePrice { get; set; }
        public Invoice Invoice { get; set; }
        public int InvoiceId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public List<Inventory> Inventories { get; set; }
    }
}
