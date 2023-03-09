using BookingTable;
using BookingTable.Services;
using MassTransit;
using Messaging.Interfaces.Impl;
using Restaurant.Booking.Enums;

namespace Restaurant.Booking;

public class Restaurant
{
    private static Mutex mut = new Mutex();
    private readonly List<Table> _tables = new List<Table>();
    private readonly INotificationFactory _notificationFactory;


    public Restaurant(INotificationFactory notificationFactory)
    {
        _notificationFactory = notificationFactory;

        var timerCallback = new TimerCallback(AutoRemoveTableBookingAsync);

        for (int i = 1; i < 10; i++)
        {
            var table = new Table(i);

            _tables.Add(table);

            var timer = new Timer(timerCallback, table, 20_000, 20_000);
        }
    }

    public void StartBookFreeTableThread(int countOfPersons, CancellationToken cancellationToken)
    {
        BookFreeTable(countOfPersons, cancellationToken);
    }

    private void BookFreeTable(int countOfPersons, CancellationToken cancellationToken)
    {
        _notificationFactory?
            .GetNotification(NotificationType.CALL)
            .SendAsync(
            new TableBooked(
                NewId.NextGuid(),
                NewId.NextGuid(),
                false, 
                null, 
                "Добрый день! Подождите секунду я подберу столик и потвержу" +
                "вашу бронь, оставайтесь на линии..."),
            cancellationToken);

        mut.WaitOne();

        var table = _tables.FirstOrDefault(table => table.SeatsCount > countOfPersons
                                            && table.State == Booking.Enums.State.Free);

        Thread.Sleep(1000 * 5);
        table?.SetState(Booking.Enums.State.Booked);

        mut.ReleaseMutex();

        _notificationFactory?
            .GetNotification(NotificationType.CALL)
            .SendAsync(
            new TableBooked(
                NewId.NextGuid(),
                NewId.NextGuid(),
                table is null,
                null,
                table is null
                ? "...К сожалению, сейчас все столики заняты"
                : $"...Готово! Ваш столик номер {table.Id}"),
            cancellationToken);
    }

    public void StartBookFreeTableAsyncThread(int countOfPersons, CancellationToken cancellationToken)
    {
        Thread newThread = new Thread(() => BookFreeTableAsync(countOfPersons, cancellationToken));
        newThread.Name = string.Format("BookFreeTable");
        newThread.Start();
    }

    private void BookFreeTableAsync(int countOfPersons, CancellationToken cancellationToken)
    {
        var clientId = NewId.NextGuid();
        var orderId = NewId.NextGuid();

        _notificationFactory?
            .GetNotification(NotificationType.CALL)
            .SendAsync(
            new TableBooked(
                orderId,
                clientId,
                true,
                null,
                "Добрый день! Подождите секунду я подберу столик и подтвержу" +
                "вашу бронь, Вам придет уведомление."),
            cancellationToken);

        mut.WaitOne();

        var table = _tables.FirstOrDefault(table => table.SeatsCount > countOfPersons
                                            && table.State == Booking.Enums.State.Free);

        //Thread.Sleep(1000 * 5);
        table?.SetState(Booking.Enums.State.Booked);

        var booked = table is not null ? true : false;

        mut.ReleaseMutex();

        _notificationFactory?
            .GetNotification(NotificationType.SMS)
            .SendAsync(new TableBooked(orderId, clientId, booked), cancellationToken);
    }

    public void StartRemoveTableBookingToIdThread(int id, CancellationToken cancellationToken)
    {
        RemoveTableBookingToId(id, cancellationToken);
    }

    private void RemoveTableBookingToId(int id, CancellationToken cancellationToken)
    {
        _notificationFactory?
            .GetNotification(NotificationType.CALL)
            .SendAsync(
            new TableBooked(
                NewId.NextGuid(),
                NewId.NextGuid(),
                false,
                null,
                "Подождите секунду я найду стол" +
                "что отменить бронирование, повисите на линии..."),
            cancellationToken);

        mut.WaitOne();

        var table = _tables.FirstOrDefault(table => table.Id == id);

        Thread.Sleep(1000 * 5);

        if (table is null)
        {
            _notificationFactory?
            .GetNotification(NotificationType.CALL)
            .SendAsync(
            new TableBooked(
                NewId.NextGuid(), 
                NewId.NextGuid(),
                false,
                null,
                "Данного стола нет в списке бронированных."),
            cancellationToken);
        }

        var result = table.SetState(Booking.Enums.State.Free);

        mut.ReleaseMutex();

        _notificationFactory?
            .GetNotification(NotificationType.CALL)
            .SendAsync(
            new TableBooked(
                NewId.NextGuid(),
                NewId.NextGuid(),
                false,
                null,
                result
                ? $"...Спасибо за ожидание, со стола номер #{table.Id} снята бронь."
                : $"...Стол не был забронирован, он остался свободным."),
            cancellationToken);
    }

    public void StartRemoveTableBookingToIdAsyncThread(int id, CancellationToken cancellationToken)
    {
        Thread newThread = new Thread(() => RemoveTableBookingToIdAsync(id, cancellationToken));
        newThread.Name = string.Format("RemoveTableBookingToId");
        newThread.Start();
    }

    private void RemoveTableBookingToIdAsync(int id, CancellationToken cancellationToken)
    {
        _notificationFactory?
            .GetNotification(NotificationType.CALL)
            .SendAsync(
            new TableBooked(
                NewId.NextGuid(),
                NewId.NextGuid(),
                false,
                null,
                "Я найду стол" +
                "что отменить бронирование, и отправлю Вам оповещение." +
                "Всего доброго!"),
            cancellationToken);

        mut.WaitOne();

        var table = _tables.FirstOrDefault(table => table.Id == id);

        Thread.Sleep(1000 * 5);

        if (table is null)
        {
            _notificationFactory?
            .GetNotification(NotificationType.SMS)
            .SendAsync(
                new TableBooked(
                NewId.NextGuid(),
                NewId.NextGuid(),
                false, 
                null, 
                "Данного стола нет в списке бронированных."), 
            cancellationToken);
        }

        var result = table.SetState(Booking.Enums.State.Free);

        mut.ReleaseMutex();

        _notificationFactory?
            .GetNotification(NotificationType.SMS)
            .SendAsync(
                new TableBooked(
                NewId.NextGuid(),
                NewId.NextGuid(),
                false,
                null,
                result
                ? $"Спасибо, что оповестили нас, со стола номер #{table.Id} снята бронь."
                : "Спасибо, что оповестили нас, стол не был забронирован, он остался свободным."),
            cancellationToken);
    }

    private void AutoRemoveTableBookingAsync(object item)
    {
        Task.Run(() =>
        {
            mut.WaitOne();

            var table = item as Table;
            if (table.State == Booking.Enums.State.Booked)
            {
                table.SetState(Booking.Enums.State.Free);
                Console.WriteLine($"[AUTO REMOVE] Бронь слетела (Стол #{table.Id})");
            }

            mut.ReleaseMutex();
        });
    }

    ~Restaurant()
    {
        mut.Dispose();
    }
}