using Messaging.Interfaces;

namespace Restaurant.Notification.Services.Impl;

public class NotificationService : INotificationService
{
    public void Notify(INotify notify)
    {
        Console.WriteLine($"[CLIENT_ID]: {notify.ClientId}\n" +
            $"[ORDER_ID]: {notify.OrderId}\n" +
            $"[MESSAGE]: {notify.Message}");
    }
}