using Restaurant.Booking.Saga;

namespace Messaging.Interfaces.Impl;

public class BookingOverdue : IBookingOverdue
{
    private readonly RestaurantBooking _instance;
    public Guid OrderId => _instance.OrderId;

    public BookingOverdue(RestaurantBooking instance)
    {
        _instance = instance;
    }
}
