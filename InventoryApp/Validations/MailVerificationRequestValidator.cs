using FluentValidation;
using InventoryApp.Application.Dto;

namespace InventoryApp.Validations
{
    public class MailVerificationRequestValidator : AbstractValidator<MailVerificationRequestDto>
    {
        public MailVerificationRequestValidator()
        {
            RuleFor(e => e.VerificationCode)
                .NotEmpty().WithMessage("Kod alanı boş olamaz.")
                .Length(6).WithMessage("Kod en az 6 karakter olmalı");

            RuleFor(e => e.Email)
                .NotEmpty().WithMessage("E-posta boş olamaz.")
                .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz");
        }
    }
}
