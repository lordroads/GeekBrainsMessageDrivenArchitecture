using System;

namespace Messaging.Models;

public class Dish
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    public Dish(int id, string name)
    {
        Name = name;
    }

    public Dish() { }

    public override bool Equals(object? obj)
    {
        if (obj == null) 
            return false;

        if (obj is Dish dish) 
            return Equals(dish);

        return false;
    }

    public bool Equals(Dish other)
    {
        if(other == null) 
            return false;
 
        return Name == other.Name;
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}