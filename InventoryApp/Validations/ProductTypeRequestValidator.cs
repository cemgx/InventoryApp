using FluentValidation;
using InventoryApp.Application.Dto;

namespace InventoryApp.Validations
{
    public class ProductTypeRequestValidator : AbstractValidator<ProductTypeRequestDto>
    {
        public ProductTypeRequestValidator()
        {
            RuleFor(e => e.Name)
                .NotEmpty().WithMessage("İsim boş olamaz.");
        }
    }
}