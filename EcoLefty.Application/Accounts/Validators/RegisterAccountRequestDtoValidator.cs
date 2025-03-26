using EcoLefty.Application.Accounts.DTOs;
using FluentValidation;

namespace EcoLefty.Application.Accounts.Validators;

public class RegisterAccountRequestDtoValidator : AbstractValidator<RegisterAccountRequestDto>
{
    public RegisterAccountRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.");
    }
}