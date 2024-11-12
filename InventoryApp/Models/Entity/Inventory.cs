namespace InventoryApp.Models.Entity
{
    public class Inventory
    {
        public int Id { get; set; }
        public Employee Employee { get; set; }
        public int EmployeeId { get; set; }
        public Product Product { get; set; }
        public int ProductId { get; set; }
        public DateTime Date { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
