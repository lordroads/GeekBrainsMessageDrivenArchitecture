using MassTransit;
using Messaging.Interfaces;
using Restaurant.Booking.Services;

namespace Restaurant.Booking.Consumers;

public class BookingKitchenFaultConsumer : IConsumer<Fault<ITableBooked>>
{
    private readonly IRepositoryTable _repositoryTable;

    public BookingKitchenFaultConsumer(IRepositoryTable repositoryTable)
    {
        _repositoryTable = repositoryTable;
    }

    public Task Consume(ConsumeContext<Fault<ITableBooked>> context)
    {
        Console.WriteLine($"Снятие брони от {context.Message.Message.ClientId}");

        _repositoryTable.TablesToFree(context.Message.Message.ClientId);

        return context.ConsumeCompleted;
    }
}
