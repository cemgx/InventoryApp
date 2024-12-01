using InventoryApp.Application.MiddleWares;
using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Application.Dto
{
    public class EmployeeResponseDto
    {
        public int Id { get; set; }

        [SensitiveDataAttirbute]
        public string Name { get; set; }

        [SensitiveDataAttirbute]
        public string Email { get; set; }

        public string Password { get; set; }
    }
}
