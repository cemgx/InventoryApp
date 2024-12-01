using FluentValidation;
using InventoryApp.Application.Dto;

namespace InventoryApp.Validations
{
    public class InvoiceResponseValidator : AbstractValidator<InvoiceResponseDto>
    {
        public InvoiceResponseValidator()
        {
            RuleFor(e => e.FirmName)
                .NotEmpty().WithMessage("FirmName boş bırakılamaz");

            RuleFor(e => e.Price)
                .NotEmpty().WithMessage("Price boş bırakılamaz");

            RuleFor(e => e.PurchaseDate)
                .NotEmpty().WithMessage("PurchaseDate boş bırakılamaz");

            RuleFor(e => e.InvoiceNo)
                .NotEmpty().WithMessage("InvoiceNo boş bırakılamaz")
                .ExclusiveBetween(1, int.MaxValue).WithMessage("InvoiceNo geçerli bir pozitif sayı olmalıdır");
        }
    }
}
