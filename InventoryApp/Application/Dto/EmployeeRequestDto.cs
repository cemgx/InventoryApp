using InventoryApp.Application.MiddleWares;
using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Application.Dto
{
    public class EmployeeRequestDto
    {
        public string Name { get; set; }

        public required string Email { get; set; }
        public string Password { get; set; }
    }
}
