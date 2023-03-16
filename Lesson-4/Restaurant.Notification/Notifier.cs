using System.Collections.Concurrent;

namespace Restaurant.Notification;

public class Notifier
{
    public void Send(Guid orderId, string message, Guid? clientId = null)
    {
        Console.WriteLine($"[ORDER_ID]: {orderId} \n[CLIENT_ID]: {clientId} \n[MESSAGE]: {message}");
    }
}