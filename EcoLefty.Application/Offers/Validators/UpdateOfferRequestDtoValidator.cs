﻿using EcoLefty.Application.Offers.DTOs;
using FluentValidation;

namespace EcoLefty.Application.Offers.Validators;

public class UpdateOfferRequestDtoValidator : AbstractValidator<UpdateOfferRequestDto>
{
    public UpdateOfferRequestDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0).WithMessage("Unit price must be greater than 0.");

        RuleFor(x => x.TotalQuantity)
            .GreaterThan(0).WithMessage("Total quantity must be greater than 0.");

        RuleFor(x => x.StartDateUtc)
            .LessThan(x => x.ExpiryDateUtc)
            .WithMessage("Start date must be before the expiry date.");
    }
}