using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.DTOs.Orders;

public sealed class UpdateOrderStatusRequest
{
    public OrderStatus Status { get; init; }
}