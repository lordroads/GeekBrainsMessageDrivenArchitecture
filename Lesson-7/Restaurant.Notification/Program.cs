﻿using Prometheus;
using MassTransit;
using MassTransit.Audit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Restaurant.Notification.Consumers;
using Restaurant.Notification.Services;
using Restaurant.Notification.Services.Impl;

namespace Restaurant.Notification;

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
                        x.AddConsumer<NotifierConsumer>()
                            .Endpoint(c => c.Temporary = true);

                        x.AddDelayedMessageScheduler();

                        x.UsingRabbitMq((context, cfg) =>
                        {
                            var auditStore = context.GetRequiredService<IMessageAuditStore>();
                            cfg.UsePrometheusMetrics(serviceName: "notification_service");
                            cfg.ConnectConsumeAuditObserver(auditStore);
                            cfg.ConnectSendAuditObservers(auditStore);

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


                    services.AddTransient<INotificationService, NotificationService>();
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