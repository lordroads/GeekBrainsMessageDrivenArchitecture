using MassTransit;
using Messaging.Interfaces;
using Microsoft.Extensions.Logging;
using Restaurant.Notification.Services;

namespace Restaurant.Notification.Consumers
{
    public class NotifierConsumer : IConsumer<INotify>
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger _logger;

        public NotifierConsumer(
            INotificationService notificationService, 
            ILogger<NotifierConsumer> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public Task Consume(ConsumeContext<INotify> context)
        {
            _logger.LogDebug("Получение сообщения!");

            _notificationService.Notify(context.Message);

            return context.ConsumeCompleted;
        }
    }
}