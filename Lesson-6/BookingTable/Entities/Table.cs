using Restaurant.Booking.Enums;

namespace Restaurant.Booking.Entities;

public class Table
{
    public int Id { get; }
    public int SeatsCount { get; } = new Random().Next(2, 6);
    public StateTable StateTable { get; private set; } = StateTable.Free;
    public Guid? ClientId { get; private set; } = null;

    public Table(int id)
    {
        Id = id;
    }

    public Table SetStateTable(StateTable state)
    {
        StateTable = state;

        return this;
    }

    public Table SetClientTable(Guid? clientId)
    {
        ClientId = clientId;

        return this;
    }
}