using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Restaurant.Booking.Consumers;
using Restaurant.Booking.Saga;
using Restaurant.Booking.Services;
using Restaurant.Booking.Services.Impl;

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
                        x.AddConsumer<BookingConsumer>()
                            .Endpoint(e => e.Temporary = true);

                        x.AddConsumer<BookingKitchenFaultConsumer>()
                            .Endpoint(e => e.Temporary = true);

                        x.AddConsumer<BookingCancellationConsumer>()
                            .Endpoint(e => e.Temporary = true);

                        x.AddSagaStateMachine<RestaurantBookingSaga, RestaurantBooking>()
                            .InMemoryRepository();

                        x.AddDelayedMessageScheduler();

                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.UseDelayedMessageScheduler();
                            cfg.UseInMemoryOutbox();
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

                    services.AddTransient<IBookingService, BookingService>();
                    services.AddTransient<IHostasService, HostasService>();
                    services.AddSingleton<IRepositoryTable, RepositoryTable>();

                    services.AddHostedService<ClientWorker>();
                })
                .Build()
                .Run();
        }
    }
}