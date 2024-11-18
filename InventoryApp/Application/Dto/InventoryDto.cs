using InventoryApp.Models.Entity;
using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Application.Dto
{
    public class InventoryDto
    {
        public int Id { get; set; }
        public int GivenByEmployeeId { get; set; }
        public int ReceivedByEmployeeId { get; set; }
        public int ProductId { get; set; }

        [Required(ErrorMessage = "DeliveredDate zorunludur")]
        public DateTime? DeliveredDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsTaken { get; set; } = false;
    }
}
