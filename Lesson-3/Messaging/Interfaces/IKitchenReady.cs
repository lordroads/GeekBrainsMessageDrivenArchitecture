namespace Messaging.Interfaces;

public interface IKitchenReady
{
    public Guid OrderId { get; }
    public bool Ready { get; }
}