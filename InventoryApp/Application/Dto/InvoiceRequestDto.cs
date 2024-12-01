using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Application.Dto
{
    public class InvoiceRequestDto
    {
        public int InvoiceNo { get; set; }

        public string FirmName { get; set; }

        public string Price { get; set; }

        public DateTime PurchaseDate { get; set; }
    }
}
