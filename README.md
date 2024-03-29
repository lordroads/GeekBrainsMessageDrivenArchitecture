# GeekBrainsMessageDrivenArchitecture

## Урок 1

* Создать возможность снять бронь синхронно и асинхронно, используя для этого
номер забронированного стола;
* Выделить логику для отправки уведомления в отдельный класс, он будет
отвечать за все вопросы связанные с коммуникациями с клиентами, добавить
задержку (они будет имитировать создание сообщения) и сделать вызов
уведомления асинхронным;
* Добавить автоматическое “снятие брони”. Например, раз в 20 секунд при
наличии забронированных мест - бронь должна слетать. Асинхронно, независимо
от ввода-вывода. Подсказка: можно использовать таймер.
* (*) Добавить синхронизацию бронирований для множественных асинхронных и
синхронных вызовов, это значит, что бронируя столики не дожидаясь
предыдущих ответов мы должны получать последовательный результат.
* (**) Добавить ограничение на количество мест за столом относительно
количества гостей, например, если придет 5 человек, то сесть можно только за
стол где больше 5 мест. Столы сдвигать можно. Должна быть синхронизация
между разными одновременными бронированиями.

## Урок 2

* Приложение из первого урока переделать на работу с RabbitMQ. Выделенную в
отдельный класс логику отправки уведомления перенести в новое консольное
приложение, реализовать взаимодействие через очередь;
* Переделать работу с очередью на шаблон publisher/subscriber
(издатель/подписчик). Сообщения не должны отправляться напрямую в очередь,
каждый подписчик должен получить сообщение от издателя. То есть если мы
запустим пять сервисов уведомлений, каждый из них должен получить и
обработать сообщение https://www.rabbitmq.com/tutorials/tutorial-three-dotnet.html
* (\*) Изменить работу на режим обменника topic с маской сообщения. Создать две
подписки, в которые по ключу будут поступать сообщения. Ключи для очередей:
“sms.*” и “email.*”. В качестве звездочки может быть любое слово.
Соответственно эти ключи должны случайно генерироваться из сервиса
бронирования при отправке уведомлений.
* (**) Предложить свою реализацию библиотеки Messaging. Например, если мы
захотим использовать не RabbitMQ, а нечто другое это должно быть легко
переключаемым. Автоматическое переподключение к брокеру при возникновении
проблем на нем. Конфигурация флагов (durable, autoack и т.д). Необходимо
мыслить критически и не ограничивать себя в творчестве.

## Урок 3

* Используя весь опыт предыдущих уроков и код из методички воспроизвести
приложение описанное в тексте материала;
* Добавить в сервис кухни неожиданные поломки или попадание блюд в стоп-лист,
которые вызывают реакцию в других сервисах: отправляется уведомление о
снятии брони с извинениями и происходит снятие бронирования. Никаких прямых
команд между сервисами быть не должно, только данные в публикациях на
которые будут реагировать остальные сервисы.
* (\*) Добавить возможность отправлять сообщение через обменник с типом direct
из сервиса бронирований на кухню с синхронным ожиданием ответа. Для этого
придется использовать заголовки и конфигурацию. Сообщение должно
содержать следующий смысл “Когда обед?”. Кухня может отвечать от 0,5 до 3
секунд любым сообщением. Если время ответа превышает 1,5 секунд в более
чем 10 случаев из последних 30 - необходимо чтобы срабатывал шаблон
“Предохранитель”.

## Урок 4

* Используя весь опыт предыдущих уроков и код из методички воспроизвести
приложение описанное в тексте материала;
* После подтверждения бронирования добавить статус “Ожидание гостя”.
Добавить генерацию сообщения “Гость прибыл” с таймаутом в диапазоне от 7 до
15 секунд. Запуск генерации события “Гость прибыл” должен происходить только
после перехода в статус “Ожидание гостя”. После получения сообщения “Гость
прибыл” - сага переводится в статус Final.
* Гость при бронировании указывает через какое время он придет. Например,
через 10 секунд. Это значение должно генерироваться случайно в диапазоне от 7
до 15 секунд сервисом бронирования. Если гость не приходит указанном
диапазоне после перехода в статус “Ожидание гостя” - мы должны отправить ему
сообщение о том, что его бронь снята и перевести сагу в статус Final.
* (*) Добавить персистентность используя любое из предложенных в документации
хранилищ

## Урок 5

* Используя весь опыт предыдущих уроков и код из методички воспроизвести
приложение описанное в тексте материала;
* Настроить политики повторов и повторной доставки для всех потребителей;
* Реализовать тип Fault\<T> для всех потребителей, добавить в сагу
компенсационное действие - отмена предыдущих действий. Например, при
ошибке в бронировании, нужно отозвать заказ на кухне].
* Добавить в сервис Kitchen правило, что мы не принимаем предзаказ с типом
Lasagna: добавить генерацию исключения, в сервисе бронирования реализовать
каждый четвертый запрос на бронирование с Lasagna;
* Включить Transaction Outbox для всех сервисов. Задание на отладку: добиться
срабатывания работы паттерна Transaction Outbox реализованного в Mass Transit
“наяву”, чтобы лучше усвоить его работу;
* (*) Реализовать Transaction Outbox в базе данных. Заниматься вопросом работы
при кластеризации и блокировке записей не нужно.

## Урок 6

* Используя весь опыт предыдущих уроков и код из методички воспроизвести
приложение описанное в тексте материала;
* Реализовать второй вариант идемпотентного потребителя - с дополнительным
хранилищем (имитировать транзакционность);
* Сделать все потребители идемпотентными;
* Для хранилищ сообщений должны быть реализована очистка - сообщение
должно удаляться через 30 секунд после появления;
* (*) Реализовать идемпотентность через базу данных. Подумать о том, как быть
при наличии нескольких инстансов сервиса.

## Урок 7

* Добавить логирование во все проекты не теряя из виду уровни логирования:
Debug, Info, Error. Добавить аудит сообщений во все проекты.
* Подключить мониторинг во все проекты;
* Покрыть тестами все потребители сообщений. Учитывать не только прямые
кейсы, но и граничные случаи. Не забыть протестировать идемпотентность.
* (*) Настроить аудит сообщений в базу данных. Добавить тесты для саги, для
Scheduler и сообщений-ошибок
