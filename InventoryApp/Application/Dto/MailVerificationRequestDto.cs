using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Application.Dto
{
    public class MailVerificationRequestDto
    {
        [EmailAddress(ErrorMessage = "Geçerli bir mail adresi giriniz.")]
        public string Email { get; set; }

        [Required]
        public string VerificationCode { get; set; }
    }
}
