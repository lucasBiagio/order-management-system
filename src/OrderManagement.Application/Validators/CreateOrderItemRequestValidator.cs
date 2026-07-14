using FluentValidation;
using OrderManagement.Application.DTOs.Orders;

namespace OrderManagement.Application.Validators;

public sealed class CreateOrderItemRequestValidator
    : AbstractValidator<CreateOrderItemRequest>
{
    public CreateOrderItemRequestValidator()
    {
        RuleFor(item => item.ProductId)
            .NotEmpty();

        RuleFor(item => item.Quantity)
            .GreaterThan(0);
    }
}