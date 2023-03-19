namespace Messaging.Interfaces.Impl;

public class BookingCancellation : IBookingCancellation
{
    public Guid ClientId { get; }

    public BookingCancellation(Guid clientId)
    {
        ClientId = clientId;
    }
}
