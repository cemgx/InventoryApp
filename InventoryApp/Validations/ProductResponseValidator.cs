using FluentValidation;
using InventoryApp.Application.Dto;

namespace InventoryApp.Validations
{
    public class ProductResponseValidator : AbstractValidator<ProductResponseDto>
    {
        public ProductResponseValidator()
        {
            RuleFor(e => e.Name)
                .NotEmpty().WithMessage("İsim boş olamaz");
        }
    }
}
