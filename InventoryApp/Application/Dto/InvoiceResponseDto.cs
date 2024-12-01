using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Application.Dto
{
    public class InvoiceResponseDto
    {
        public int Id { get; set; }

        public int InvoiceNo { get; set; }

        public string FirmName { get; set; }

        public string Price { get; set; }

        public DateTime PurchaseDate { get; set; }
    }
}
