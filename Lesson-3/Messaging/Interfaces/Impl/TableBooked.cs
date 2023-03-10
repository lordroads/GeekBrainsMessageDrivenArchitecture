using Messaging.Models;

namespace Messaging.Interfaces.Impl;

public class TableBooked : ITableBooked, ITableBookedCall
{
    public Guid OrderId { get; }
    public Guid ClientId { get; }
    public Dish? PreOrder { get; }
    public bool Success { get; }
    public string? Message { get; }

    public TableBooked(
        Guid orderId,
        Guid clientId,
        bool success,
        Dish? preOrder = null,
        string message = null)
    {
        ClientId = clientId;
        PreOrder = preOrder;
        Success = success;
        Message = message;
        OrderId = orderId;
    }
}
