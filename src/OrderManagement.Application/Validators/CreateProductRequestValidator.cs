using FluentValidation;
using OrderManagement.Application.DTOs.Products;

namespace OrderManagement.Application.Validators;

public sealed class CreateProductRequestValidator
    : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Price)
            .GreaterThan(0);

        RuleFor(x => x.CurrentStock)
            .GreaterThanOrEqualTo(0);
    }
}