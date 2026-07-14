using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Application.Common.Mapping;
using OrderManagement.Application.DTOs.Customers;
using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Application.Services;
using OrderManagement.Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;

namespace OrderManagement.UnitTests.Services;

public sealed class CustomerServiceTests
{
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly Mock<IValidator<CreateCustomerRequest>> _createValidatorMock;
    private readonly Mock<IValidator<UpdateCustomerRequest>> _updateValidatorMock;
    private readonly CustomerService _customerService;

    public CustomerServiceTests()
    {
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _createValidatorMock = new Mock<IValidator<CreateCustomerRequest>>();
        _updateValidatorMock = new Mock<IValidator<UpdateCustomerRequest>>();

        var mapperConfiguration = new MapperConfiguration(
    configuration => configuration.AddProfile<CustomerProfile>(),
    NullLoggerFactory.Instance);

        var mapper = mapperConfiguration.CreateMapper();

        _customerService = new CustomerService(
            _customerRepositoryMock.Object,
            mapper,
            _createValidatorMock.Object,
            _updateValidatorMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_ShouldCreateCustomer()
    {
        // Arrange
        var request = new CreateCustomerRequest
        {
            Name = "Lucas Biagio",
            Email = "LUCAS@EMAIL.COM"
        };

        ConfigureSuccessfulValidation(request);

        _customerRepositoryMock
            .Setup(repository => repository.ExistsByEmailAsync(
                "lucas@email.com",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var response = await _customerService.CreateAsync(request);

        // Assert
        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal("Lucas Biagio", response.Name);
        Assert.Equal("lucas@email.com", response.Email);

        _customerRepositoryMock.Verify(
            repository => repository.AddAsync(
                It.Is<Customer>(customer =>
                    customer.Name == "Lucas Biagio" &&
                    customer.Email == "lucas@email.com"),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _customerRepositoryMock.Verify(
            repository => repository.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicatedEmail_ShouldThrowConflictException()
    {
        // Arrange
        var request = new CreateCustomerRequest
        {
            Name = "Lucas Biagio",
            Email = "lucas@email.com"
        };

        ConfigureSuccessfulValidation(request);

        _customerRepositoryMock
            .Setup(repository => repository.ExistsByEmailAsync(
                request.Email,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var action = () => _customerService.CreateAsync(request);

        // Assert
        var exception = await Assert.ThrowsAsync<ConflictException>(action);

        Assert.Contains("Já existe um cliente", exception.Message);

        _customerRepositoryMock.Verify(
            repository => repository.AddAsync(
                It.IsAny<Customer>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _customerRepositoryMock.Verify(
            repository => repository.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCustomerDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        _customerRepositoryMock
            .Setup(repository => repository.GetByIdAsync(
                customerId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        // Act
        var action = () => _customerService.GetByIdAsync(customerId);

        // Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(action);

        Assert.Contains(customerId.ToString(), exception.Message);
    }

    private void ConfigureSuccessfulValidation(
        CreateCustomerRequest request)
    {
        _createValidatorMock
            .Setup(validator => validator.ValidateAsync(
                request,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }
}