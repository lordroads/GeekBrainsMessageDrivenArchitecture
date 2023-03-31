using MassTransit;
using Messaging.Interfaces;
using Messaging.Interfaces.Impl;

namespace Restaurant.Booking.Saga;

public sealed class RestaurantBookingSaga : MassTransitStateMachine<RestaurantBooking>
{
    public State AwaitingGuestComing { get; private set; }
    public State AwaitingBookingApproved { get; private set; }
    public Event<IBookingRequest> BookingRequested { get; private set; }
    public Event<ITableBooked> TableBooked { get; private set; }
    public Event<IKitchenReady> KitchenReady { get; private set; }

    public Event<Fault<IBookingRequest>> BookingRequestFault { get; private set; }
    public Event<Fault<ITableBooked>> BookingKitchenFault { get; private set; }

    public Schedule<RestaurantBooking, IBookingExpire> BookingExpired { get; private set; }
    public Schedule<RestaurantBooking, IGuestCame> GuestCame { get; private set; }
    public Schedule<RestaurantBooking, IBookingOverdue> BookingOverdue { get; private set; }
    public Event BookingApproved { get; private set; }

    public RestaurantBookingSaga()
    {
        InstanceState(x => x.CurrentState);

        Event(() => BookingRequested, x =>
            x.CorrelateById(context => context.Message.OrderId)
            .SelectId(context => context.Message.OrderId));

        Event(() => TableBooked, x =>
            x.CorrelateById(context => context.Message.OrderId));

        Event(() => KitchenReady, x =>
            x.CorrelateById(context => context.Message.OrderId));

        CompositeEvent(() => BookingApproved,
            x => x.ReadyEventStatus, KitchenReady, TableBooked);

        Event(() => BookingRequestFault, x =>
            x.CorrelateById(context => context.Message.Message.OrderId));

        Event(() => BookingKitchenFault, x =>
            x.CorrelateById(context => context.Message.Message.OrderId));

        Schedule(() => BookingExpired, x =>
            x.ExpirationId, x =>
            {
                x.Delay = TimeSpan.FromSeconds(15);
                x.Received = e => e.CorrelateById(context => context.Message.OrderId);
            });

        Schedule(() => GuestCame, x =>
            x.ExpirationIdGuestCame, x =>
            {
                x.Received = e => e.CorrelateById(context => context.Message.OrderId);
            });

        Schedule(() => BookingOverdue, x =>
            x.ExpirationIdBookingOverdue, x =>
            {
                x.Received = e => e.CorrelateById(context => context.Message.OrderId);
            });

        Initially(
            When(BookingRequested)
            .Then(context =>
            {
                context.Instance.CorrelationId = context.Data.OrderId;
                context.Instance.OrderId = context.Data.OrderId;
                context.Instance.ClientId = context.Data.ClientId;

                Console.WriteLine($"[{DateTime.Now}] Saga: {context.Data.OrderId} - {context.Message.CreationDate}");
            })
            .Schedule(BookingExpired,
                context => new BookingExpire(context.Instance),
                context => TimeSpan.FromSeconds(5))
            .TransitionTo(AwaitingBookingApproved)
            );

        During(AwaitingBookingApproved,
            When(BookingApproved)
                .Unschedule(BookingExpired)
                .Publish(context =>
                (INotify)new Notify(
                    context.Instance.OrderId,
                    context.Instance.ClientId,
                    "Стол успешно забронирован."))
                .TransitionTo(AwaitingGuestComing)
                .Schedule(GuestCame, 
                    context => new GuestCame(context.Instance),
                    context => context.Saga.ArrivalPeriod)
                .Schedule(BookingOverdue,
                    context => new BookingOverdue(context.Instance),
                    context => TimeSpan.FromSeconds(new Random().Next(7, 15))),

            When(BookingRequestFault)
                .Unschedule(BookingExpired)
                .Then(context => Console.WriteLine("Ошибочка вышла!"))
                .Publish(context => (INotify)new Notify(
                    context.Instance.OrderId,
                    context.Instance.ClientId,
                    "Приносим извинения, стол не получилось забронировать."))
                //.Publish(context => (IBookingCancellation)
                //    new BookingCancellation(context.Data.Message.ClientId))
                .Finalize(),

            When(BookingKitchenFault)
                .Unschedule(BookingExpired)
                .Then(context => Console.WriteLine($"Кухня не дает добро для {context.Message.Message.PreOrder?.Name ?? ("\"Лазаньи\"")}!"))
                .Publish(context => (INotify)new Notify(
                    context.Instance.OrderId,
                    context.Instance.ClientId,
                    $"Приносим извинения, но {context.Message.Message.PreOrder?.Name ?? ("\"Лазаньи\"")} у нас нет."))
                //.Publish(context => (IBookingCancellation)
                //    new BookingCancellation(context.Data.Message.ClientId))
                .Finalize(),

            When(BookingExpired.Received)
                .Then(context => Console.WriteLine($"Отмена заказа {context.Instance.OrderId}"))
                .Publish(context => 
                (IBookingCancellation) new BookingCancellation(
                    context.Instance.ClientId))
                .Finalize()

        );

        During(AwaitingGuestComing,
            When(GuestCame.Received)
                .Unschedule(BookingOverdue)
                .Then(context => Console.WriteLine($"Гость пришел! ({context.Instance.OrderId})"))
                .Finalize(),

            When(BookingOverdue.Received)
                .Unschedule(GuestCame)
                .Then(context => Console.WriteLine($"Упс! Бронь слетела для {context.Instance.OrderId}"))
                .Publish(context =>
                (IBookingCancellation)new BookingCancellation(
                    context.Instance.ClientId))
                .Finalize()
        );

        SetCompletedWhenFinalized();
    }
}
