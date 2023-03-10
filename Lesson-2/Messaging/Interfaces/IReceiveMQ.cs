namespace Messaging.Interfaces;

public interface IReceiveMQ<T>
{
    public void Receive(EventHandler<T> receiveCallback);
}