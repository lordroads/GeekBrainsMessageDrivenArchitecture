using MassTransit;
using Messaging.Interfaces;
using Restaurant.Booking.Services;

namespace Restaurant.Booking.Consumers;

public class BookingCancellationConsumer : IConsumer<IBookingCancellation>
{
    private readonly IRepositoryTable _repositoryTable;

    public BookingCancellationConsumer(IRepositoryTable repositoryTable)
    {
        _repositoryTable = repositoryTable;
    }

    public Task Consume(ConsumeContext<IBookingCancellation> context)
    {
        Console.WriteLine($"Галя! Отмена {context.Message.ClientId}");

        _repositoryTable.TablesToFree(context.Message.ClientId);

        return context.ConsumeCompleted;
    }
}
