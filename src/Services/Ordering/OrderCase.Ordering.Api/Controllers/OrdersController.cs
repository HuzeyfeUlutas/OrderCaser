using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderCase.Ordering.Api.Authentication;
using OrderCase.Ordering.Api.Requests;
using OrderCase.Ordering.Application.Features.Orders;
using OrderCase.Ordering.Application.Features.Orders.Commands.CreateOrder;
using OrderCase.Ordering.Application.Features.Orders.Commands.DeleteOrder;
using OrderCase.Ordering.Application.Features.Orders.Queries.GetOrderById;
using OrderCase.Ordering.Application.Features.Orders.Queries.GetOrders;
using OrderCase.Ordering.Application.Features.Orders.Queries.GetOrdersByUserId;

namespace OrderCase.Ordering.Api.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public sealed class OrdersController(ISender sender) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(CreateOrderResult), StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(
        [FromBody] CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateOrderCommand(
            request.UserId,
            request.ProductId,
            request.Quantity,
            request.PaymentMethod);

        var result = await sender.Send(command, cancellationToken);

        return AcceptedAtAction(nameof(GetById), new { id = result.OrderId }, result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var response = await sender.Send(new GetOrderByIdQuery(id), cancellationToken);
        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<OrderResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var response = await sender.Send(new GetOrdersQuery(), cancellationToken);
        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteOrderCommand(id), cancellationToken);
        return NoContent();
    }
    
    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(IReadOnlyList<OrderResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetByUserId(string userId, CancellationToken cancellationToken)
    {
        var response = await sender.Send(new GetOrdersByUserIdQuery(userId), cancellationToken);
        return Ok(response);
    }
}