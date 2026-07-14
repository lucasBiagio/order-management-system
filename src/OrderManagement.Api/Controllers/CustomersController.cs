using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.DTOs.Customers;
using OrderManagement.Application.Interfaces.Services;

namespace OrderManagement.Api.Controllers;

[ApiController]
[Route("api/customers")]
public sealed class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyCollection<CustomerResponse>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<CustomerResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var customers =
            await _customerService.GetAllAsync(cancellationToken);

        return Ok(customers);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(
        typeof(CustomerResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var customer =
            await _customerService.GetByIdAsync(id, cancellationToken);

        return Ok(customer);
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(CustomerResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ValidationProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CustomerResponse>> Create(
        CreateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var customer =
            await _customerService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = customer.Id },
            customer);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(
        typeof(CustomerResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ValidationProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CustomerResponse>> Update(
        Guid id,
        UpdateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var customer =
            await _customerService.UpdateAsync(
                id,
                request,
                cancellationToken);

        return Ok(customer);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _customerService.DeleteAsync(id, cancellationToken);

        return NoContent();
    }
}