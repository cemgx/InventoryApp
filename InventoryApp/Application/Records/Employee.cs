using InventoryApp.Application.MiddleWares;

namespace InventoryApp.Application.Records
{
    public record Employee(
        [PiiDataAttirbute]string Name,
        [PiiDataAttirbute] string Email,
        DateOnly Date)
    {
        public Guid Id { get; } = Guid.NewGuid();
    }
}
