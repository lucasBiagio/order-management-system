using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Application.DTOs.Customers;
using OrderManagement.Application.DTOs.Orders;
using OrderManagement.Application.DTOs.Products;
using OrderManagement.Domain.Enums;
using OrderManagement.Infrastructure.Persistence;
using OrderManagement.IntegrationTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace OrderManagement.IntegrationTests.Orders;

public sealed class CreateOrderTests
    : IClassFixture<OrderManagementApiFactory>
{
    private readonly OrderManagementApiFactory _factory;
    private readonly HttpClient _client;

    public CreateOrderTests(OrderManagementApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateOrder_WithValidData_ShouldPersistOrderAndDecreaseStock()
    {
        // Arrange
        var customerRequest = new CreateCustomerRequest
        {
            Name = "Cliente Integração",
            Email = "integracao@email.com"
        };

        var customerResponse = await _client.PostAsJsonAsync(
            "/api/customers",
            customerRequest);

        Assert.Equal(HttpStatusCode.Created, customerResponse.StatusCode);

        var customer = await customerResponse.Content
            .ReadFromJsonAsync<CustomerResponse>();

        Assert.NotNull(customer);

        var productRequest = new CreateProductRequest
        {
            Name = "Produto Integração",
            Price = 100m,
            CurrentStock = 5
        };

        var productResponse = await _client.PostAsJsonAsync(
            "/api/products",
            productRequest);

        Assert.Equal(HttpStatusCode.Created, productResponse.StatusCode);

        var product = await productResponse.Content
            .ReadFromJsonAsync<ProductResponse>();

        Assert.NotNull(product);

        var orderRequest = new CreateOrderRequest
        {
            CustomerId = customer.Id,
            Items =
            [
                new CreateOrderItemRequest
                {
                    ProductId = product.Id,
                    Quantity = 3
                }
            ]
        };

        // Act
        var orderResponse = await _client.PostAsJsonAsync(
            "/api/orders",
            orderRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Created, orderResponse.StatusCode);

        var order = await orderResponse.Content
            .ReadFromJsonAsync<OrderResponse>();

        Assert.NotNull(order);
        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Equal(300m, order.TotalAmount);
        Assert.Single(order.Items);

        var updatedProduct = await _client.GetFromJsonAsync<ProductResponse>(
            $"/api/products/{product.Id}");

        Assert.NotNull(updatedProduct);
        Assert.Equal(2, updatedProduct.CurrentStock);

        using var scope = _factory.Services.CreateScope();

        var dbContext =
            scope.ServiceProvider.GetRequiredService<OrderManagementDbContext>();

        var persistedOrder = await dbContext.Orders.FindAsync(order.Id);

        Assert.NotNull(persistedOrder);
        Assert.Equal(300m, persistedOrder.TotalAmount);
    }

    [Fact]
    public async Task CreateOrder_WithInsufficientStock_ShouldReturnConflict()
    {
        // Arrange
        var suffix = Guid.NewGuid().ToString("N");

        var customerResponse = await _client.PostAsJsonAsync(
            "/api/customers",
            new CreateCustomerRequest
            {
                Name = $"Cliente Estoque {suffix}",
                Email = $"estoque-{suffix}@email.com"
            });

        Assert.Equal(HttpStatusCode.Created, customerResponse.StatusCode);

        var customer = await customerResponse.Content
            .ReadFromJsonAsync<CustomerResponse>();

        Assert.NotNull(customer);

        var productResponse = await _client.PostAsJsonAsync(
            "/api/products",
            new CreateProductRequest
            {
                Name = $"Produto Estoque {suffix}",
                Price = 100m,
                CurrentStock = 5
            });

        Assert.Equal(HttpStatusCode.Created, productResponse.StatusCode);

        var product = await productResponse.Content
            .ReadFromJsonAsync<ProductResponse>();

        Assert.NotNull(product);

        var firstOrderResponse = await _client.PostAsJsonAsync(
            "/api/orders",
            new CreateOrderRequest
            {
                CustomerId = customer.Id,
                Items =
                [
                    new CreateOrderItemRequest
                {
                    ProductId = product.Id,
                    Quantity = 3
                }
                ]
            });

        Assert.Equal(HttpStatusCode.Created, firstOrderResponse.StatusCode);

        // Act
        var secondOrderResponse = await _client.PostAsJsonAsync(
            "/api/orders",
            new CreateOrderRequest
            {
                CustomerId = customer.Id,
                Items =
                [
                    new CreateOrderItemRequest
                {
                    ProductId = product.Id,
                    Quantity = 4
                }
                ]
            });

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, secondOrderResponse.StatusCode);

        var problem = await secondOrderResponse.Content
            .ReadFromJsonAsync<ProblemDetails>();

        Assert.NotNull(problem);
        Assert.Equal("Conflito", problem.Title);
        Assert.Contains("Estoque insuficiente", problem.Detail);

        var updatedProduct = await _client.GetFromJsonAsync<ProductResponse>(
            $"/api/products/{product.Id}");

        Assert.NotNull(updatedProduct);
        Assert.Equal(2, updatedProduct.CurrentStock);
    }

    [Fact]
    public async Task CancelPendingOrder_ShouldRestoreProductStock()
    {
        // Arrange
        var suffix = Guid.NewGuid().ToString("N");

        var customerResponse = await _client.PostAsJsonAsync(
            "/api/customers",
            new CreateCustomerRequest
            {
                Name = $"Cliente Cancelamento {suffix}",
                Email = $"cancelamento-{suffix}@email.com"
            });

        Assert.Equal(HttpStatusCode.Created, customerResponse.StatusCode);

        var customer = await customerResponse.Content
            .ReadFromJsonAsync<CustomerResponse>();

        Assert.NotNull(customer);

        var productResponse = await _client.PostAsJsonAsync(
            "/api/products",
            new CreateProductRequest
            {
                Name = $"Produto Cancelamento {suffix}",
                Price = 100m,
                CurrentStock = 5
            });

        Assert.Equal(HttpStatusCode.Created, productResponse.StatusCode);

        var product = await productResponse.Content
            .ReadFromJsonAsync<ProductResponse>();

        Assert.NotNull(product);

        var createOrderResponse = await _client.PostAsJsonAsync(
            "/api/orders",
            new CreateOrderRequest
            {
                CustomerId = customer.Id,
                Items =
                [
                    new CreateOrderItemRequest
                {
                    ProductId = product.Id,
                    Quantity = 3
                }
                ]
            });

        Assert.Equal(HttpStatusCode.Created, createOrderResponse.StatusCode);

        var createdOrder = await createOrderResponse.Content
            .ReadFromJsonAsync<OrderResponse>();

        Assert.NotNull(createdOrder);
        Assert.Equal(OrderStatus.Pending, createdOrder.Status);

        var productAfterOrder = await _client.GetFromJsonAsync<ProductResponse>(
            $"/api/products/{product.Id}");

        Assert.NotNull(productAfterOrder);
        Assert.Equal(2, productAfterOrder.CurrentStock);

        // Act
        var cancelResponse = await _client.PatchAsJsonAsync(
            $"/api/orders/{createdOrder.Id}/status",
            new UpdateOrderStatusRequest
            {
                Status = OrderStatus.Cancelled
            });

        // Assert
        Assert.Equal(HttpStatusCode.OK, cancelResponse.StatusCode);

        var cancelledOrder = await cancelResponse.Content
            .ReadFromJsonAsync<OrderResponse>();

        Assert.NotNull(cancelledOrder);
        Assert.Equal(OrderStatus.Cancelled, cancelledOrder.Status);

        var productAfterCancellation =
            await _client.GetFromJsonAsync<ProductResponse>(
                $"/api/products/{product.Id}");

        Assert.NotNull(productAfterCancellation);
        Assert.Equal(5, productAfterCancellation.CurrentStock);
    }

    [Fact]
    public async Task CancelAlreadyCancelledOrder_ShouldNotRestoreStockTwice()
    {
        // Arrange
        var suffix = Guid.NewGuid().ToString("N");

        var customerResponse = await _client.PostAsJsonAsync(
            "/api/customers",
            new CreateCustomerRequest
            {
                Name = $"Cliente Duplo Cancelamento {suffix}",
                Email = $"duplo-cancelamento-{suffix}@email.com"
            });

        Assert.Equal(HttpStatusCode.Created, customerResponse.StatusCode);

        var customer = await customerResponse.Content
            .ReadFromJsonAsync<CustomerResponse>();

        Assert.NotNull(customer);

        var productResponse = await _client.PostAsJsonAsync(
            "/api/products",
            new CreateProductRequest
            {
                Name = $"Produto Duplo Cancelamento {suffix}",
                Price = 100m,
                CurrentStock = 5
            });

        Assert.Equal(HttpStatusCode.Created, productResponse.StatusCode);

        var product = await productResponse.Content
            .ReadFromJsonAsync<ProductResponse>();

        Assert.NotNull(product);

        var createOrderResponse = await _client.PostAsJsonAsync(
            "/api/orders",
            new CreateOrderRequest
            {
                CustomerId = customer.Id,
                Items =
                [
                    new CreateOrderItemRequest
                {
                    ProductId = product.Id,
                    Quantity = 3
                }
                ]
            });

        Assert.Equal(HttpStatusCode.Created, createOrderResponse.StatusCode);

        var createdOrder = await createOrderResponse.Content
            .ReadFromJsonAsync<OrderResponse>();

        Assert.NotNull(createdOrder);

        var firstCancellationResponse = await _client.PatchAsJsonAsync(
            $"/api/orders/{createdOrder.Id}/status",
            new UpdateOrderStatusRequest
            {
                Status = OrderStatus.Cancelled
            });

        Assert.Equal(
            HttpStatusCode.OK,
            firstCancellationResponse.StatusCode);

        // Act
        var secondCancellationResponse = await _client.PatchAsJsonAsync(
            $"/api/orders/{createdOrder.Id}/status",
            new UpdateOrderStatusRequest
            {
                Status = OrderStatus.Cancelled
            });

        // Assert
        Assert.Equal(
            HttpStatusCode.BadRequest,
            secondCancellationResponse.StatusCode);

        var productAfterSecondCancellation =
            await _client.GetFromJsonAsync<ProductResponse>(
                $"/api/products/{product.Id}");

        Assert.NotNull(productAfterSecondCancellation);
        Assert.Equal(5, productAfterSecondCancellation.CurrentStock);
    }
}