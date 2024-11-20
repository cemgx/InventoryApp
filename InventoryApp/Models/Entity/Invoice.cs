using InventoryApp.Application.Interfaces;

namespace InventoryApp.Models.Entity
{
    public class Invoice : ISoftDelete
    {
        public int Id { get; set; }
        public int InvoiceNo { get; set; }
        public string FirmName { get; set; }
        public string Price { get; set; }
        public DateTime PurchaseDate { get; set; }
        public ICollection<Product> Products { get; set; }
        public bool IsDeleted { get; set; }
    }
}
