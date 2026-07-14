using AutoMapper;
using FluentValidation;
using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Application.DTOs.Products;
using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Application.Interfaces.Services;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Services;

public sealed class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateProductRequest> _createValidator;
    private readonly IValidator<UpdateProductRequest> _updateValidator;

    public ProductService(
        IProductRepository productRepository,
        IMapper mapper,
        IValidator<CreateProductRequest> createValidator,
        IValidator<UpdateProductRequest> updateValidator)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IReadOnlyCollection<ProductResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetAllAsync(cancellationToken);

        return _mapper.Map<IReadOnlyCollection<ProductResponse>>(products);
    }

    public async Task<ProductResponse> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var product = await GetProductOrThrowAsync(id, cancellationToken);

        return _mapper.Map<ProductResponse>(product);
    }

    public async Task<ProductResponse> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        await _createValidator.ValidateAndThrowAsync(
            request,
            cancellationToken);

        var normalizedName = request.Name.Trim();

        if (await _productRepository.ExistsByNameAsync(
                normalizedName,
                cancellationToken))
        {
            throw new ConflictException(
                $"Já existe um produto cadastrado com o nome '{normalizedName}'.");
        }

        var product = new Product(
            normalizedName,
            request.Price,
            request.CurrentStock);

        await _productRepository.AddAsync(product, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ProductResponse>(product);
    }

    public async Task<ProductResponse> UpdateAsync(
        Guid id,
        UpdateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        await _updateValidator.ValidateAndThrowAsync(
            request,
            cancellationToken);

        var product = await GetProductOrThrowAsync(id, cancellationToken);
        var normalizedName = request.Name.Trim();

        var productWithSameName =
            await _productRepository.GetByNameAsync(
                normalizedName,
                cancellationToken);

        if (productWithSameName is not null &&
            productWithSameName.Id != product.Id)
        {
            throw new ConflictException(
                $"Já existe um produto cadastrado com o nome '{normalizedName}'.");
        }

        product.ChangeName(normalizedName);
        product.ChangePrice(request.Price);
        product.ChangeStock(request.CurrentStock);

        _productRepository.Update(product);
        await _productRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ProductResponse>(product);
    }

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var product = await GetProductOrThrowAsync(id, cancellationToken);

        _productRepository.Remove(product);
        await _productRepository.SaveChangesAsync(cancellationToken);
    }

    private async Task<Product> GetProductOrThrowAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (product is null)
        {
            throw new NotFoundException(
                $"Produto com o identificador '{id}' não foi encontrado.");
        }

        return product;
    }
}