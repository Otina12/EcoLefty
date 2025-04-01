using EcoLefty.Application.Companies.DTOs;
using FluentValidation;

namespace EcoLefty.Application.Companies.Validators;

public class UpdateCompanyRequestDtoValidator : AbstractValidator<UpdateCompanyRequestDto>
{
    public UpdateCompanyRequestDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Company name is required.")
            .MaximumLength(200).WithMessage("Company name cannot exceed 200 characters.");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required.")
            .MaximumLength(100).WithMessage("Country cannot exceed 100 characters.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required.")
            .MaximumLength(100).WithMessage("City cannot exceed 100 characters.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required.")
            .MaximumLength(300).WithMessage("Address cannot exceed 300 characters.");
    }
}