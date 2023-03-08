namespace BookingTable.Services.Impl;

public class CallNotification : NotificationAbstract
{
    private static readonly Lazy<CallNotification> _instance =
        new Lazy<CallNotification>(() => new CallNotification());

    private CallNotification() {  }

    public static CallNotification GetInstance()
    {
        return _instance.Value;
    }
    protected override void Send(string message)
    {
        Thread.Sleep(new Random().Next(100, 500));

        Console.WriteLine($"[CALL]: {message}");
    }
}