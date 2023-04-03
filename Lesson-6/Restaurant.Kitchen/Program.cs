using MassTransit;
using Messaging.Interfaces;
using Messaging.Interfaces.Impl;
using Messaging.Models.Requests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Restaurant.Kitchen.Consumers;
using Restaurant.Kitchen.Services;
using Restaurant.Kitchen.Services.Impl;

namespace Restaurant.Kitchen;

internal class Program
{
    static void Main(string[] args)
    {
        Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<KitchenConsumer>()
                            .Endpoint(e => e.Temporary = true);

                        x.AddConsumer<KitchenCancellationConsumer>()
                            .Endpoint(e => e.Temporary = true);

                        x.AddDelayedMessageScheduler();

                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.UseDelayedMessageScheduler();
                            cfg.UseInMemoryOutbox();
                            cfg.ConfigureEndpoints(context);

                            cfg.UseMessageRetry(cfg =>
                            {
                                cfg.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2));
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


                    services.AddSingleton<IInMemoryRepository<KitchenRequestModel>, InMemoryRepository<KitchenRequestModel>>(); 
                    services.AddTransient<IKitchenService, KitchenService>();
                })
                .Build()
                .Run();
    }
}