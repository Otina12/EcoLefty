using EcoLefty.Application.ApplicationUsers.DTOs;
using FluentValidation;

namespace EcoLefty.Application.ApplicationUsers.Validators;

public class CreateApplicationUserRequestDtoValidator : AbstractValidator<CreateApplicationUserRequestDto>
{
    public CreateApplicationUserRequestDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("Passwords do not match.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.");

        RuleFor(x => x.Bio)
            .MaximumLength(1000).WithMessage("Bio cannot exceed 1000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Bio));

        RuleFor(x => x.BirthDay)
            .InclusiveBetween(1, 31).WithMessage("Birth day must be between 1 and 31.");

        RuleFor(x => x.BirthMonth)
            .InclusiveBetween(1, 12).WithMessage("Birth month must be between 1 and 12.");

        RuleFor(x => x.BirthYear)
            .InclusiveBetween(1900, DateTime.Now.Year)
            .WithMessage($"Birth year must be between 1900 and {DateTime.Now.Year}.");

        //RuleFor(x => x.ProfilePictureUrl)
        //    .Must(url => string.IsNullOrEmpty(url))
        //    .WithMessage("Profile picture URL must be a valid absolute URL.");
    }
}