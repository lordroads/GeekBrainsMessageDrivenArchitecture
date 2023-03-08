namespace BookingTable.Services;

public abstract class NotificationAbstract
{
    public void SendAsync(string message)
    {
        Thread newThread = new Thread(() => Send(message));
        newThread.Name = "Send_Notification_Message";
        newThread.Start();
    }
    protected abstract void Send(string message);
}