using FluentValidation;
using OrderManagement.Application.DTOs.Orders;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.Validators;

public sealed class UpdateOrderStatusRequestValidator
    : AbstractValidator<UpdateOrderStatusRequest>
{
    public UpdateOrderStatusRequestValidator()
    {
        RuleFor(request => request.Status)
            .IsInEnum()
            .WithMessage("O status informado é inválido.")
            .Must(status => status != OrderStatus.Pending)
            .WithMessage("Não é possível alterar manualmente o status para Pendente.");
    }
}