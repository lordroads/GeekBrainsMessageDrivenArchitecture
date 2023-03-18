using MassTransit;
using Messaging.Interfaces;
using Restaurant.Kitchen.Services;

namespace Restaurant.Kitchen.Consumers;

public class KitchenCancellationConsumer : IConsumer<IBookingCancellation>
{
    private readonly IKitchenService _kitchenService;

    public KitchenCancellationConsumer(IKitchenService kitchenService)
    {
        _kitchenService = kitchenService;
    }

    public Task Consume(ConsumeContext<IBookingCancellation> context)
    {
        _kitchenService.OrderCancallation(context.Message.ClientId);

        Console.WriteLine($"Отмена предзаказа для {context.Message.ClientId}");

        return context.ConsumeCompleted;
    }
}
