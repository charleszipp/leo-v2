using Phase.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Phase.Providers
{
    //This should only be used in the context of unit testing so excluding from code coverage
    [ExcludeFromCodeCoverage]
    public class IdemptotentTestingEventsPublisher : IEventsPublisher
    {
        protected readonly HandlersFactory _factory;

        public IdemptotentTestingEventsPublisher(HandlersFactory factory) => _factory = factory;

        public void Publish(IEnumerable<IEvent> events)
        {
            // we want these to be sent to the subscribers in the order they occurred
            // if they are subscribed to multiple events, this will ensure they get them in the order
            // that they were produced
            foreach (var e in events.OrderBy(e => e.SequenceId))
            {
                var invoker = (EventInvoker)Activator.CreateInstance(typeof(EventInvoker<>).MakeGenericType(e.GetType()));
                invoker.Invoke(e, _factory);
                //publish the event a second time to test if the subscriber is idemptotent
                invoker.Invoke(e, _factory);
            }
        }

        public void Publish(IEvent @event)
        {
            var invoker = (EventInvoker)Activator.CreateInstance(typeof(EventInvoker<>).MakeGenericType(@event.GetType()));
            invoker.Invoke(@event, _factory);
            //publish the event a second time to test if the subscriber is idemptotent
            invoker.Invoke(@event, _factory);
        }
    }
}