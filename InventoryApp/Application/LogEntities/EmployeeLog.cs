using InventoryApp.Application.MiddleWares;
using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Application.LogEntities
{
    public class EmployeeLog
    {
        [SensitiveDataAttirbute]
        [Required(ErrorMessage = "İsim değeri boş olamaz")]
        public string Name { get; set; }

        [SensitiveDataAttirbute]
        [EmailAddress(ErrorMessage = "Geçerli bir mail adresi giriniz.")]
        public required string Email { get; set; }
        public string Password { get; set; }
    }
}
