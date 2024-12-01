using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Application.Dto
{
    public class ForgotPasswordResponseDto
    {
        public string Email { get; set; }

        public string ForgotCode { get; set; }

        public string NewPassword { get; set; }

    }
}
