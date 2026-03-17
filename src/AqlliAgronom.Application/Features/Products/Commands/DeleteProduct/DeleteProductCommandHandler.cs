using AqlliAgronom.Application.Common.Exceptions;
using AqlliAgronom.Domain.Enums;
using AqlliAgronom.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AqlliAgronom.Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler(IUnitOfWork uow, ILogger<DeleteProductCommandHandler> logger)
    : IRequestHandler<DeleteProductCommand>
{
    public async Task Handle(DeleteProductCommand request, CancellationToken ct)
    {
        var product = await uow.Products.GetByIdAsync(request.ProductId, ct)
            ?? throw new NotFoundException("Product", request.ProductId.ToString());

        var requester = await uow.Users.GetByIdAsync(request.RequestingUserId, ct)
            ?? throw new NotFoundException("User", request.RequestingUserId.ToString());

        if (product.CreatedById != request.RequestingUserId && requester.Role != UserRole.Admin)
            throw new UnauthorizedAccessException("You can only delete your own products.");

        uow.Products.Delete(product);
        await uow.SaveChangesAsync(ct);

        logger.LogInformation("Product {ProductId} deleted by {UserId}", request.ProductId, request.RequestingUserId);
    }
}
