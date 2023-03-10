using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading.Channels;

namespace Messaging.Interfaces.Impl.RabbitMQ;

public class RabbitMQConsumer : IDisposable, IReceiveMQ<BasicDeliverEventArgs>
{
    private readonly string _queueName;
    private readonly string _hostName;
    private readonly string _exchange = "direct_exchange";

    private readonly IConnection _connection;
    private readonly IModel _chennal;
    public RabbitMQConsumer(string queryName, string hostName)
    {
        _queueName = queryName;
        _hostName = hostName;

        var factory = new ConnectionFactory()
        {
            HostName = _hostName,
        };

        _connection = factory.CreateConnection();
        _chennal = _connection.CreateModel();

        _queueName = _chennal.QueueDeclare().QueueName;
    }

    public void Receive(EventHandler<BasicDeliverEventArgs> receiveCallback)
    {
        _chennal.ExchangeDeclare(_exchange, ExchangeType.Fanout);

        _chennal.QueueBind(_queueName, _exchange, string.Empty);

        var consumer = new EventingBasicConsumer(_chennal);
        consumer.Received += receiveCallback;

        _chennal.BasicConsume(_queueName, true, consumer);
    }

    public void Dispose()
    {
        try
        {
            _chennal?.Close();
            _chennal?.Dispose();

            _connection?.Close();
            _connection?.Dispose();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CRITICAL ERROR]: Cannot dispose RabbitMQ channel or connection. Message:{ex.Message}");
        }
    }

    ~RabbitMQConsumer()
    {
        Dispose();
    }
}
