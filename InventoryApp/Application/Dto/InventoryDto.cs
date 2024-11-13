using InventoryApp.Models.Entity;

namespace InventoryApp.Application.Dto
{
    public class InventoryDto
    {
        public int Id { get; set; }
        public int GivenByEmployeeId { get; set; }
        public string? GivenByEmployeeName { get; set; }
        public int ReceivedByEmployeeId { get; set; }
        public string? ReceivedByEmployeeName { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
