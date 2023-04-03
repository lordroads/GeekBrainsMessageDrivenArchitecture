using Messaging.Interfaces;

namespace Restaurant.Booking.Services;

public interface IBookingService
{
    public bool BookingTable(IBookingRequest request);
}