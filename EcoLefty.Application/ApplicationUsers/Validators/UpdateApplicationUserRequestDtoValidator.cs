using EcoLefty.Application.ApplicationUsers.DTOs;
using FluentValidation;

namespace EcoLefty.Application.ApplicationUsers.Validators;

public class UpdateApplicationUserRequestDtoValidator : AbstractValidator<UpdateApplicationUserRequestDto>
{
    public UpdateApplicationUserRequestDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters.");

        RuleFor(x => x.Bio)
            .MaximumLength(1000).WithMessage("Bio cannot exceed 1000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Bio));

        //RuleFor(x => x.ProfilePictureUrl)
        //    .Must(url => string.IsNullOrEmpty(url) || Uri.IsWellFormedUriString(url, UriKind.Absolute))
        //    .WithMessage("Profile picture URL must be a valid absolute URL.");
    }
}