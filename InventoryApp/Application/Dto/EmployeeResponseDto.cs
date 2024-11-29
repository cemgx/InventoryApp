using InventoryApp.Application.MiddleWares;
using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Application.Dto
{
    public class EmployeeResponseDto
    {
        public int Id { get; set; }

        [SensitiveDataAttirbute]
        [Required(ErrorMessage ="İsim değeri boş olamaz")]
        public string Name { get; set; }

        [SensitiveDataAttirbute]
        [EmailAddress(ErrorMessage = "Geçerli bir mail adresi giriniz.")]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
