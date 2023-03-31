using Messaging.Models;
using Microsoft.Extensions.Logging;

namespace Restaurant.Kitchen.Services.Impl;

public class KitchenService : IKitchenService
{
    private Dictionary<Dish, int> _stopList = 
        new Dictionary<Dish, int>();

    private Dictionary<Dish, OrderDish> _preOrders =
        new Dictionary<Dish, OrderDish>();

    private readonly ILogger _logger;

    public KitchenService(ILogger<KitchenService> logger)
    {
        var _dishes = new List<Dish>
        {
            new Dish
            {
                Id= 1,
                Name = "Стейк из семги"
            },
            new Dish
            {
                Id= 2,
                Name = "Каре ягненка"
            },
            new Dish
            {
                Id= 3,
                Name = "Устрицы"
            }
        };

        foreach (var item in _dishes)
        {
            _stopList.Add(item, new Random().Next(4, 8));
        }
        _logger = logger;
    }


    public bool CheckMenu(Dish? dish, int countOfPersons, Guid clientId)
    {
        _logger.LogDebug("Проверка стоп листа на кухне.");

        if (dish is null)
        {
            return false;
        }

        if (_stopList[dish] > 0 && _stopList[dish] >= countOfPersons)
        {
            _stopList[dish] = _stopList[dish] - countOfPersons;

            var preOrder = new OrderDish(clientId, countOfPersons);

            _preOrders.Add(dish, preOrder);

            return true;
        }

        return false;
    }

    public void OrderCancallation(Guid clientId)
    {
        _logger.LogInformation("Отмена предзаказа на кухне.");

        var preOrder = _preOrders.FirstOrDefault(order => order.Value.ClientId == clientId);

        if (preOrder.Key is not null)
        {
            _preOrders.Remove(preOrder.Key);

            var dish = _stopList.FirstOrDefault(dish => dish.Key == preOrder.Key);

            _stopList.Remove(dish.Key);

            var count = dish.Value + preOrder.Value.CountOfPersons;

            _stopList.Add(dish.Key, count);
        }
    }
}