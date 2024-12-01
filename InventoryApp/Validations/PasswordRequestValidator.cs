using FluentValidation;
using InventoryApp.Application.Dto;

namespace InventoryApp.Validations
{
    public class PasswordRequestValidator : AbstractValidator<PasswordRequestDto>
    {
        public PasswordRequestValidator()
        {
            RuleFor(e => e.Password)
                .NotEmpty().WithMessage("Şifre boş olamaz.")
                .MinimumLength(3).WithMessage("Şifre en az 3 karakter olmalı");

            RuleFor(e => e.NewPassword)
                .NotEmpty().WithMessage("Şifre boş olamaz.")
                .MinimumLength(3).WithMessage("Şifre en az 3 karakter olmalı");
        }
    }
}
