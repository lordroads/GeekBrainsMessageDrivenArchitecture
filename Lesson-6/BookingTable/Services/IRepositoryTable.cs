using Messaging.Interfaces;

namespace Restaurant.Booking.Services;

public interface IRepositoryTable
{
    public bool Booking(IBookingRequest request);
    public void TablesToFree(Guid clientId);
}