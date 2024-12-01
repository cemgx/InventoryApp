using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Application.Dto
{
    public class MailVerificationRequestDto
    {
        public string Email { get; set; }

        public string VerificationCode { get; set; }
    }
}
