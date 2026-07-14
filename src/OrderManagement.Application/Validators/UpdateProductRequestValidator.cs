using FluentValidation;
using OrderManagement.Application.DTOs.Products;

namespace OrderManagement.Application.Validators;

public sealed class UpdateProductRequestValidator
    : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage("O nome do produto é obrigatório.")
            .MaximumLength(150)
            .WithMessage("O nome deve ter no máximo 150 caracteres.");

        RuleFor(request => request.Price)
            .GreaterThan(0)
            .WithMessage("O preço deve ser maior que zero.");

        RuleFor(request => request.CurrentStock)
            .GreaterThanOrEqualTo(0)
            .WithMessage("O estoque não pode ser negativo.");
    }
}