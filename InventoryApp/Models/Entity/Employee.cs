using InventoryApp.Application.Interfaces;

namespace InventoryApp.Models.Entity
{
    public class Employee : BaseEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string? ForgotCode { get; set; }
        public bool IsVerified { get; set; } = false;
        public string? MailVerificationCode { get; set; }
        public List<Inventory> InventoriesGiven { get; set; }
        public List<Inventory> InventoriesReceived { get; set; }
    }
}