using System;
using System.Collections.Generic;
using System.Threading;

namespace EventThrottlingExample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Створюємо шину подій з обмеженням 2 події/секунду
            var eventBus = new ThrottledEventBus(2);

            // Реєструємо обробник першої події
            var handler1 = new EventHandler(() => Console.WriteLine("Handling event 1"));
            eventBus.Subscribe("event1", handler1);

            // Реєструємо обробник другої події
            var handler2 = new EventHandler(() => Console.WriteLine("Handling event 2"));
            eventBus.Subscribe("event2", handler2);

            // Відправляємо події з різною частотою
            for (int i = 0; i < 10; i++)
            {
                eventBus.Publish("event1");
                Thread.Sleep(200);
            }

            for (int i = 0; i < 10; i++)
            {
                eventBus.Publish("event2");
                Thread.Sleep(500);
            }

            // Скасовуємо реєстрацію обробника першої події
            eventBus.Unsubscribe("event1", handler1);

            // Відправляємо ще декілька подій
            for (int i = 0; i < 5; i++)
            {
                eventBus.Publish("event1");
                eventBus.Publish("event2");
                Thread.Sleep(200);
            }

            Console.ReadKey();
        }
    }

    public class ThrottledEventBus
    {
        private readonly Dictionary<string, List<EventHandler>> _eventHandlers;
        private readonly int _throttleMilliseconds;
        private readonly object _lockObject;

        public ThrottledEventBus(int eventsPerSecond)
        {
            _eventHandlers = new Dictionary<string, List<EventHandler>>();
            _throttleMilliseconds = 1000 / eventsPerSecond;
            _lockObject = new object();
        }

        public void Subscribe(string eventName, EventHandler handler)
        {
            lock (_lockObject)
            {
                if (!_eventHandlers.ContainsKey(eventName))
                {
                    _eventHandlers[eventName] = new List<EventHandler>();
                }

                _eventHandlers[eventName].Add(handler);
            }
        }

        public void Unsubscribe(string eventName, EventHandler handler)
        {
            lock (_lockObject)
            {
                if (_eventHandlers.ContainsKey(eventName))
                {
                    _eventHandlers[eventName].Remove(handler);
                }
            }
        }

        public void Publish(string eventName)
        {
            List<EventHandler> handlersToInvoke = null;

            lock (_lockObject)
            {
                if (_eventHandlers.ContainsKey(eventName))
                {
                    handlersToInvoke = new List<EventHandler>(_eventHandlers[eventName]);
                }
            }

            if (handlersToInvoke != null)
            {
                foreach (var handler in handlersToInvoke)
                {
                    handler();
                    Thread.Sleep(_throttleMilliseconds);
                }
            }
        }
    }

    public delegate void EventHandler();
}

