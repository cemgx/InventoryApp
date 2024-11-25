using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Application.Dto
{
    public class ForgotPasswordResponseDto
    {
        [EmailAddress(ErrorMessage = "Geçerli bir mail adresi giriniz.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Kod alanı boş olamaz.")]
        public string ForgotCode { get; set; }

        [Required(ErrorMessage = "Yeni şifre boş olamaz.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Yeni şifreyi tekrar giriniz.")]
        public string NewPassword2 { get; set; }

    }
}
