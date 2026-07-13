using OrderManagement.Domain.Exceptions;

namespace OrderManagement.Domain.Entities;

public sealed class OrderItem
{
    private OrderItem()
    {
    }

    internal OrderItem(Guid productId, int quantity, decimal unitPrice)
    {
        if (productId == Guid.Empty)
        {
            throw new DomainException("O produto do item é obrigatório.");
        }

        if (quantity <= 0)
        {
            throw new DomainException("A quantidade do item deve ser maior que zero.");
        }

        if (unitPrice <= 0)
        {
            throw new DomainException("O preço unitário deve ser maior que zero.");
        }

        Id = Guid.NewGuid();
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public Guid Id { get; private set; }

    public Guid OrderId { get; private set; }

    public Guid ProductId { get; private set; }

    public int Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    public decimal Total => Quantity * UnitPrice;

    public Order Order { get; private set; } = null!;

    public Product Product { get; private set; } = null!;
}