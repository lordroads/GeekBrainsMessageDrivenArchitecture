namespace Messaging.Models;

public class OrderDish
{
    public Guid ClientId { get; }
    public int CountOfPersons { get; }

    public OrderDish(Guid clientId, int countOfPersons)
    {
        ClientId = clientId;
        CountOfPersons = countOfPersons;
    }
}
