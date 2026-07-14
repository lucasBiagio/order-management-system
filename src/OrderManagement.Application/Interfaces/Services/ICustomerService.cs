using OrderManagement.Application.DTOs.Customers;

namespace OrderManagement.Application.Interfaces.Services;

public interface ICustomerService
{
    Task<IReadOnlyCollection<CustomerResponse>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<CustomerResponse> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<CustomerResponse> CreateAsync(
        CreateCustomerRequest request,
        CancellationToken cancellationToken = default);

    Task<CustomerResponse> UpdateAsync(
        Guid id,
        UpdateCustomerRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}