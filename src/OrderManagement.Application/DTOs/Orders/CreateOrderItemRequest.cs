using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagement.Application.DTOs.Orders;

public sealed class CreateOrderItemRequest
{
    public Guid ProductId { get; init; }

    public int Quantity { get; init; }
}
