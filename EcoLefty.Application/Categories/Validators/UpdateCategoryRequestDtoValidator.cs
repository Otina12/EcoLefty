using EcoLefty.Application.Categories.DTOs;
using FluentValidation;

namespace EcoLefty.Application.Categories.Validators;

public class UpdateCategoryRequestDtoValidator : AbstractValidator<UpdateCategoryRequestDto>
{
    public UpdateCategoryRequestDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.");
    }
}
