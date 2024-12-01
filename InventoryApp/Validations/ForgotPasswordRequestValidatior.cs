using FluentValidation;
using InventoryApp.Application.Dto;

namespace InventoryApp.Validations
{
    public class ForgotPasswordRequestValidatior : AbstractValidator<ForgotPasswordRequestDto>
    {
        public ForgotPasswordRequestValidatior()
        {
            RuleFor(e => e.Email)
                    .NotEmpty().WithMessage("E-posta boş olamaz.")
                    .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz");
        }

    }
}
