using MassTransit;
using Messaging.Interfaces;
using Messaging.Interfaces.Impl;
using Messaging.Models.Requests;
using Microsoft.Extensions.Logging;
using Restaurant.Booking.Services;

namespace Restaurant.Booking.Consumers;

public class BookingConsumer : IConsumer<IBookingRequest>
{
    private readonly IBus _bus;
    private readonly IBookingService _bookingService;
    private readonly IInMemoryRepository<BookingRequestModel> _inMemoryRepository;
    private readonly ILogger _logger;


    public BookingConsumer(
        IBus bus,
        IBookingService bookingService,
        IInMemoryRepository<BookingRequestModel> inMemoryRepository,
        ILogger<BookingConsumer> logger)
    {
        _bus = bus;
        _bookingService = bookingService;
        _inMemoryRepository = inMemoryRepository;
        _logger = logger;
    }

    public Task Consume(ConsumeContext<IBookingRequest> context)
    {
        var model = _inMemoryRepository.GetAll().FirstOrDefault(item => item.OrderId == context.Message.OrderId);

        if (model is not null && model.CheckMessageId(context.MessageId.ToString()))
        {
            _logger.LogInformation($"[LOGGER_INFO]: Повторное сообщение от {context.MessageId}");
            return context.ConsumeCompleted;
        }

        var requestModel = new BookingRequestModel(
            context.Message.OrderId,
            context.Message.ClientId,
            context.Message.PreOrder,
            context.Message.CreationDate,
            context.MessageId.ToString());

        var resultModel = model?.Update(requestModel, context.MessageId.ToString()) ?? requestModel;

        _inMemoryRepository.AddOrUpdate(resultModel);

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
            _logger.LogInformation($"Нет мест для - {context.Message.OrderId}");

            _bus.Publish<Fault<IBookingRequest>>(
                context,
                context.CancellationToken);
        }

        return context.ConsumeCompleted;
    }
}
