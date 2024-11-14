using InventoryApp.Models.Entity;

namespace InventoryApp.Application.Dto
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProductTypeId { get; set; }
        public decimal PurchasePrice { get; set; }
        public int InvoiceId { get; set; }
        public DateTime PurchaseDate { get; set; }
    }
}
