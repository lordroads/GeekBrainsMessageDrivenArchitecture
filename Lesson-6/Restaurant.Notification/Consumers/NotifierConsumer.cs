using MassTransit;
using Messaging.Interfaces;
using Restaurant.Notification.Services;

namespace Restaurant.Notification.Consumers
{
    public class NotifierConsumer : IConsumer<INotify>
    {
        private readonly INotificationService _notificationService;

        public NotifierConsumer(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public Task Consume(ConsumeContext<INotify> context)
        {
            _notificationService.Notify(context.Message);

            return context.ConsumeCompleted;
        }
    }
}