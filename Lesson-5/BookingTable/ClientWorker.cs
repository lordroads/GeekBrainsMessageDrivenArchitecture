using Microsoft.Extensions.Hosting;
using Restaurant.Booking.Services;
using System.Diagnostics;

namespace Restaurant.Booking;

public class ClientWorker : BackgroundService
{
    private readonly IHostasService _hostasService;

    public ClientWorker(IHostasService hostasService)
    {
        _hostasService = hostasService;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine("Привет! Чтобы забронировать столик отправте \"1\"");

            if (!int.TryParse(Console.ReadLine(), out var choice) && new[] {1}.Contains(choice))
            {
                continue;
            }

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            switch (choice)
            {
                case 1:
                    _hostasService.InitBookingTable(new Random().Next(2,10), stoppingToken);
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
}
