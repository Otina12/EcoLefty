using EcoLefty.Application.Categories.DTOs;
using FluentValidation;

namespace EcoLefty.Application.Categories.Validators;

public class CreateCategoryRequestDtoValidator : AbstractValidator<CreateCategoryRequestDto>
{
    public CreateCategoryRequestDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.");
    }
}
