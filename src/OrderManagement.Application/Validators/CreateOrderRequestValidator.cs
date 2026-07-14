using FluentValidation;
using OrderManagement.Application.DTOs.Orders;

namespace OrderManagement.Application.Validators;

public sealed class CreateOrderRequestValidator
    : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(order => order.CustomerId)
            .NotEmpty()
            .WithMessage("O cliente do pedido é obrigatório.");

        RuleFor(order => order.Items)
            .NotEmpty()
            .WithMessage("O pedido deve possuir pelo menos um item.");

        RuleForEach(order => order.Items)
            .SetValidator(new CreateOrderItemRequestValidator());

        RuleFor(order => order.Items)
            .Must(items =>
                items.Select(item => item.ProductId).Distinct().Count()
                == items.Count)
            .WithMessage(
                "Um produto não pode aparecer mais de uma vez no pedido.");
    }
}