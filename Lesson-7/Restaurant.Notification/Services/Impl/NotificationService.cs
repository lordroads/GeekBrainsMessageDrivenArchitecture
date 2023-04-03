using Messaging.Interfaces;
using Microsoft.Extensions.Logging;

namespace Restaurant.Notification.Services.Impl;

public class NotificationService : INotificationService
{
    private readonly ILogger _logger;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }

    public void Notify(INotify notify)
    {
        _logger.LogInformation($"[CLIENT_ID]: {notify.ClientId}\n" +
            $"[ORDER_ID]: {notify.OrderId}\n" +
            $"[MESSAGE]: {notify.Message}");

        //TODO: Hard logic notification users.
    }
}