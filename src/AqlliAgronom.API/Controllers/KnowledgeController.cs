using AqlliAgronom.API.Models;
using AqlliAgronom.Application.Common.Models;
using AqlliAgronom.Application.Features.Knowledge.Commands.CreateKnowledgeEntry;
using AqlliAgronom.Application.Features.Knowledge.Commands.PublishKnowledgeEntry;
using AqlliAgronom.Application.Features.Knowledge.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AqlliAgronom.API.Controllers;

/// <summary>
/// Knowledge base management — requires Admin or Agronom role for write operations.
/// </summary>
public class KnowledgeController : BaseApiController
{
    /// <summary>
    /// Create a new agronomic knowledge entry (Admin/Agronom only).
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "AgronomiystOrAdmin")]
    [ProducesResponseType(typeof(ApiResponse<KnowledgeEntryDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<KnowledgeEntryDto>>> Create(
        [FromBody] CreateKnowledgeEntryCommand command, CancellationToken ct)
    {
        var result = await Mediator.Send(command, ct);
        return CreatedResponse(result, $"/api/v1/knowledge/{result.Id}");
    }

    /// <summary>
    /// Publish a knowledge entry, making it available for RAG search (Admin/Agronom only).
    /// </summary>
    [HttpPost("{id:guid}/publish")]
    [Authorize(Policy = "AgronomiystOrAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Publish(Guid id, CancellationToken ct)
    {
        await Mediator.Send(new PublishKnowledgeEntryCommand(id), ct);
        return NoContent();
    }
}
