using FluentValidation;
using InventoryApp.Application.Dto;

namespace InventoryApp.Validations
{
    public class InventoryResponseValidator : AbstractValidator<InventoryResponseDto>
    {
        public InventoryResponseValidator ()
        {
            RuleFor(e => e.DeliveredDate)
                .NotEmpty().WithMessage("DeliveredDate zorunludur");
        }
    }
}