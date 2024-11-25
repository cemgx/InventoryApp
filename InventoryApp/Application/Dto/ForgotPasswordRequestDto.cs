using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Application.Dto
{
    public class ForgotPasswordRequestDto
    {
        [EmailAddress(ErrorMessage = "Geçerli bir mail adresi giriniz.")]
        public string Email { get; set; }
    }
}
