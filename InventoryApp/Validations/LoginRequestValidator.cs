using FluentValidation;
using InventoryApp.Application.Dto;

namespace InventoryApp.Validations
{
    public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
    {
        public LoginRequestValidator() 
        {
            RuleFor(e => e.Email)
                .NotEmpty().WithMessage("E-posta boş olamaz.")
                .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz");

            RuleFor(e => e.Password)
                .NotEmpty().WithMessage("Şifre boş olamaz.");
        }
    }
}