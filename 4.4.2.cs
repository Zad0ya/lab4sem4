using System;
using System.Collections.Generic;
using System.Linq;

namespace lab4_sem4_2
{
    class Program
    {
        static void Main(string[] args)
        {
            var publisher = new Publisher();

       
            var highPrioritySubscriber = new Subscriber("High priority subscriber", Priority.High);
            var mediumPrioritySubscriber = new Subscriber("Medium priority subscriber", Priority.Medium);
            var lowPrioritySubscriber = new Subscriber("Low priority subscriber", Priority.Low);

            publisher.Subscribe(highPrioritySubscriber, EventName.Event1);
            publisher.Subscribe(mediumPrioritySubscriber, EventName.Event1);
            publisher.Subscribe(lowPrioritySubscriber, EventName.Event1);

            publisher.Subscribe(highPrioritySubscriber, EventName.Event2);
            publisher.Subscribe(lowPrioritySubscriber, EventName.Event2);

            publisher.Publish(EventName.Event1, "Data for event 1");
            publisher.Publish(EventName.Event2, "Data for event 2");

            Console.ReadKey();
        }
    }

    public class Publisher
    {
        private readonly Dictionary<EventName, List<Subscription>> _subscriptions;

        public Publisher()
        {
            _subscriptions = new Dictionary<EventName, List<Subscription>>();
        }

        public void Subscribe(Subscriber subscriber, EventName eventName)
        {
            if (!_subscriptions.ContainsKey(eventName))
            {
                _subscriptions[eventName] = new List<Subscription>();
            }

            _subscriptions[eventName].Add(new Subscription(subscriber, eventName));
        }

        public void Unsubscribe(Subscriber subscriber, EventName eventName)
        {
            if (_subscriptions.ContainsKey(eventName))
            {
                _subscriptions[eventName] = _subscriptions[eventName]
                    .Where(subscription => subscription.Subscriber != subscriber)
                    .ToList();
            }
        }

        public void Publish(EventName eventName, string eventData)
        {
            if (_subscriptions.ContainsKey(eventName))
            {
                var subscriptions = _subscriptions[eventName]
                    .OrderByDescending(subscription => subscription.Subscriber.Priority);

                foreach (var subscription in subscriptions)
                {
                    subscription.Subscriber.HandleEvent(eventName, eventData);
                }
            }
        }
    }

    public class Subscriber
    {
        public string Name { get; }
        public Priority Priority { get; }

        public Subscriber(string name, Priority priority)
        {
            Name = name;
            Priority = priority;
        }

        public void HandleEvent(EventName eventName, string eventData)
        {
            Console.WriteLine($"{Name} received event {eventName} with data '{eventData}'");
        }
    }

    public class Subscription
    {
        public Subscriber Subscriber { get; }
        public EventName EventName { get; }

        public Subscription(Subscriber subscriber, EventName eventName)
        {
            Subscriber = subscriber;
            EventName = eventName;
        }
    }

    public enum Priority
    {
        High,
        Medium,
        Low
    }

    public enum EventName
    {
        Event1,
        Event2
    }
}
