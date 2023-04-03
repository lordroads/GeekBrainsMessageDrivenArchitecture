using MassTransit;
using Messaging.Interfaces;
using Microsoft.Extensions.Logging;
using Restaurant.Kitchen.Services;

namespace Restaurant.Kitchen.Consumers;

public class KitchenCancellationConsumer : IConsumer<IBookingCancellation>
{
    private readonly IKitchenService _kitchenService;
    private readonly ILogger _logger;

    public KitchenCancellationConsumer(
        IKitchenService kitchenService, 
        ILogger<KitchenCancellationConsumer> logger)
    {
        _kitchenService = kitchenService;
        _logger = logger;
    }

    public Task Consume(ConsumeContext<IBookingCancellation> context)
    {
        _kitchenService.OrderCancallation(context.Message.ClientId);

        _logger.LogInformation($"Отмена предзаказа для {context.Message.ClientId}");

        return context.ConsumeCompleted;
    }
}
