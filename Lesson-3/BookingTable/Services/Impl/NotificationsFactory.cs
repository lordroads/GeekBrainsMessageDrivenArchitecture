using MassTransit;
using Restaurant.Booking.Enums;

namespace BookingTable.Services.Impl;

public class NotificationFactory : INotificationFactory
{
    private readonly IBus _bus;

    public NotificationFactory(IBus bus)
    {
        _bus = bus;
    }

    public NotificationAbstract GetNotification(NotificationType type)
    {
        return type switch
        {
            NotificationType vehicule when vehicule == NotificationType.CALL => new CallNotification(),
            NotificationType vehicule when vehicule == NotificationType.SMS => new SmsNotification(_bus),
            _ => throw new IndexOutOfRangeException($"Cannot create this type {type}"),
        };
    }
}