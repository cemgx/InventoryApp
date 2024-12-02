using FluentValidation;
using InventoryApp.Application.LogEntities;

namespace InventoryApp.Validations
{
    public class EmployeeLogValidator : AbstractValidator<EmployeeLog>
    {
        public EmployeeLogValidator()
        {
            RuleFor(e => e.Name)
                .NotEmpty().WithMessage("İsim boş olamaz.")
                .MaximumLength(40).WithMessage("İsim 40 karakterden uzun olamaz");

            RuleFor(e => e.Email)
                .NotEmpty().WithMessage("E-posta boş olamaz.")
                .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz");

            RuleFor(e => e.Password)
                .NotEmpty().WithMessage("Şifre boş olamaz.")
                .MinimumLength(3).WithMessage("Şifre en az 3 karakter olmalı");
        }
    }
}
