using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Interfaces.Repositories;

public interface ICustomerRepository
{
    Task<IReadOnlyCollection<Customer>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);

    void Update(Customer customer);

    void Remove(Customer customer);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}