using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Application.Dto
{
    public class EmployeeRequestDto
    {
        [Required(ErrorMessage = "İsim değeri boş olamaz")]
        public string Name { get; set; }

        [EmailAddress(ErrorMessage = "Geçerli bir mail adresi giriniz.")]
        public required string Email { get; set; }
        public string Password { get; set; }
    }
}
