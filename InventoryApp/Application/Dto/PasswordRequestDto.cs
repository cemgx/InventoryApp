using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Application.Dto
{
    public class PasswordRequestDto
    {
        [EmailAddress(ErrorMessage = "Geçerli bir mail adresi giriniz.")]
        public string Email { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string NewPassword2 { get; set; }
    }
}
