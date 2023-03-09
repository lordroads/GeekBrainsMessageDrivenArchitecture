﻿namespace Messaging.Interfaces.Impl;

public class KitchenReady : IKitchenReady
{
    public Guid OrderId { get; }
    public bool Ready { get; }


    public KitchenReady(Guid orderId, bool ready)
    {
        OrderId = orderId;
        Ready = ready;
    }
}
