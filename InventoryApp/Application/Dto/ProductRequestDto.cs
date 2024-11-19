using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Application.Dto
{
    public class ProductRequestDto
    {
        [Required(ErrorMessage = "İsim değeri boş olamaz")]
        public string Name { get; set; }
        public int ProductTypeId { get; set; }
        public int InvoiceId { get; set; }
    }
}
