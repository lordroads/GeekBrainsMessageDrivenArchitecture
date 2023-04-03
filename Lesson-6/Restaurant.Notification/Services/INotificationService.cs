using Messaging.Interfaces;

namespace Restaurant.Notification.Services;

public interface INotificationService
{
    public void Notify(INotify notify);
}