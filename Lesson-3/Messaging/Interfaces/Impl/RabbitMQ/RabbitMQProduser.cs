using RabbitMQ.Client;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Messaging.Interfaces.Impl.RabbitMQ;

public class RabbitMQProduser : IDisposable, ISendMessageMQ<string>
{
    private readonly string _queueName;
    private readonly string _hostName;
    private readonly string _exchange = "direct_exchange";

    private readonly IConnection _connection;
    private readonly IModel _chennal;
    public RabbitMQProduser(string queryName, string hostName)
    {
        _queueName = queryName;
        _hostName = hostName;

        var factory = new ConnectionFactory()
        {
            HostName = _hostName,
        };

        _connection = factory.CreateConnection();
        _chennal = _connection.CreateModel();
    }

    public void SendMessage(object obj)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            new BinaryFormatter().Serialize(ms, obj);

            var message = Convert.ToBase64String(ms.ToArray());

            SendMessage(message);
        }
    }
    public void SendMessage(string message)
    {
        _chennal.ExchangeDeclare(_exchange, ExchangeType.Fanout);

        var body = Encoding.UTF8.GetBytes(message);

        _chennal.BasicPublish(_exchange, string.Empty, null, body);
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

    ~RabbitMQProduser() 
    { 
        Dispose();
    }
}
