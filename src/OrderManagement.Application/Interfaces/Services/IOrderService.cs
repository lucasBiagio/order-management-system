using OrderManagement.Application.DTOs.Orders;

namespace OrderManagement.Application.Interfaces.Services;

public interface IOrderService
{
    Task<IReadOnlyCollection<OrderResponse>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<OrderResponse> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<OrderResponse> CreateAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<OrderResponse> UpdateStatusAsync(
    Guid id,
    UpdateOrderStatusRequest request,
    CancellationToken cancellationToken = default);
}