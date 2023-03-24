using Restaurant.Booking.Saga;

namespace Messaging.Interfaces.Impl;

public class BookingExpire : IBookingExpire
{
    private readonly RestaurantBooking _instance;
    public Guid OrderId => _instance.OrderId;

    public BookingExpire(RestaurantBooking instance)
    {
        _instance = instance;
    }
}
