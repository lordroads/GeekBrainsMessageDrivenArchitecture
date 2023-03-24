namespace Messaging.Models.Requests;

public class KitchenRequestModel : AbstractRequestModel<KitchenRequestModel>
{
    public int CountOfPersons { get; private set; }
    public bool Success { get; private set; }
    public Dish? PreOrder { get; private set; }

    public KitchenRequestModel(
        Guid orderId, 
        Guid clientId,
        string messageId, 
        int countOfPersons, 
        bool success, 
        Dish? preOrder) : base(orderId, clientId, messageId)
    {
        CountOfPersons = countOfPersons;
        Success = success;
        PreOrder = preOrder;
    }

    public override KitchenRequestModel Update(KitchenRequestModel model, string messageId)
    {
        _messageIds.Add(messageId);

        OrderId = model.OrderId;
        ClientId = model.ClientId;
        PreOrder = model.PreOrder;
        CountOfPersons = model.CountOfPersons;
        Success = model.Success;

        return this;
    }
}
