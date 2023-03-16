using MassTransit;
using Messaging.Interfaces;
using Messaging.Interfaces.Impl;
using Restaurant.Booking.Services;

namespace Restaurant.Booking.Consumers;

public class BookingConsumer : IConsumer<IBookingRequest>
{
    private readonly IBus _bus;
    private readonly IBookingService _bookingService;

    public BookingConsumer(IBus bus, IBookingService bookingService)
    {
        _bus = bus;
        _bookingService = bookingService;
    }

    public Task Consume(ConsumeContext<IBookingRequest> context)
    {
        var result = _bookingService.BookingTable(context.Message);

        if (result)
        {
            _bus.Publish<ITableBooked>(
                new TableBooked(context.Message.OrderId,
                context.Message.ClientId,
                context.Message.CountOfPersons,
                result,
                context.Message.PreOrder)
                , context.CancellationToken);
        }
        else
        {
            Console.WriteLine($"Нет мест для - {context.Message.OrderId}");

            _bus.Publish<Fault<IBookingRequest>>(
                context,
                context.CancellationToken);
        }

        return context.ConsumeCompleted;
    }
}
