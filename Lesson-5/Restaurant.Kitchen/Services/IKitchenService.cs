using Messaging.Models;

namespace Restaurant.Kitchen.Services;

public interface IKitchenService
{
    public bool CheckMenu(Dish? dish, int countOfPersons, Guid orderId);
    public void OrderCancallation(Guid clientId);
}