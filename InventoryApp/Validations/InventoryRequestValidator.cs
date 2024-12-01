using FluentValidation;
using InventoryApp.Application.Dto;

namespace InventoryApp.Validations
{
    public class InventoryRequestValidator : AbstractValidator<InventoryRequestDto>
    {
        public InventoryRequestValidator()
        {
            RuleFor(e => e.DeliveredDate)
                .NotEmpty().WithMessage("DeliveredDate zorunludur");
        }
    }
}
