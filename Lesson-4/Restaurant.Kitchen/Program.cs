using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Restaurant.Kitchen.Consumers;

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
                        x.AddConsumer<KitchenTableBookedConsumer>();

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


                    services.AddTransient<Manager>();
                })
                .Build()
                .Run();
    }
}