using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Application.Dto
{
    public class PasswordRequestDto
    {
        public string Password { get; set; }
        public string NewPassword { get; set; }
    }
}
