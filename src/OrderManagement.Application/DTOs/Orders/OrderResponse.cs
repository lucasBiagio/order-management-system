using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.DTOs.Orders;

public sealed class OrderResponse
{
    public Guid Id { get; init; }

    public Guid CustomerId { get; init; }

    public DateTime OrderDate { get; init; }

    public OrderStatus Status { get; init; }

    public decimal TotalAmount { get; init; }

    public IReadOnlyCollection<OrderItemResponse> Items { get; init; }
        = [];
}