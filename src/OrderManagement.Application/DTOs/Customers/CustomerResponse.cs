using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagement.Application.DTOs.Customers;

public sealed class CustomerResponse
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;
}
