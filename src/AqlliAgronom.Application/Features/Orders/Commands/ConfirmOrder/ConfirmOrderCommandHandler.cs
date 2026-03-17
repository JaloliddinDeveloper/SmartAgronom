using AqlliAgronom.Application.Common.Exceptions;
using AqlliAgronom.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AqlliAgronom.Application.Features.Orders.Commands.ConfirmOrder;

public class ConfirmOrderCommandHandler(IUnitOfWork uow, ILogger<ConfirmOrderCommandHandler> logger)
    : IRequestHandler<ConfirmOrderCommand, Guid>
{
    public async Task<Guid> Handle(ConfirmOrderCommand request, CancellationToken ct)
    {
        var order = await uow.Orders.GetWithItemsAsync(request.OrderId, ct)
            ?? throw new NotFoundException("Order", request.OrderId.ToString());

        order.Confirm(request.AgronomiystId);
        await uow.SaveChangesAsync(ct);

        logger.LogInformation("Order {OrderId} confirmed by agronom {AgronomiystId}",
            request.OrderId, request.AgronomiystId);

        return order.UserId;
    }
}
