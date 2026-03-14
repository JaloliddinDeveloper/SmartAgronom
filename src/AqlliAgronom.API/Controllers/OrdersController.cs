using AqlliAgronom.API.Models;
using AqlliAgronom.Application.Common.Interfaces;
using AqlliAgronom.Application.Features.Orders.Commands.PlaceOrder;
using AqlliAgronom.Application.Features.Orders.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AqlliAgronom.API.Controllers;

/// <summary>
/// Order management for agricultural products.
/// </summary>
[Authorize]
public class OrdersController(ICurrentUserService currentUser) : BaseApiController
{
    /// <summary>
    /// Place an order for recommended agricultural products.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<OrderDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<ApiResponse<OrderDto>>> PlaceOrder(
        [FromBody] PlaceOrderCommand command, CancellationToken ct)
    {
        // Override userId from JWT claim
        var cmd = command with { UserId = currentUser.UserId!.Value };
        var result = await Mediator.Send(cmd, ct);
        return CreatedResponse(result, $"/api/v1/orders/{result.Id}");
    }
}
