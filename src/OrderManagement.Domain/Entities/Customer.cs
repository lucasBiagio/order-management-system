using OrderManagement.Domain.Exceptions;

namespace OrderManagement.Domain.Entities;

public sealed class Customer
{
    private Customer()
    {
    }

    public Customer(string name, string email)
    {
        Id = Guid.NewGuid();
        Update(name, email);
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public ICollection<Order> Orders { get; private set; } = [];

    public void Update(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("O nome do cliente é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new DomainException("O e-mail do cliente é obrigatório.");
        }

        Name = name.Trim();
        Email = email.Trim().ToLowerInvariant();
    }
}