using InventoryApp.Models.Entity;
using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Application.Dto
{
    public class InventoryResponseDto
    {
        public int Id { get; set; }
        public int GivenByEmployeeId { get; set; }
        public int ReceivedByEmployeeId { get; set; }
        public int ProductId { get; set; }

        public DateTime? DeliveredDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsTaken { get; set; } = false;
    }
}
