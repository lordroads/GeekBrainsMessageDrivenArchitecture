namespace BookingTable.Services.Impl;

public class SmsNotification : NotificationAbstract
{
    protected override void Send(string message)
    {
        Thread.Sleep(new Random().Next(3000, 5000));

        Console.WriteLine($"[SMS]: {message}");
    }
}