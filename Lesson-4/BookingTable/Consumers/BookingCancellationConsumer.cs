using MassTransit;
using Messaging.Interfaces;
using Restaurant.Booking.Services;

namespace Restaurant.Booking.Consumers;

public class BookingCancellationConsumer : IConsumer<Fault<IBookingCancellation>>
{
    private readonly IRepositoryTable _repositoryTable;

    public BookingCancellationConsumer(IRepositoryTable repositoryTable)
    {
        _repositoryTable = repositoryTable;
    }

    public Task Consume(ConsumeContext<Fault<IBookingCancellation>> context)
    {
        Console.WriteLine($"Галя! Отмена {context.Message.Message.ClientId}");

        _repositoryTable.TablesToFree(context.Message.Message.ClientId);

        return context.ConsumeCompleted;
    }
}
