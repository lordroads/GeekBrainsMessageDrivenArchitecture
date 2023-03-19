using MassTransit;
using Messaging.Interfaces;
using Messaging.Interfaces.Impl;
using Messaging.Models;

namespace Restaurant.Booking.Services.Impl;

public class HostasService : IHostasService
{
    private readonly List<Dish?> _dishes;
    private readonly IBus _bus;

    public HostasService(IBus bus)
    {
        _dishes = new List<Dish?>
        {
            new Dish
            {
                Id= 1,
                Name = "Стейк из семги"
            },
            new Dish
            {
                Id= 2,
                Name = "Каре ягненка"
            },
            new Dish
            {
                Id= 3,
                Name = "Устрицы"
            },
            null
        };
        _bus = bus;
    }

    public void InitBookingTable(int countOfPersons, CancellationToken cancellationToken)
    {
        _bus.Publish<IBookingRequest>(
            new BookingRequest(
                NewId.NextGuid(),
                NewId.NextGuid(),
                countOfPersons,
                RandomDish()), cancellationToken);
    }

    private Dish? RandomDish()
    {
        return _dishes[new Random().Next(1, 4)];
    }
}
