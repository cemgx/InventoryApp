using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Application.Dto
{
    public class ProductTypeResponseDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "İsim değeri boş olamaz")]
        public string Name { get; set; }
    }
}
