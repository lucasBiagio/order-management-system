using OrderManagement.Domain.Enums;
using OrderManagement.Domain.Exceptions;

namespace OrderManagement.Domain.Entities;

public sealed class Order
{
    private readonly List<OrderItem> _items = [];

    private Order()
    {
    }

    public Order(Guid customerId)
    {
        if (customerId == Guid.Empty)
        {
            throw new DomainException("O cliente do pedido é obrigatório.");
        }

        Id = Guid.NewGuid();
        CustomerId = customerId;
        OrderDate = DateTime.UtcNow;
        Status = OrderStatus.Pending;
    }

    public Guid Id { get; private set; }

    public Guid CustomerId { get; private set; }

    public DateTime OrderDate { get; private set; }

    public OrderStatus Status { get; private set; }

    public decimal TotalAmount { get; private set; }

    public Customer Customer { get; private set; } = null!;

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public void AddItem(Product product, int quantity)
    {
        ArgumentNullException.ThrowIfNull(product);

        product.DecreaseStock(quantity);

        var item = new OrderItem(
            product.Id,
            quantity,
            product.Price);

        _items.Add(item);

        CalculateTotal();
    }

    public void ChangeStatus(OrderStatus newStatus)
    {
        if (Status == OrderStatus.Pending && newStatus == OrderStatus.Paid)
        {
            Status = newStatus;
            return;
        }

        if (Status == OrderStatus.Paid && newStatus == OrderStatus.Cancelled)
        {
            Status = newStatus;
            return;
        }

        throw new DomainException(
            $"Não é permitido alterar o status de '{Status}' para '{newStatus}'.");
    }

    private void CalculateTotal()
    {
        TotalAmount = _items.Sum(item => item.Total);
    }
}