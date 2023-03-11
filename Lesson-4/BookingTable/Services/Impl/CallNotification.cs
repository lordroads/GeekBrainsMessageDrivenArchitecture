using Messaging.Interfaces.Impl;

namespace BookingTable.Services.Impl;

public class CallNotification : NotificationAbstract
{
    protected override Task Send(TableBooked message, CancellationToken cancellationToken)
    {
        Task task = new Task(() =>
        {
            Thread.Sleep(new Random().Next(100, 500));

            Console.WriteLine($"[CALL]: {message.Message} \n" +
                $"Order_ID: {message.OrderId}\n" +
                $"Client_ID: {message.ClientId}\n" +
                $"Dish: {message.PreOrder}\n" +
                "Seccess: " + (message.Success ? "ондрбепфдемн" : "ме ондрбепфдемн") + "\"");
        });

        task.Start();

        return Task.CompletedTask;
    }
}