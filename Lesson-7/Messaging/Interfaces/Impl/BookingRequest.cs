using Messaging.Models;

namespace Messaging.Interfaces.Impl;

public class BookingRequest : IBookingRequest
{
    public Guid OrderId { get; }
    public Guid ClientId { get; }
    public Dish? PreOrder { get; }
    public int CountOfPersons { get; }
    public DateTime CreationDate { get; } = DateTime.Now;

    public BookingRequest(
        Guid orderId,
        Guid clientId,
        int countOfPersons,
        Dish? preOrder)
    {
        ClientId = clientId;
        PreOrder = preOrder;
        CountOfPersons = countOfPersons;
        OrderId = orderId;
    }
}