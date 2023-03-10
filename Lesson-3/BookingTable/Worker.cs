using MassTransit;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace Restaurant.Booking;

public class Worker : BackgroundService
{
    private readonly Restaurant _restaurant;

    public Worker(Restaurant restaurant)
    {
        _restaurant = restaurant;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine("Привет! Желаете забронировать столик?\n" +
                "1 - мы уведомим Вас по смс (асинхронно)\n" +
                "2 - подождите на линии, мы Вас оповестим (синхронно)\n" +
                "3 - отмена бронирования по смс (асинхронно)\n" +
                "4 - отмена бронирования по телефону (синхронно)");

            if (!int.TryParse(Console.ReadLine(), out var choice) && new[] { 1, 2, 3, 4 }.Contains(choice))
            {
                Console.WriteLine("ВВедите, поджалуста от 1 до  4-х.");
                continue;
            }

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            switch (choice)
            {
                case 1:
                    _restaurant.StartBookFreeTableAsyncThread(1, stoppingToken);
                    break;
                case 2:
                    _restaurant.StartBookFreeTableThread(1, stoppingToken);
                    break;
                case 3:
                    _restaurant.StartRemoveTableBookingToIdAsyncThread(GetTableId(), stoppingToken);
                    break;
                case 4:
                    _restaurant.StartRemoveTableBookingToIdThread(GetTableId(), stoppingToken);
                    break;
                default:
                    break;
            }

            Console.WriteLine("Спасибо за Ваше обращение!");
            stopWatch.Stop();

            var ts = stopWatch.Elapsed;
            Console.WriteLine($"{ts.Seconds:00}:{ts.Milliseconds:00}");
        }

        return Task.CompletedTask;
    }

    private static int GetTableId()
    {
        Console.WriteLine("Укажите номер стола.");
        if (!int.TryParse(Console.ReadLine(), out int tableId))
        {
            Console.WriteLine("Вы ввели не число.");
        }

        return tableId;
    }
}
