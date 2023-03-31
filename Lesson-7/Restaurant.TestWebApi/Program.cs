using MassTransit;
using MassTransit.Audit;
using Messaging.Exceptions;
using Messaging.Interfaces;
using Messaging.Interfaces.Impl;
using Messaging.Models.Requests;
using Prometheus;
using Restaurant.Booking;
using Restaurant.Booking.Consumers;
using Restaurant.Booking.Saga;
using Restaurant.Booking.Services;
using Restaurant.Booking.Services.Impl;
using Restaurant.TestWebApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.WebHost
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
                            var auditStore = context.GetService<IMessageAuditStore>();
                            cfg.UsePrometheusMetrics(serviceName: "booking_service");
                            cfg.ConnectSendAuditObservers(auditStore);
                            cfg.ConnectConsumeAuditObserver(auditStore);

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
                    services.AddSingleton<IMessageAuditStore, AuditStore>();

                    services.AddHostedService<ClientHostedService>();

                    services.AddControllers();

                });

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseRouting();
app.UseHttpsRedirection();

app.UseEndpoints(endpoint =>
{
    endpoint.MapMetrics();
    endpoint.MapControllers();
});

app.Run();