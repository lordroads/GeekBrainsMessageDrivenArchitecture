using MassTransit;
using Messaging.Exceptions;
using Messaging.Interfaces;
using Messaging.Interfaces.Impl;
using Messaging.Models.Requests;
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
                            .Endpoint(e => e.Temporary = true)
                            .InMemoryRepository();

                        x.AddDelayedMessageScheduler();

                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.UseDelayedMessageScheduler();
                            cfg.UseInMemoryOutbox();
                            cfg.ConfigureEndpoints(context);

                            cfg.UseMessageRetry(cfg =>
                            {
                                cfg.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2));

                                cfg.Ignore<LasagnaException>();
                            });

                            cfg.UseScheduledRedelivery(cfg =>
                            {
                                cfg.Intervals(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(6));
                            });
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
                    services.AddSingleton<IInMemoryRepository<BookingRequestModel>, InMemoryRepository<BookingRequestModel>>(); 

                    services.AddHostedService<ClientWorker>();
                })
                .Build()
                .Run();
        }
    }
}