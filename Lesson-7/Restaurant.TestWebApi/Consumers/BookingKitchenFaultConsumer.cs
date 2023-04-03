using MassTransit;
using Messaging.Interfaces;
using Microsoft.Extensions.Logging;
using Restaurant.Booking.Services;

namespace Restaurant.Booking.Consumers;

public class BookingKitchenFaultConsumer : IConsumer<Fault<ITableBooked>>
{
    private readonly IRepositoryTable _repositoryTable;
    private readonly ILogger _logger;

    public BookingKitchenFaultConsumer(
        IRepositoryTable repositoryTable, 
        ILogger<BookingKitchenFaultConsumer> logger)
    {
        _repositoryTable = repositoryTable;
        _logger = logger;
    }

    public Task Consume(ConsumeContext<Fault<ITableBooked>> context)
    {
        _logger.LogInformation($"Произошла ошибка на кухне. Снятие брони от {context.Message.Message.ClientId}");

        _repositoryTable.TablesToFree(context.Message.Message.ClientId);

        return context.ConsumeCompleted;
    }
}
