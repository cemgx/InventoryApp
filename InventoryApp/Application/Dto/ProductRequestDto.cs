using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Application.Dto
{
    public class ProductRequestDto
    {
        public string Name { get; set; }
        public int ProductTypeId { get; set; }
        public int InvoiceId { get; set; }
    }
}
