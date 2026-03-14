using AqlliAgronom.API.Models;
using AqlliAgronom.Application.Common.Models;
using AqlliAgronom.Application.Features.Products.DTOs;
using AqlliAgronom.Application.Features.Products.Queries.GetProductList;
using Microsoft.AspNetCore.Mvc;

namespace AqlliAgronom.API.Controllers;

/// <summary>
/// Agricultural product catalog.
/// </summary>
public class ProductsController : BaseApiController
{
    /// <summary>
    /// Get paginated list of products with optional filters.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<ProductSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<ProductSummaryDto>>>> GetList(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] bool? availableOnly = null,
        [FromQuery] string? category = null,
        CancellationToken ct = default)
    {
        var result = await Mediator.Send(
            new GetProductListQuery(page, pageSize, search, availableOnly, category), ct);
        return OkResponse(result);
    }
}
