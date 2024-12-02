using InventoryApp.Application.MiddleWares;
using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Application.LogEntities
{
    public class EmployeeLog
    {
        [SensitiveDataAttirbute]
        public string Name { get; set; }

        [SensitiveDataAttirbute]
        public required string Email { get; set; }

        [SensitiveDataAttirbute]
        public string Password { get; set; }
    }
}
