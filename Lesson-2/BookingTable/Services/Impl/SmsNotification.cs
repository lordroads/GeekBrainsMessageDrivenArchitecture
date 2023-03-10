using Messaging.Interfaces;
using Messaging.Interfaces.Impl.RabbitMQ;

namespace BookingTable.Services.Impl;

public class SmsNotification : NotificationAbstract
{
    private static readonly Lazy<SmsNotification> _instance =
        new Lazy<SmsNotification>(() => new SmsNotification());

    private readonly ISendMessageMQ<string> _produser;

    private SmsNotification()
    {
        _produser = new RabbitMQProduser("BookingNotification", "localhost");
    }

    public static SmsNotification GetInstance()
    {
        return _instance.Value;
    }

    protected override void Send(string message)
    {
        _produser.SendMessage($"[SMS]: {message}");
    }
}