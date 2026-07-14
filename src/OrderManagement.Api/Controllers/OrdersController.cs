using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.DTOs.Orders;
using OrderManagement.Application.Interfaces.Services;

namespace OrderManagement.Api.Controllers;

[ApiController]
[Route("api/orders")]
[Produces("application/json")]
public sealed class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(
        IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<OrderResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        return Ok(
            await _orderService.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        return Ok(
            await _orderService.GetByIdAsync(
                id,
                cancellationToken));
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(OrderResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ValidationProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OrderResponse>> Create(
    CreateOrderRequest request,
    CancellationToken cancellationToken)
    {
        var order =
            await _orderService.CreateAsync(
                request,
                cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = order.Id },
            order);
    }

    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(
        typeof(OrderResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ValidationProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> UpdateStatus(
    Guid id,
    UpdateOrderStatusRequest request,
    CancellationToken cancellationToken)
    {
        var order = await _orderService.UpdateStatusAsync(
            id,
            request,
            cancellationToken);

        return Ok(order);
    }
}