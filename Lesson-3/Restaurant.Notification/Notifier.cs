using System.Collections.Concurrent;

namespace Restaurant.Notification;

public class Notifier
{
    private readonly ConcurrentDictionary<Guid, Tuple<Guid?, Accepted>> _state = 
        new ConcurrentDictionary<Guid, Tuple<Guid?, Accepted>>();

    internal void Accept(Guid orderId, Accepted accepted, Guid? clientId = null)
    {
        _state.AddOrUpdate(orderId, new Tuple<Guid?, Accepted>(clientId, accepted),
            (guid, oldValue) => new Tuple<Guid?, Accepted>(
                oldValue.Item1 ?? clientId, oldValue.Item2 | accepted));

        Notify(orderId);
    }

    private void Notify(Guid orderId)
    {
        var booking = _state[orderId];

        switch (booking.Item2)
        {
            case Accepted.All:
                Console.WriteLine($"Умпешно забронированно для клиента - {booking.Item1}");
                _state.Remove(orderId, out _);
                break;
            case Accepted.Rejected:
                Console.WriteLine($"Гость {booking.Item1},к сожалению, все столики заняты.");
                _state.Remove(orderId, out _);
                break;
            case Accepted.Kitchen:
            case Accepted.Booking:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}