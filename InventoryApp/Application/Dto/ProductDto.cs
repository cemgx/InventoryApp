using InventoryApp.Models.Entity;
using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Application.Dto
{
    public class ProductDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "İsim değeri boş olamaz")]
        public string Name { get; set; }
        public int ProductTypeId { get; set; }
        public int InvoiceId { get; set; }
    }
}
