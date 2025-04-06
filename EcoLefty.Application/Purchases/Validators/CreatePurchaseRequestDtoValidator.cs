using EcoLefty.Application.Purchases.DTOs;
using FluentValidation;

namespace EcoLefty.Application.Purchases.Validators;

public class CreatePurchaseRequestDtoValidator : AbstractValidator<CreatePurchaseRequestDto>
{
    public CreatePurchaseRequestDtoValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Purchase quantity must be greater than 0.");

        RuleFor(x => x.OfferId)
            .NotEmpty().WithMessage("Offer Id is required.");
    }
}