using AqlliAgronom.Application.Features.Auth.Commands.LoginFarmer;
using AqlliAgronom.Application.Features.Auth.Commands.RegisterFarmer;
using AqlliAgronom.Application.Features.Auth.DTOs;
using AqlliAgronom.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace AqlliAgronom.API.Controllers;

/// <summary>
/// Authentication endpoints — register, login, token refresh.
/// </summary>
public class AuthController : BaseApiController
{
    /// <summary>
    /// Register a new farmer account.
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register(
        [FromBody] RegisterFarmerCommand command, CancellationToken ct)
    {
        var result = await Mediator.Send(command, ct);
        return CreatedResponse(result, $"/api/v1/users/{result.UserId}");
    }

    /// <summary>
    /// Login with phone number and password.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login(
        [FromBody] LoginFarmerCommand command, CancellationToken ct)
    {
        var result = await Mediator.Send(command, ct);
        return OkResponse(result, "Login successful");
    }
}
