using BookingTable.Services;
using BookingTable.Services.Impl;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Restaurant.Booking
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddMassTransit(x =>
                    {
                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.ConfigureEndpoints(context);
                        });
                    });

                    services.Configure<MassTransitHostOptions>(options =>
                    {
                        options.WaitUntilStarted = true;
                        options.StartTimeout = TimeSpan.FromSeconds(30);
                        options.StopTimeout = TimeSpan.FromMinutes(1);
                    });

                    services.Configure<HostOptions>(
                        opts => opts.ShutdownTimeout = TimeSpan.FromMinutes(1));


                    services.AddTransient<Restaurant>();
                    services.AddTransient<INotificationFactory, NotificationFactory>();

                    services.AddHostedService<Worker>();
                })
                .Build()
                .Run();
        }
    }
}