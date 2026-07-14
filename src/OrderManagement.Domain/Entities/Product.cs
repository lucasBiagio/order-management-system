using OrderManagement.Domain.Exceptions;

namespace OrderManagement.Domain.Entities;

public sealed class Product
{
    private Product()
    {
    }

    public Product(
        string name,
        decimal price,
        int currentStock)
    {
        Id = Guid.NewGuid();

        ChangeName(name);
        ChangePrice(price);
        ChangeStock(currentStock);
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public decimal Price { get; private set; }

    public int CurrentStock { get; private set; }

    public ICollection<OrderItem> OrderItems { get; private set; } = [];

    public void ChangeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(
                "O nome do produto é obrigatório.");

        Name = name.Trim();
    }

    public void ChangePrice(decimal price)
    {
        if (price <= 0)
            throw new DomainException(
                "O preço deve ser maior que zero.");

        Price = price;
    }

    public void ChangeStock(int currentStock)
    {
        if (currentStock < 0)
            throw new DomainException(
                "O estoque não pode ser negativo.");

        CurrentStock = currentStock;
    }

    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException(
                "A quantidade deve ser maior que zero.");

        if (quantity > CurrentStock)
            throw new DomainException(
                $"Estoque insuficiente para o produto '{Name}'.");

        CurrentStock -= quantity;
    }

    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException(
                "A quantidade deve ser maior que zero.");

        CurrentStock += quantity;
    }
}