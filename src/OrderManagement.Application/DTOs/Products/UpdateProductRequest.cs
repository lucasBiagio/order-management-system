using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagement.Application.DTOs.Products;

public sealed class UpdateProductRequest
{
    public string Name { get; init; } = string.Empty;

    public decimal Price { get; init; }

    public int CurrentStock { get; init; }
}
