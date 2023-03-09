using MassTransit;
using Messaging.Interfaces.Impl;

namespace BookingTable.Services.Impl;

public class SmsNotification : NotificationAbstract
{
    private readonly IBus _bus;

    public SmsNotification(IBus bus)
    {
        _bus = bus;
    }

    protected override async Task Send(TableBooked message, CancellationToken cancellationToken)
    {
        await _bus.Publish(message,
            context => context.Durable = false,
            cancellationToken);
    }
}