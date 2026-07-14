using FluentValidation;
using OrderManagement.Application.DTOs.Customers;

namespace OrderManagement.Application.Validators;

public sealed class CreateCustomerRequestValidator
    : AbstractValidator<CreateCustomerRequest>
{
    public CreateCustomerRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage("O nome do cliente é obrigatório.")
            .MaximumLength(150)
            .WithMessage("O nome deve ter no máximo 150 caracteres.");

        RuleFor(request => request.Email)
            .NotEmpty()
            .WithMessage("O e-mail do cliente é obrigatório.")
            .EmailAddress()
            .WithMessage("O e-mail informado é inválido.")
            .MaximumLength(200)
            .WithMessage("O e-mail deve ter no máximo 200 caracteres.");
    }
}