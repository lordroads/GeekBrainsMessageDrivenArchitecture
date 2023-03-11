using Messaging.Models;

namespace Messaging.Interfaces;

public interface IKitchenAccident
{
    public Guid OrderId { get; }
    public Dish Dish { get; }
}
