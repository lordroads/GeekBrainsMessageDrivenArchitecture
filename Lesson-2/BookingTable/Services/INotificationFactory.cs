using BookingTable.Enums;

namespace BookingTable.Services;

public interface INotificationFactory
{
    public NotificationAbstract GetNotification(NotificationType type);
}