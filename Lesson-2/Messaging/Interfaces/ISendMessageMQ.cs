namespace Messaging.Interfaces;

public interface ISendMessageMQ<T>
{
    public void SendMessage(object obj);
    public void SendMessage(T message);
}