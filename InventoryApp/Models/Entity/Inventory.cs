using InventoryApp.Application.Interfaces;

namespace InventoryApp.Models.Entity
{
    public class Inventory : BaseEntity
    {
        public Employee GivenByEmployee { get; set; }
        public Employee ReceivedByEmployee { get; set; }
        public int GivenByEmployeeId { get; set; }
        public int ReceivedByEmployeeId { get; set; }
        public Product Product { get; set; }
        public int ProductId { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsTaken { get; set; } = false;
    }
}
