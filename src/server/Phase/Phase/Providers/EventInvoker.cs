using Phase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Phase.Providers
{
    public delegate IEnumerable<object> HandlersFactory(Type eventHandlerType);

    public abstract class EventInvoker
    {
        public abstract void Invoke(IEvent @event, HandlersFactory factory);

        protected IEnumerable<TEventHandler> GetEventHandlers<TEventHandler>(HandlersFactory factory) =>
            factory(typeof(TEventHandler)).Select(x => (TEventHandler)x);
    }

    public class EventInvoker<TEvent> : EventInvoker
        where TEvent : IEvent
    {
        public override void Invoke(IEvent @event, HandlersFactory factory)
        {
            var handlers = GetEventHandlers<IHandleEvent<TEvent>>(factory);
            if (handlers?.Any() ?? false)
                Parallel.ForEach(handlers, (handler) => handler.Handle((TEvent)@event));
        }
    }
}