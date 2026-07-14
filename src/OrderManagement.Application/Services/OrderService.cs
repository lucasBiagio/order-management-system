using AutoMapper;
using FluentValidation;
using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Application.DTOs.Orders;
using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Application.Interfaces.Services;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Services;

public sealed class OrderService : IOrderService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateOrderRequest> _createValidator;

    public OrderService(
        ICustomerRepository customerRepository,
        IProductRepository productRepository,
        IOrderRepository orderRepository,
        IMapper mapper,
        IValidator<CreateOrderRequest> createValidator)
    {
        _customerRepository = customerRepository;
        _productRepository = productRepository;
        _orderRepository = orderRepository;
        _mapper = mapper;
        _createValidator = createValidator;
    }

    public async Task<IReadOnlyCollection<OrderResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.GetAllAsync(cancellationToken);

        return _mapper.Map<IReadOnlyCollection<OrderResponse>>(orders);
    }

    public async Task<OrderResponse> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (order is null)
        {
            throw new NotFoundException(
                $"Pedido com o identificador '{id}' não foi encontrado.");
        }

        return _mapper.Map<OrderResponse>(order);
    }

    public async Task<OrderResponse> CreateAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        await _createValidator.ValidateAndThrowAsync(
            request,
            cancellationToken);

        var customer = await _customerRepository.GetByIdAsync(
            request.CustomerId,
            cancellationToken);

        if (customer is null)
        {
            throw new NotFoundException(
                $"Cliente com o identificador " +
                $"'{request.CustomerId}' não foi encontrado.");
        }

        var requestedProductIds = request.Items
            .Select(item => item.ProductId)
            .ToArray();

        var products = await _productRepository.GetByIdsAsync(
            requestedProductIds,
            cancellationToken);

        ValidateAllProductsExist(request, products);

        ValidateAvailableStock(request, products);

        var productsById = products.ToDictionary(product => product.Id);

        var order = new Order(request.CustomerId);

        foreach (var itemRequest in request.Items)
        {
            var product = productsById[itemRequest.ProductId];

            order.AddItem(
                product,
                itemRequest.Quantity);
        }

        await _orderRepository.AddAsync(
            order,
            cancellationToken);

        // Pedido, itens e alterações de estoque são persistidos juntos.
        await _orderRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<OrderResponse>(order);
    }

    private static void ValidateAllProductsExist(
        CreateOrderRequest request,
        IReadOnlyCollection<Product> products)
    {
        var foundProductIds = products
            .Select(product => product.Id)
            .ToHashSet();

        var missingProductIds = request.Items
            .Select(item => item.ProductId)
            .Where(productId => !foundProductIds.Contains(productId))
            .Distinct()
            .ToArray();

        if (missingProductIds.Length == 0)
        {
            return;
        }

        throw new NotFoundException(
            $"Os seguintes produtos não foram encontrados: " +
            $"{string.Join(", ", missingProductIds)}.");
    }

    private static void ValidateAvailableStock(
        CreateOrderRequest request,
        IReadOnlyCollection<Product> products)
    {
        var requestedQuantities = request.Items.ToDictionary(
            item => item.ProductId,
            item => item.Quantity);

        foreach (var product in products)
        {
            var requestedQuantity = requestedQuantities[product.Id];

            if (requestedQuantity > product.CurrentStock)
            {
                throw new ConflictException(
                    $"Estoque insuficiente para o produto '{product.Name}'. " +
                    $"Disponível: {product.CurrentStock}; " +
                    $"solicitado: {requestedQuantity}.");
            }
        }
    }
}