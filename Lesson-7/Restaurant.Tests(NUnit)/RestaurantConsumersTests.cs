using MassTransit;
using Messaging.Interfaces.Impl;
using Messaging.Interfaces;
using Messaging.Models.Requests;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Booking.Consumers;
using Restaurant.Booking.Services.Impl;
using Restaurant.Booking.Services;
using MassTransit.Testing;
using Restaurant.Kitchen.Services.Impl;
using Restaurant.Kitchen.Services;
using Restaurant.Kitchen.Consumers;
using Messaging.Models;
using Restaurant.Notification.Consumers;
using Restaurant.Notification.Services.Impl;
using Restaurant.Notification.Services;

namespace Restaurant.Tests_NUnit_
{
    [TestFixture]
    public class RestaurantConsumersTests
    {
        private ServiceProvider _provider;
        private InMemoryTestHarness _harness;


        [OneTimeSetUp]
        public async Task Init()
        {
            _provider = new ServiceCollection()
                .AddMassTransitInMemoryTestHarness(cfg =>
                {
                    cfg.AddConsumer<BookingConsumer>();
                    cfg.AddConsumer<KitchenConsumer>();
                    cfg.AddConsumer<NotifierConsumer>();
                })
                .AddLogging()
                .AddTransient<IBookingService, BookingService>()
                .AddSingleton<IRepositoryTable, RepositoryTable>()
                .AddTransient<IKitchenService, KitchenService>()
                .AddTransient<INotificationService, NotificationService>()
                .AddSingleton<IInMemoryRepository<BookingRequestModel>, InMemoryRepository<BookingRequestModel>>()
                .AddSingleton<IInMemoryRepository<KitchenRequestModel>, InMemoryRepository<KitchenRequestModel>>()
                .BuildServiceProvider(true);

            _harness = _provider.GetRequiredService<InMemoryTestHarness>();

            await _harness.Start();
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await _harness.Stop();
            await _provider.DisposeAsync();
        }

        [Test]
        public async Task Any_Booking_Request_Consumed()
        {
            
            await _harness.Bus.Publish(
                new BookingRequest(
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    1,
                    null));

            Assert.That(await _harness.Consumed.Any<IBookingRequest>());

            await _harness.OutputTimeline(TestContext.Out, opt => opt.Now().IncludeAddress());
        }

        [Test]
        public async Task Booking_Request_Consumer_Published_Table_Booked_Message()
        {
            var orderId = NewId.NextGuid();
            var clientId = NewId.NextGuid();

            var bus = _provider.GetRequiredService<IBus>();

            await bus.Publish<IBookingRequest>(
                new BookingRequest(
                    orderId,
                    clientId,
                    1,
                    null));
            
            Assert.That(_harness.Consumed.Select<IBookingRequest>()
                .Any(x => x.Context.Message.OrderId == orderId), Is.True);

            Assert.That(_harness.Published.Select<ITableBooked>()
                .Any(x => x.Context.Message.OrderId == orderId), Is.True);

            await _harness.OutputTimeline(TestContext.Out, opt => opt.Now().IncludeAddress());
        }

        [Test]
        public async Task Any_Table_Booked_Consumed()
        {

            await _harness.Bus.Publish(
                new TableBooked(
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    1,
                    true,
                    new Dish(1, "Стейк из семги")));

            Assert.That(await _harness.Consumed.Any<ITableBooked>());

            await _harness.OutputTimeline(TestContext.Out, opt => opt.Now().IncludeAddress());
        }

        [Test]
        public async Task Fault_Table_Booked_Consumed()
        {

            await _harness.Bus.Publish(
                new TableBooked(
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    1,
                    true,
                    null));

            Assert.That(await _harness.Consumed.Any<ITableBooked>());

            Assert.That(await _harness.Published.Any<Fault<ITableBooked>>());

            await _harness.OutputTimeline(TestContext.Out, opt => opt.Now().IncludeAddress());
        }

        [Test]
        public async Task Any_Notify_Booked_Consumed()
        {

            await _harness.Bus.Publish(
                new Notify(
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    "Test"));

            Assert.That(await _harness.Consumed.Any<INotify>());

            await _harness.OutputTimeline(TestContext.Out, opt => opt.Now().IncludeAddress());
        }
    }
}