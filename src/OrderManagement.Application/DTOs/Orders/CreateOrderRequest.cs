using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagement.Application.DTOs.Orders;

public sealed class CreateOrderRequest
{
    public Guid CustomerId { get; init; }

    public List<CreateOrderItemRequest> Items { get; init; } = [];
}
