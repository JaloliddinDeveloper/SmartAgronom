using AqlliAgronom.API.Models;
using AqlliAgronom.Application.Features.MathGame.Commands.SubmitScore;
using AqlliAgronom.Application.Features.MathGame.DTOs;
using AqlliAgronom.Application.Features.MathGame.Queries.GetLeaderboard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AqlliAgronom.API.Controllers;

/// <summary>
/// MathRace game scores and leaderboard — no authentication required.
/// </summary>
[EnableRateLimiting("math_game")]
public class MathController : BaseApiController
{
    /// <summary>
    /// Get leaderboard — top scores, optionally filtered by difficulty.
    /// </summary>
    [HttpGet("scores")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<MathScoreDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<MathScoreDto>>>> GetLeaderboard(
        [FromQuery] string? difficulty = null,
        [FromQuery] int limit = 20,
        CancellationToken ct = default)
    {
        var result = await Mediator.Send(new GetMathLeaderboardQuery(difficulty, limit), ct);
        return OkResponse(result);
    }

    /// <summary>
    /// Submit a new score after a game session.
    /// </summary>
    [HttpPost("scores")]
    [ProducesResponseType(typeof(ApiResponse<MathScoreDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<ApiResponse<MathScoreDto>>> Submit(
        [FromBody] SubmitMathScoreCommand command,
        CancellationToken ct)
    {
        var result = await Mediator.Send(command, ct);
        return CreatedResponse(result, $"/api/v1/math/scores");
    }
}
