using MassTransit;
using Messaging.Interfaces;
using Microsoft.Extensions.Logging;
using Restaurant.Booking.Services;

namespace Restaurant.Booking.Consumers;

public class BookingCancellationConsumer : IConsumer<IBookingCancellation>
{
    private readonly IRepositoryTable _repositoryTable;
    private readonly ILogger _logger;

    public BookingCancellationConsumer(
        IRepositoryTable repositoryTable, 
        ILogger<BookingCancellationConsumer> logger)
    {
        _repositoryTable = repositoryTable;
        _logger = logger;
    }

    public Task Consume(ConsumeContext<IBookingCancellation> context)
    {
        _logger.LogInformation($"Отмена бронирования {context.Message.ClientId}");

        _repositoryTable.TablesToFree(context.Message.ClientId);

        return context.ConsumeCompleted;
    }
}
