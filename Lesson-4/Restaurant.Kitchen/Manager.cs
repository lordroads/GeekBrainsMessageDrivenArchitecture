using MassTransit;
using Messaging.Interfaces;
using Messaging.Interfaces.Impl;
using Messaging.Models;

namespace Restaurant.Kitchen
{
    public class Manager
    {
        private readonly IBus _bus;

        public Manager(IBus bus)
        {
            _bus = bus;
        }

        public void CheckKitchenReady(Guid orderId, Dish? preOrder)
        {
            Console.WriteLine($"Get message from - {orderId}, Dish - {preOrder?.Name}");
            _bus.Publish<IKitchenReady>(new KitchenReady(orderId, true));
        }
    }
}