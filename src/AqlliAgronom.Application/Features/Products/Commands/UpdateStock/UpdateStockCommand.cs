using AqlliAgronom.Application.Features.Products.DTOs;
using MediatR;

namespace AqlliAgronom.Application.Features.Products.Commands.UpdateStock;

public record UpdateStockCommand(Guid ProductId, int Quantity) : IRequest<ProductDto>;
