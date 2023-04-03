namespace Messaging.Models.Requests;

public class BookingRequestModel
{
    private readonly List<string> _messageIds = new List<string>();

    public Guid OrderId { get; private set; }
    public Guid ClientId { get; private set; }
    public Dish? PreOrder { get; private set; }
    public DateTime CreationDate { get; private set; }


    public BookingRequestModel(Guid orderId, Guid clientId, Dish? preOrder, DateTime creationDate, string messageId)
    {
        _messageIds.Add(messageId);

        OrderId = orderId;
        ClientId = clientId;
        PreOrder = preOrder;
        CreationDate = creationDate;
    }

    public bool CheckMessageId(string messageId)
    {
        return _messageIds.Contains(messageId);
    }

    public BookingRequestModel Update(BookingRequestModel model, string messageId)
    {
        _messageIds.Add(messageId);

        OrderId = model.OrderId;
        ClientId = model.ClientId;
        PreOrder = model.PreOrder;
        CreationDate = model.CreationDate;

        return this;
    }
}
