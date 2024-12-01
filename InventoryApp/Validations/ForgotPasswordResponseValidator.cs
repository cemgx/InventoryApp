using FluentValidation;
using InventoryApp.Application.Dto;

namespace InventoryApp.Validations
{
    public class ForgotPasswordResponseValidator : AbstractValidator<ForgotPasswordResponseDto>
    {
        public ForgotPasswordResponseValidator()
        {
            RuleFor(e => e.ForgotCode)
                .NotEmpty().WithMessage("Kod alanı boş olamaz.")
                .Length(6).WithMessage("Kod en az 6 karakter olmalı");

            RuleFor(e => e.Email)
                .NotEmpty().WithMessage("E-posta boş olamaz.")
                .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz");

            RuleFor(e => e.NewPassword)
                .NotEmpty().WithMessage("Yeni şifre boş olamaz.")
                .MinimumLength(3).WithMessage("Şifre en az 3 karakter olmalı");
        }
    }
}
