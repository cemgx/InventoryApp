using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Application.Dto
{
    public class InvoiceRequestDto
    {
        [Required(ErrorMessage = "InvoiceNo boş bırakılamaz.")]
        [Range(1, int.MaxValue, ErrorMessage = "InvoiceNo geçerli bir pozitif sayı olmalıdır.")]
        public int InvoiceNo { get; set; }

        [Required(ErrorMessage = "FirmName boş bırakılamaz.")]
        public string FirmName { get; set; }

        [Required(ErrorMessage = "Price boş bırakılamaz.")]
        public string Price { get; set; }

        [Required(ErrorMessage = "PurchaseDate boş bırakılamaz.")]
        public DateTime PurchaseDate { get; set; }
    }
}
