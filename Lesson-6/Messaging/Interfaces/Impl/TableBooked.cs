using Messaging.Models;

namespace Messaging.Interfaces.Impl;

public class TableBooked : ITableBooked
{
    public Guid OrderId { get; }
    public Guid ClientId { get; }
    public int CountOfPersons { get; }
    public bool Success { get; }
    public Dish? PreOrder { get; }

    public TableBooked(Guid orderId, Guid clientId, int countOfPersons, bool success, Dish? preOrder)
    {
        OrderId = orderId;
        ClientId = clientId;
        CountOfPersons = countOfPersons;
        Success = success;
        PreOrder = preOrder;
    }
}
