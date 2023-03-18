namespace Restaurant.Booking.Services;

public interface IHostasService
{
    public void InitBookingTable(int countOfPersons, CancellationToken cancellationToken);
}