namespace Messaging.Interfaces.Impl;

public class KitchenReady : IKitchenReady
{
    public Guid ClientId { get; }
    public Guid OrderId { get; }
    public bool Seccesed { get; }


    public KitchenReady(Guid clientId, Guid orderId, bool seccesed)
    {
        ClientId = clientId;
        OrderId = orderId;
        Seccesed = seccesed;
    }
}
