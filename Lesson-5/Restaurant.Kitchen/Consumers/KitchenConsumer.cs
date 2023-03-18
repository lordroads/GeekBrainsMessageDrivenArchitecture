using MassTransit;
using Messaging.Interfaces;
using Messaging.Interfaces.Impl;
using Restaurant.Kitchen.Services;

namespace Restaurant.Kitchen.Consumers
{
    public class KitchenConsumer : IConsumer<ITableBooked>
    {
        private readonly IKitchenService _kitchenService;
        private readonly IBus _bus;

        public KitchenConsumer(IBus bus, IKitchenService kitchenService)
        {
            _bus = bus;
            _kitchenService = kitchenService;
        }

        public Task Consume(ConsumeContext<ITableBooked> context)
        {
            var result = _kitchenService.CheckMenu(context.Message.PreOrder, context.Message.CountOfPersons);

            Console.WriteLine($"Dish {context.Message.PreOrder?.Name ?? ("None")} - {context.Message.OrderId} [STATUS KITCHEN]: {result}");

            if (result)
            {
                _bus.Publish<IKitchenReady>(
                    new KitchenReady(
                        context.Message.ClientId,
                        context.Message.OrderId,
                        result),
                    context.CancellationToken);
            }
            else
            {
                _bus.Publish<Fault<ITableBooked>>(
                    context,
                    context.CancellationToken);
            }

            return context.ConsumeCompleted;
        }
    }
}