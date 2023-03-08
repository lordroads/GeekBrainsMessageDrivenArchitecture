using BookingTable.Services.Impl;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;

namespace BookingTable
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var restaurant = new Restaurant(new NotificationFactory());
            while (true)
            {
                Console.WriteLine("Привет! Желаете забронировать столик?\n" +
                "1 - мы уведомим Вас по смс (асинхронно)\n" +
                "2 - подождите на линии, мы Вас оповестим (синхронно)\n" +
                "3 - отмена бронирования по смс (асинхронно)\n" +
                "4 - отмена бронирования по телефону (синхронно)");

                if (!int.TryParse(Console.ReadLine(), out var choice) && new[] {1,2,3,4}.Contains(choice))
                {
                    Console.WriteLine("ВВедите, поджалуста от 1 до  4-х.");
                    continue;
                }

                var stopWatch = new Stopwatch();
                stopWatch.Start();

                switch (choice)
                {
                    case 1:
                        restaurant.StartBookFreeTableAsyncThread(1);
                        break;
                    case 2:
                        restaurant.StartBookFreeTableThread(1);
                        break;
                    case 3:
                        restaurant.StartRemoveTableBookingToIdAsyncThread(GetTableId());
                        break;
                    case 4:
                        restaurant.StartRemoveTableBookingToIdThread(GetTableId());
                        break;
                    default:
                        break;
                }

                Console.WriteLine("Спасибо за Ваше обращение!");
                stopWatch.Stop();

                var ts = stopWatch.Elapsed;
                Console.WriteLine($"{ts.Seconds:00}:{ts.Milliseconds:00}");
            }
        }

        private static int GetTableId()
        {
            Console.WriteLine("Укажите номер стола.");
            if (!int.TryParse(Console.ReadLine(), out int tableId))
            {
                Console.WriteLine("Вы ввели не число.");
            }

            return tableId;
        }
    }
}