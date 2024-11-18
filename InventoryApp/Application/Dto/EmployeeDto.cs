using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Application.Dto
{
    public class EmployeeDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="İsim değeri boş olamaz")]
        public string Name { get; set; }

        [EmailAddress(ErrorMessage = "Geçerli bir mail adresi giriniz.")]
        public string Email { get; set; }
    }
}
