namespace Messaging.Models.Requests;

public abstract class AbstractRequestModel<T>
{
    protected readonly List<string> _messageIds = new List<string>();
    public Guid OrderId { get; protected set; }
    public Guid ClientId { get; protected set; }

    protected AbstractRequestModel(Guid orderId, Guid clientId, string messageId)
    {
        _messageIds.Add(messageId);

        OrderId = orderId;
        ClientId = clientId;
    }

    public virtual bool CheckMessageId(string messageId)
    {
        return _messageIds.Contains(messageId);
    }

    public abstract T Update(T model, string messageId);
}
