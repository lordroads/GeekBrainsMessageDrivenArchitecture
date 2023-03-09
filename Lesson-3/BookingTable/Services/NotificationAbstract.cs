using Messaging.Interfaces.Impl;

namespace BookingTable.Services;

public abstract class NotificationAbstract
{
    private protected NotificationAbstract() {  }

    public async Task SendAsync(TableBooked message, CancellationToken cancellationToken)
    {
        Task<Task> task = new Task<Task>(() => Send(message, cancellationToken));
        task.Start();

        await task.Result;
    }
    protected abstract Task Send(TableBooked message, CancellationToken cancellationToken);
}