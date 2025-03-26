using EcoLefty.Application.Products.DTOs;
using FluentValidation;

namespace EcoLefty.Application.Products.Validators;

public class UpdateProductRequestDtoValidator : AbstractValidator<UpdateProductRequestDto>
{
    public UpdateProductRequestDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.DefaultPrice)
            .GreaterThan(0).WithMessage("Default price must be greater than 0.");

        RuleFor(x => x.ImageUrl)
            .NotEmpty().WithMessage("Image URL is required.")
            //.Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
            .WithMessage("Image URL must be a valid absolute URL.");
    }
}