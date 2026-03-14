using AqlliAgronom.Application.Common.Interfaces;
using AqlliAgronom.Domain.Events.Orders;
using AqlliAgronom.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AqlliAgronom.Application.EventHandlers.Orders;

public class OrderPlacedEventHandler(
    IUnitOfWork uow,
    ITelegramNotificationService telegram,
    ILogger<OrderPlacedEventHandler> logger)
    : INotificationHandler<OrderPlacedEvent>
{
    public async Task Handle(OrderPlacedEvent notification, CancellationToken ct)
    {
        var user = await uow.Users.GetByIdAsync(notification.UserId, ct);
        if (user?.TelegramChatId is null) return;

        var message = $"""
            ✅ Buyurtmangiz qabul qilindi!

            📦 Buyurtma ID: {notification.OrderId.ToString()[..8]}
            👤 Ism: {notification.FarmerName}
            📞 Telefon: {notification.Phone}

            Agronomiyst tez orada siz bilan bog'lanadi.
            """;

        try
        {
            await telegram.SendMessageAsync(user.TelegramChatId.Value, message, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send order confirmation to Telegram for user {UserId}", notification.UserId);
        }
    }
}
