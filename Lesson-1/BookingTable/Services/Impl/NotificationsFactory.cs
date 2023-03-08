using BookingTable.Enums;

namespace BookingTable.Services.Impl;

public class NotificationFactory : INotificationFactory
{
    public NotificationAbstract GetNotification(NotificationType type)
    {
        return type switch
        {
            NotificationType vehicule when vehicule == NotificationType.CALL => new CallNotification(),
            NotificationType vehicule when vehicule == NotificationType.SMS => new SmsNotification(),
            _ => throw new IndexOutOfRangeException($"Cannot create this type {type}"),
        };
    }
}