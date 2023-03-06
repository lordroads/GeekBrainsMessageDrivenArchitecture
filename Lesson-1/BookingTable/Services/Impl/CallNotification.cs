namespace BookingTable.Services.Impl;

public class CallNotification : NotificationAbstract
{
    protected override void Send(string message)
    {
        Thread.Sleep(new Random().Next(100, 500));

        Console.WriteLine($"[CALL]: {message}");
    }
}