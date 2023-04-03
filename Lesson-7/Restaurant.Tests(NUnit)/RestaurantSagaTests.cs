using MassTransit.Testing;
using Messaging.Interfaces.Impl;
using Messaging.Interfaces;
using Messaging.Models.Requests;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Booking.Services.Impl;
using Restaurant.Booking.Services;
using Restaurant.Kitchen.Services.Impl;
using Restaurant.Kitchen.Services;
using Restaurant.Notification.Services.Impl;
using Restaurant.Notification.Services;
using Restaurant.Booking.Consumers;
using Restaurant.Kitchen.Consumers;
using Restaurant.Notification.Consumers;
using Restaurant.Booking.Saga;
using MassTransit;
using Messaging.Models;

namespace Restaurant.Tests_NUnit_
{
    [TestFixture]
    public class RestaurantSagaTests
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
                    cfg.AddSagaStateMachine<RestaurantBookingSaga, RestaurantBooking>()
                        .InMemoryRepository();
                    cfg.AddSagaStateMachineTestHarness<RestaurantBookingSaga, RestaurantBooking>();
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
        public async Task Should_Be_Easy()
        {
            var orderId = NewId.NextGuid();
            var clientId = NewId.NextGuid();

            await _harness.Bus.Publish(
                new BookingRequest(
                    orderId,
                    clientId,
                    1,
                    new Dish(1, "Стейк из семги")));

            Assert.That(await _harness.Published.Any<IBookingRequest>());
            Assert.That(await _harness.Consumed.Any<IBookingRequest>());

            var sagaHarness = _provider
                .GetRequiredService<IStateMachineSagaTestHarness<RestaurantBooking, RestaurantBookingSaga>>();

            Assert.That(await sagaHarness.Consumed.Any<IBookingRequest>());
            Assert.That(await sagaHarness.Created.Any(x => x.CorrelationId == orderId));

            var saga = sagaHarness.Created.Contains(orderId);

            Assert.That(saga, Is.Not.Null);
            Assert.That(saga.ClientId, Is.EqualTo(clientId));
            Assert.That(await _harness.Published.Any<ITableBooked>());
            Assert.That(await _harness.Published.Any<IKitchenReady>());

            await _harness.OutputTimeline(TestContext.Out, opt => opt.Now().IncludeAddress());
        }
    }
}
