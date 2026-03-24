using AqlliAgronom.Application.Features.Products.DTOs;
using MediatR;

namespace AqlliAgronom.Application.Features.Products.Queries.GetProductById;

public record GetProductByIdQuery(Guid ProductId) : IRequest<ProductDto>;
