using BookingTable.Enums;
using BookingTable.Services;
using System.Threading;

namespace BookingTable;

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

    public void StartBookFreeTableThread(int countOfPersons)
    {
        BookFreeTable(countOfPersons);
    }

    private void BookFreeTable(int countOfPersons)
	{
        _notificationFactory
            .GetNotification(NotificationType.CALL)
            .SendAsync("Добрый день! Подождите секунду я подберу столик и потвержу" +
            "вашу бронь, оставайтесь на линии...");

        mut.WaitOne();

        var table = _tables.FirstOrDefault(table => table.SeatsCount > countOfPersons 
											&& table.State == State.Free);

		Thread.Sleep(1000 * 5);

		mut.ReleaseMutex();

        _notificationFactory
            .GetNotification(NotificationType.CALL)
            .SendAsync(table is null
            ? "...К сожалению, сейчас все столики заняты"
            : $"...Готово! Ваш столик номер {table.Id}");
    }

    public void StartBookFreeTableAsyncThread(int countOfPersons)
    {
        Thread newThread = new Thread(() => BookFreeTableAsync(countOfPersons));
        newThread.Name = String.Format("BookFreeTable");
        newThread.Start();
    }

    private void BookFreeTableAsync(int countOfPersons)
	{
        _notificationFactory
            .GetNotification(NotificationType.CALL)
            .SendAsync("Добрый день! Подождите секунду я подберу столик и подтвержу" +
            "вашу бронь, Вам придет уведомление.");

		
        mut.WaitOne();

        var table = _tables.FirstOrDefault(table => table.SeatsCount > countOfPersons
                                            && table.State == State.Free);

        Thread.Sleep(1000 * 5);
	    table?.SetState(State.Booked);

        mut.ReleaseMutex();

        _notificationFactory
            .GetNotification(NotificationType.SMS)
            .SendAsync(table is null
            ? "К сожалению, сейчас все столики заняты"
            : $"Готово! Ваш столик номер {table.Id}");
    }

    public void StartRemoveTableBookingToIdThread(int id)
    {
        RemoveTableBookingToId(id);
    }

    private void RemoveTableBookingToId(int id)
	{
        _notificationFactory
            .GetNotification(NotificationType.CALL)
            .SendAsync("Снова Здравствуйте! Подождите секунду я найду стол" +
            "что отменить бронирование, повисите на линии...");

        mut.WaitOne();

        var table = _tables.FirstOrDefault(table => table.Id == id);

		Thread.Sleep(1000 * 5);

		if (table is null)
		{
            _notificationFactory
            .GetNotification(NotificationType.CALL)
            .SendAsync("Данного стола нет в списке бронированных.");
        }

		var result = table.SetState(State.Free);

        mut.ReleaseMutex();

        _notificationFactory
            .GetNotification(NotificationType.CALL)
            .SendAsync(result
            ? $"...Спасибо за ожидание, со стола номер #{table.Id} снята бронь."
            : $"...Стол не был забронирован, он остался свободным.");
    }

    public void StartRemoveTableBookingToIdAsyncThread(int id)
    {
        Thread newThread = new Thread(() => RemoveTableBookingToIdAsync(id));
        newThread.Name = String.Format("RemoveTableBookingToId");
        newThread.Start();
    }

    private void RemoveTableBookingToIdAsync(int id)
	{
        _notificationFactory
            .GetNotification(NotificationType.CALL)
            .SendAsync("Снова Здравствуйте! Я найду стол" +
            "что отменить бронирование, и отправлю Вам оповещение." +
            "Всего доброго!");

        mut.WaitOne();

        var table = _tables.FirstOrDefault(table => table.Id == id);

        Thread.Sleep(1000 * 5);

        if (table is null)
        {
            _notificationFactory
                .GetNotification(NotificationType.SMS)
                .SendAsync("Данного стола нет в списке бронированных.");
        }

        var result = table.SetState(State.Free);

        mut.ReleaseMutex();

        _notificationFactory
            .GetNotification(NotificationType.SMS)
            .SendAsync(result
                ? $"Спасибо, что оповестили нас, со стола номер #{table.Id} снята бронь."
                : "Спасибо, что оповестили нас, стол не был забронирован, он остался свободным.");
    }

	private void AutoRemoveTableBookingAsync(object item)
	{
		Task.Run(() =>
		{
            mut.WaitOne();

            var table = item as Table;
			if (table.State == State.Booked)
			{
                table.SetState(State.Free);
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