using Phase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Phase.Providers
{
    public class EventsPublisher : IEventsPublisher
    {
        protected readonly HandlersFactory _factory;

        public EventsPublisher(HandlersFactory factory)
        {
            _factory = factory;
        }

        public void Publish(IEnumerable<IEvent> events)
        {
            // we want these to be sent to the subscribers in the order they occurred
            // if they are subscribed to multiple events, this will ensure they get them in the order
            // that they were produced
            foreach (var e in events.OrderBy(e => e.SequenceId))
            {
                var invoker = (EventInvoker)Activator.CreateInstance(typeof(EventInvoker<>).MakeGenericType(e.GetType()));
                invoker.Invoke(e, _factory);
                // todo: log error, indicate the order the events were published, and which failed
            }
        }

        public void Publish(IEvent @event)
        {
            var invoker = (EventInvoker)Activator.CreateInstance(typeof(EventInvoker<>).MakeGenericType(@event.GetType()));
            invoker.Invoke(@event, _factory);
        }
    }
}