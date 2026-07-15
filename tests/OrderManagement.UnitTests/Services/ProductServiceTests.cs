using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Application.Common.Mapping;
using OrderManagement.Application.DTOs.Customers;
using OrderManagement.Application.DTOs.Products;
using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Application.Interfaces.Services;
using OrderManagement.Application.Services;
using OrderManagement.Domain.Entities;

namespace OrderManagement.UnitTests.Services;

public sealed class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IValidator<CreateProductRequest>> _createValidatorMock;
    private readonly Mock<IValidator<UpdateProductRequest>> _updateValidatorMock;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _createValidatorMock = new Mock<IValidator<CreateProductRequest>>();
        _updateValidatorMock = new Mock<IValidator<UpdateProductRequest>>();

        var mapperConfiguration = new MapperConfiguration(
        configuration =>
        {
            configuration.AddProfile<ProductProfile>();
        },
        NullLoggerFactory.Instance);

        mapperConfiguration.AssertConfigurationIsValid();

        var mapper = mapperConfiguration.CreateMapper();

        _productService = new ProductService(
            _productRepositoryMock.Object,
            mapper,
            _createValidatorMock.Object,
            _updateValidatorMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_ShouldCreateProduct()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Notebook Dell",
            Price = 5000,
            CurrentStock = 10
        };

        ConfigureSuccessfulValidation(request);

        _productRepositoryMock
            .Setup(repository => repository.ExistsByNameAsync(
                "Notebook Dell",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var response = await _productService.CreateAsync(request);

        // Assert
        Assert.Equal("Notebook Dell", response.Name);
        Assert.Equal(5000, response.Price);
        Assert.Equal(10, response.CurrentStock);

        _productRepositoryMock.Verify(
            repository => repository.AddAsync(
                It.IsAny<Product>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _productRepositoryMock.Verify(
            repository => repository.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicatedName_ShouldThrowConflictException()
    {
        var request = new CreateProductRequest
        {
            Name = "Notebook Dell",
            Price = 5000,
            CurrentStock = 10
        };

        ConfigureSuccessfulValidation(request);

        _productRepositoryMock
            .Setup(repository => repository.ExistsByNameAsync(
                request.Name,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<ConflictException>(
            () => _productService.CreateAsync(request));

        _productRepositoryMock.Verify(
            repository => repository.AddAsync(
                It.IsAny<Product>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductDoesNotExist_ShouldThrowNotFoundException()
    {
        var id = Guid.NewGuid();

        _productRepositoryMock
            .Setup(repository => repository.GetByIdAsync(
                id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _productService.GetByIdAsync(id));
    }

    [Fact]
    public void IncreaseStock_ShouldRestoreCancelledOrderQuantity()
    {
        // Arrange
        var product = new Product(
            name: "Monitor",
            price: 899.90m,
            currentStock: 5);

        product.DecreaseStock(3);

        // Act
        product.IncreaseStock(3);

        // Assert
        Assert.Equal(5, product.CurrentStock);
    }

    private void ConfigureSuccessfulValidation(
        CreateProductRequest request)
    {
        _createValidatorMock
            .Setup(validator => validator.ValidateAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }
}