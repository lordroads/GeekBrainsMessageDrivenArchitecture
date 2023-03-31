using Restaurant.Booking.Saga;

namespace Messaging.Interfaces.Impl;

public class GuestCame : IGuestCame
{
    private readonly RestaurantBooking _instance;
    public Guid OrderId => _instance.OrderId;

    public GuestCame(RestaurantBooking instance)
    {
        _instance = instance;
    }
}
