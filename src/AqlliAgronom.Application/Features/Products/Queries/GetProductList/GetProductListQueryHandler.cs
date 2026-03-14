using AqlliAgronom.Application.Common.Models;
using AqlliAgronom.Application.Features.Products.DTOs;
using AqlliAgronom.Domain.Interfaces;
using MediatR;

namespace AqlliAgronom.Application.Features.Products.Queries.GetProductList;

public class GetProductListQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetProductListQuery, PaginatedList<ProductSummaryDto>>
{
    public async Task<PaginatedList<ProductSummaryDto>> Handle(GetProductListQuery request, CancellationToken ct)
    {
        var (items, totalCount) = await uow.Products.GetPagedAsync(
            page: request.Page,
            pageSize: request.PageSize,
            searchTerm: request.SearchTerm,
            availableOnly: request.AvailableOnly,
            category: request.Category,
            ct: ct);

        var dtos = items.Select(p => new ProductSummaryDto(
            p.Id, p.Name, p.Category, p.Price.Amount, p.Price.Currency,
            p.IsAvailable, p.ImageUrl)).ToList();

        return PaginatedList<ProductSummaryDto>.Create(dtos, totalCount, request.Page, request.PageSize);
    }
}
