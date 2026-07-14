using AutoMapper;
using FluentValidation;
using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Application.DTOs.Customers;
using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Application.Interfaces.Services;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Services;

public sealed class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateCustomerRequest> _createValidator;
    private readonly IValidator<UpdateCustomerRequest> _updateValidator;

    public CustomerService(
        ICustomerRepository customerRepository,
        IMapper mapper,
        IValidator<CreateCustomerRequest> createValidator,
        IValidator<UpdateCustomerRequest> updateValidator)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IReadOnlyCollection<CustomerResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var customers = await _customerRepository.GetAllAsync(cancellationToken);

        return _mapper.Map<IReadOnlyCollection<CustomerResponse>>(customers);
    }

    public async Task<CustomerResponse> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var customer = await GetCustomerOrThrowAsync(id, cancellationToken);

        return _mapper.Map<CustomerResponse>(customer);
    }

    public async Task<CustomerResponse> CreateAsync(
        CreateCustomerRequest request,
        CancellationToken cancellationToken = default)
    {
        await _createValidator.ValidateAndThrowAsync(
            request,
            cancellationToken);

        var normalizedEmail = NormalizeEmail(request.Email);

        if (await _customerRepository.ExistsByEmailAsync(
                normalizedEmail,
                cancellationToken))
        {
            throw new ConflictException(
                $"Já existe um cliente cadastrado com o e-mail '{normalizedEmail}'.");
        }

        var customer = new Customer(
            request.Name,
            normalizedEmail);

        await _customerRepository.AddAsync(
            customer,
            cancellationToken);

        await _customerRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CustomerResponse>(customer);
    }

    public async Task<CustomerResponse> UpdateAsync(
        Guid id,
        UpdateCustomerRequest request,
        CancellationToken cancellationToken = default)
    {
        await _updateValidator.ValidateAndThrowAsync(
            request,
            cancellationToken);

        var customer = await GetCustomerOrThrowAsync(
            id,
            cancellationToken);

        var normalizedEmail = NormalizeEmail(request.Email);

        var customerWithSameEmail =
            await _customerRepository.GetByEmailAsync(
                normalizedEmail,
                cancellationToken);

        if (customerWithSameEmail is not null &&
            customerWithSameEmail.Id != customer.Id)
        {
            throw new ConflictException(
                $"Já existe um cliente cadastrado com o e-mail '{normalizedEmail}'.");
        }

        customer.Update(
            request.Name,
            normalizedEmail);

        _customerRepository.Update(customer);

        await _customerRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CustomerResponse>(customer);
    }

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var customer = await GetCustomerOrThrowAsync(
            id,
            cancellationToken);

        _customerRepository.Remove(customer);

        await _customerRepository.SaveChangesAsync(cancellationToken);
    }

    private async Task<Customer> GetCustomerOrThrowAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (customer is null)
        {
            throw new NotFoundException(
                $"Cliente com o identificador '{id}' não foi encontrado.");
        }

        return customer;
    }

    private static string NormalizeEmail(string email)
    {
        return email.Trim().ToLowerInvariant();
    }
}