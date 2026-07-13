using OrderManagement.Domain.Exceptions;

namespace OrderManagement.Domain.Entities;

public sealed class Product
{
    private Product()
    {
    }

    public Product(string name, decimal price, int currentStock)
    {
        Id = Guid.NewGuid();
        Update(name, price, currentStock);
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public decimal Price { get; private set; }

    public int CurrentStock { get; private set; }

    public ICollection<OrderItem> OrderItems { get; private set; } = [];

    public void Update(string name, decimal price, int currentStock)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("O nome do produto é obrigatório.");
        }

        if (price <= 0)
        {
            throw new DomainException("O preço do produto deve ser maior que zero.");
        }

        if (currentStock < 0)
        {
            throw new DomainException("O estoque do produto não pode ser negativo.");
        }

        Name = name.Trim();
        Price = price;
        CurrentStock = currentStock;
    }

    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0)
        {
            throw new DomainException("A quantidade deve ser maior que zero.");
        }

        if (quantity > CurrentStock)
        {
            throw new DomainException(
                $"Estoque insuficiente para o produto '{Name}'. " +
                $"Disponível: {CurrentStock}, solicitado: {quantity}.");
        }

        CurrentStock -= quantity;
    }
}