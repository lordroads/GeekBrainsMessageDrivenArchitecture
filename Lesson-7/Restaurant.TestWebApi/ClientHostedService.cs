using Restaurant.Booking.Services;
using System.Diagnostics;
using System.Threading;

namespace Restaurant.TestWebApi
{
    public class ClientHostedService : IHostedService
    {
        private readonly IHostasService _hostasService;

        public ClientHostedService(IHostasService hostasService)
        {
            _hostasService = hostasService;
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            DoWorker(cancellationToken);

            return Task.CompletedTask;
        }

        private async Task DoWorker(CancellationToken stoppingToken)
        {
            var t = new Thread(() =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    Console.WriteLine("Привет! Чтобы забронировать столик отправте \"1\"");

                    if (!int.TryParse(Console.ReadLine(), out var choice) && new[] { 1 }.Contains(choice))
                    {
                        continue;
                    }

                    var stopWatch = new Stopwatch();
                    stopWatch.Start();

                    switch (choice)
                    {
                        case 1:
                            _hostasService.InitBookingTable(new Random().Next(2, 10), stoppingToken);
                            break;
                        default:
                            break;
                    }

                    Console.WriteLine("Спасибо за Ваше обращение!");
                    stopWatch.Stop();

                    var ts = stopWatch.Elapsed;
                    Console.WriteLine($"{ts.Seconds:00}:{ts.Milliseconds:00}");
                }
            });

            t.Start();

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
