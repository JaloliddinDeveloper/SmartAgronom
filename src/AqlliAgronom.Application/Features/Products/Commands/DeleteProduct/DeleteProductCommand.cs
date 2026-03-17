using MediatR;

namespace AqlliAgronom.Application.Features.Products.Commands.DeleteProduct;

public record DeleteProductCommand(Guid ProductId, Guid RequestingUserId) : IRequest;
