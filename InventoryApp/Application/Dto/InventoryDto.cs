using InventoryApp.Models.Entity;

namespace InventoryApp.Application.Dto
{
    public class InventoryDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int ProductId { get; set; }
        public DateTime Date { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
