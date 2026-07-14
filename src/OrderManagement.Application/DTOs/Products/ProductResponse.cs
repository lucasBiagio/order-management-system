using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagement.Application.DTOs.Products;

public sealed class ProductResponse
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public decimal Price { get; init; }

    public int CurrentStock { get; init; }
}
