using AqlliAgronom.API.Models;
using AqlliAgronom.Application.Common.Interfaces;
using AqlliAgronom.Application.Common.Models;
using AqlliAgronom.Application.Features.Products.Commands.CreateProduct;
using AqlliAgronom.Application.Features.Products.Commands.DeleteProduct;
using AqlliAgronom.Application.Features.Products.Commands.UpdateProduct;
using AqlliAgronom.Application.Features.Products.Commands.UpdateStock;
using AqlliAgronom.Application.Features.Products.DTOs;
using AqlliAgronom.Application.Features.Products.Queries.GetProductById;
using AqlliAgronom.Application.Features.Products.Queries.GetProductList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AqlliAgronom.API.Controllers;

/// <summary>
/// Agricultural product catalog.
/// </summary>
public class ProductsController(ICurrentUserService currentUser) : BaseApiController
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

    /// <summary>
    /// Get a single product by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ProductDto>>> GetById(Guid id, CancellationToken ct)
    {
        var result = await Mediator.Send(new GetProductByIdQuery(id), ct);
        return OkResponse(result);
    }

    /// <summary>
    /// Create a new product (Admin / Agronom only).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Agronom")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<ProductDto>>> Create(
        [FromBody] CreateProductCommand command,
        CancellationToken ct)
    {
        var cmd = command with { CreatedById = currentUser.UserId };
        var product = await Mediator.Send(cmd, ct);
        return CreatedResponse(product, $"/api/v1/products/{product.Id}");
    }

    /// <summary>
    /// Update an existing product (Admin / Agronom only).
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Agronom")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ProductDto>>> Update(
        Guid id,
        [FromBody] UpdateProductCommand command,
        CancellationToken ct)
    {
        var cmd = command with { ProductId = id };
        var result = await Mediator.Send(cmd, ct);
        return OkResponse(result);
    }

    /// <summary>
    /// Update stock quantity for a product (Admin / Agronom only).
    /// </summary>
    [HttpPut("{id:guid}/stock")]
    [Authorize(Roles = "Admin,Agronom")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ProductDto>>> UpdateStock(
        Guid id,
        [FromBody] UpdateStockRequest request,
        CancellationToken ct)
    {
        var result = await Mediator.Send(new UpdateStockCommand(id, request.Quantity), ct);
        return OkResponse(result);
    }

    /// <summary>
    /// Delete a product (Admin only, or Agronom who created it).
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,Agronom")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await Mediator.Send(new DeleteProductCommand(id, currentUser.UserId!.Value), ct);
        return NoContent();
    }
}

public record UpdateStockRequest(int Quantity);
