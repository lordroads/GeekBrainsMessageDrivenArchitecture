using MassTransit;
using Messaging.Exceptions;
using Messaging.Interfaces;
using Messaging.Interfaces.Impl;
using Messaging.Models.Requests;
using Restaurant.Kitchen.Services;

namespace Restaurant.Kitchen.Consumers
{
    public class KitchenConsumer : IConsumer<ITableBooked>
    {
        private readonly IKitchenService _kitchenService;
        private readonly IBus _bus;
        private readonly IInMemoryRepository<KitchenRequestModel> _inMemoryRepository;

        public KitchenConsumer(
            IBus bus, 
            IKitchenService kitchenService, 
            IInMemoryRepository<KitchenRequestModel> inMemoryRepository)
        {
            _bus = bus;
            _kitchenService = kitchenService;
            _inMemoryRepository = inMemoryRepository;
        }

        public Task Consume(ConsumeContext<ITableBooked> context)
        {
            var model = _inMemoryRepository.GetAll().FirstOrDefault(item => item.OrderId == context.Message.OrderId);

            if (model is not null && model.CheckMessageId(context.MessageId.ToString()))
            {
                Console.WriteLine($"[LOGGER_INFO]: Повторное сообщение от {context.MessageId}");
                return context.ConsumeCompleted;
            }

            var requestModel = new KitchenRequestModel(
                context.Message.OrderId,
                context.Message.ClientId,
                context.MessageId.ToString(),
                context.Message.CountOfPersons,
                context.Message.Success,
                context.Message.PreOrder);

            var resultModel = model?.Update(requestModel, context.MessageId.ToString()) ?? requestModel;

            _inMemoryRepository.AddOrUpdate(resultModel);

            if (context.Message.PreOrder is null) { throw new LasagnaException("Ошибка лазаньи!"); }

            var result = _kitchenService.CheckMenu(context.Message.PreOrder, context.Message.CountOfPersons, context.Message.OrderId);

            Console.WriteLine($"Dish {context.Message.PreOrder?.Name ?? ("Лазанья")} - {context.Message.OrderId} [STATUS KITCHEN]: {result}");

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