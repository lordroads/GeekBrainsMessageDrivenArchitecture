using Messaging.Models;

namespace Messaging.Interfaces;

public interface IBookingRequest
{
    public Guid OrderId { get; }
    public Guid ClientId { get; }
    public Dish? PreOrder { get; }
    public int CountOfPersons { get; }
    public DateTime CreationDate { get; }
}
