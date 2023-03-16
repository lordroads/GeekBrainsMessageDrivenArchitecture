namespace Messaging.Interfaces;

public interface IKitchenReady
{
    public Guid ClientId { get; }
    public Guid OrderId { get; }
    public bool Seccesed { get; }
}