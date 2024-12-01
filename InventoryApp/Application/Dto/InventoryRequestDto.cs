using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Application.Dto
{
    public class InventoryRequestDto
    {
        public int GivenByEmployeeId { get; set; }
        public int ReceivedByEmployeeId { get; set; }
        public int ProductId { get; set; }

        public DateTime? DeliveredDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
