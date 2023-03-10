using Messaging.Interfaces;
using Messaging.Interfaces.Impl.RabbitMQ;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Events;
using System.Text;

namespace Restaurant.Notification;

public class Worker : BackgroundService
{
    private readonly IReceiveMQ<BasicDeliverEventArgs> _consumer;

	public Worker()
	{
		_consumer = new RabbitMQConsumer("BookingNotification", "localhost");
	}

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Run(() =>
        {
            _consumer.Receive((sender, args) =>
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine($" [x] Received {message}");
            });
        });
    }
}