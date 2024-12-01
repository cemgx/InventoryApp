using FluentValidation;
using InventoryApp.Application.Dto;

namespace InventoryApp.Validations
{
    public class ProductTypeResponseValidator : AbstractValidator<ProductTypeResponseDto>
    {
        public ProductTypeResponseValidator()
        {
            RuleFor(e => e.Name)
                .NotEmpty().WithMessage("İsim boş olamaz.");
        }
    }
}