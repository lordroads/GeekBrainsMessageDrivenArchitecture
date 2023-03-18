using Messaging.Interfaces;

namespace Restaurant.Booking.Services.Impl;

public class BookingService : IBookingService
{
    private readonly IRepositoryTable _repositoryTable;

    public BookingService(IRepositoryTable repositoryTable)
    {
        _repositoryTable = repositoryTable;
    }

    public bool BookingTable(IBookingRequest request)
    {
        return _repositoryTable.Booking(request);
    }
}