using Messaging.Models;

namespace Messaging.Interfaces;

public interface ITableBooked
{
    public Guid OrderId { get; }
    public Guid ClientId { get; }
    public Dish? PreOrder { get; }
    public bool Success { get; }
}