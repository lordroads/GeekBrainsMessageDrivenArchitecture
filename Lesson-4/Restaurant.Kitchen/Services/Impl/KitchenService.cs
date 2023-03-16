using Messaging.Models;

namespace Restaurant.Kitchen.Services.Impl;

public class KitchenService : IKitchenService
{
    private Dictionary<Dish, int> _stopList = 
        new Dictionary<Dish, int>();

    public KitchenService()
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
    }


    public bool CheckMenu(Dish? dish, int countOfPersons)
    {
        if (dish is null)
        {
            return false;
        }

        if (_stopList[dish] > 0 && _stopList[dish] >= countOfPersons)
        {
            _stopList[dish] = _stopList[dish] - countOfPersons;

            return true;
        }

        return false;
    }
}