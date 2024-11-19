using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Application.Dto
{
    public class ProductTypeRequestDto
    {
        [Required(ErrorMessage = "İsim değeri boş olamaz")]
        public string Name { get; set; }
    }
}
