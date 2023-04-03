using MassTransit;
using Messaging.Interfaces;
using Messaging.Interfaces.Impl;
using Messaging.Models.Requests;
using Restaurant.Kitchen.Consumers;
using Restaurant.Kitchen.Services;
using Restaurant.Kitchen.Services.Impl;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus;
using MassTransit.Audit;

namespace Restaurant.Kitchen;

internal class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration
            .AddJsonFile("appsettings.json");

        builder.WebHost
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
                            var auditStore = context.GetService<IMessageAuditStore>();
                            cfg.UsePrometheusMetrics(serviceName: "kitchen_service");
                            cfg.ConnectSendAuditObservers(auditStore);
                            cfg.ConnectConsumeAuditObserver(auditStore);

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
                    services.AddSingleton<IMessageAuditStore, AuditStore>();

                    services.AddControllers();
                });

        var app = builder.Build();

        app.UseRouting();
        app.UseHttpsRedirection();

        app.UseEndpoints(configure =>
        {
            configure.MapMetrics();
            configure.MapControllers();
        });

        app.Run();
    }
}