using AqlliAgronom.Application.Common.Models;
using AqlliAgronom.Application.Features.Products.DTOs;
using MediatR;

namespace AqlliAgronom.Application.Features.Products.Queries.GetProductList;

public record GetProductListQuery(
    int Page = 1,
    int PageSize = 20,
    string? SearchTerm = null,
    bool? AvailableOnly = null,
    string? Category = null) : IRequest<PaginatedList<ProductSummaryDto>>;
