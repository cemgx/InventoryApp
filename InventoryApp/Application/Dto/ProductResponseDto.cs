using InventoryApp.Models.Entity;
using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Application.Dto
{
    public class ProductResponseDto
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public int ProductTypeId { get; set; }
        public int InvoiceId { get; set; }
    }
}
