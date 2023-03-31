using MassTransit;
using Messaging.Interfaces;
using Messaging.Interfaces.Impl;
using Messaging.Models;
using Microsoft.Extensions.Logging;

namespace Restaurant.Booking.Services.Impl;

public class HostasService : IHostasService
{
    private readonly List<Dish?> _dishes;
    private readonly IBus _bus;
    private readonly ILogger _logger;

    public HostasService(IBus bus, ILogger<HostasService> logger)
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
        _logger = logger;
    }

    public void InitBookingTable(int countOfPersons, CancellationToken cancellationToken)
    {
        _logger.LogDebug($"Иницилизация бронирования на {countOfPersons} персон.");

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
