namespace InventoryApp.Models.Entity
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<Inventory> InventoriesGiven { get; set; }  // Veren employee'ler için
        public List<Inventory> InventoriesReceived { get; set; } // Alan employee'ler için
    }
}