using Messaging.Interfaces;
using Restaurant.Booking.Entities;
using Restaurant.Booking.Enums;

namespace Restaurant.Booking.Services.Impl;

public class RepositoryTable : IRepositoryTable
{
    private readonly List<Table> _tables;

    public RepositoryTable()
    {
        _tables = new List<Table>();

        for (int i = 0; i < 10; i++)
        {
            _tables.Add(new Table(i));
        }
    }
    public bool Booking(IBookingRequest request)
    {
        var tables = GetTables(request.CountOfPersons);

        var result = tables.Count > 0;

        if (result)
        {
            foreach (var item in tables)
            {
                _tables.Remove(item);

                item
                    .SetStateTable(StateTable.Booked)
                    .SetClientTable(request.ClientId);

                _tables.Add(item);
            }

            Console.WriteLine($"[BOOKING]: OrderId {request.OrderId} table count: {tables.Count}");
        }

        return result;
    }

    public void TablesToFree(Guid clientId)
    {
        var tables = _tables.FindAll(table => table.ClientId == clientId);

        foreach (var item in tables)
        {
            _tables.Remove(item);

            item
                .SetStateTable(StateTable.Free)
                .SetClientTable(null);

            _tables.Add(item);
        }

        Console.WriteLine($"Освободилось {tables.Count} стол(а), " +
            $"на {tables.Sum(table => table.SeatsCount)} мест.");
    }

    private List<Table> GetTables(int countOfPersons)
    {
        if (countOfPersons <= 0)
        {
            return new List<Table>();
        }

        var table = _tables.FirstOrDefault(table => table.StateTable == StateTable.Free);

        if (table is null)
        {
            return new List<Table>();
        }

        var result = new List<Table>
        {
            table
        };

        countOfPersons -= table.SeatsCount;

        if (countOfPersons > 0) 
        {
            result.AddRange(GetTables(countOfPersons));
        }

        return result;
    }
}