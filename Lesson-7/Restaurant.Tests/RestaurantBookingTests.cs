using MassTransit;
using Messaging.Interfaces.Impl;
using Messaging.Interfaces;
using Messaging.Models.Requests;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Restaurant.Booking.Consumers;
using Restaurant.Booking.Services.Impl;
using Restaurant.Booking.Services;
using MassTransit.Testing;
using Assert = NUnit.Framework.Assert;

namespace Restaurant.Tests
{
    public class RestaurantBookingTests
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
                    cfg.AddConsumerContainerTestHarness<BookingConsumer>();
                })
                .AddLogging()
                .AddTransient<IBookingService, BookingService>()
                .AddTransient<IHostasService, HostasService>()
                .AddSingleton<IRepositoryTable, RepositoryTable>()
                .AddSingleton<IInMemoryRepository<BookingRequestModel>, InMemoryRepository<BookingRequestModel>>()
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
    }
}