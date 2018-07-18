using Phase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Phase.Publishers
{
    internal class EventPublisher : IDisposable
    {
        private List<EventSubscriber> _subscribers = new List<EventSubscriber>();

        internal void Subscribe<TEvent>(IHandleEvent<TEvent> handler)
            where TEvent : IEvent => _subscribers.Add(new EventSubscriber<TEvent>(handler));

        internal void Publish(IEnumerable<IEvent> events, CancellationToken cancellationToken)
        {
            foreach(var e in events.OrderBy(e => e.SequenceId))
            {
                if (!cancellationToken.IsCancellationRequested)
                    Publish(e, cancellationToken);
            }
        }

        internal virtual void Publish(IEvent @event, CancellationToken cancellationToken)
        {
            var subscriberType = typeof(EventSubscriber<>).MakeGenericType(@event.GetType());
            var subscribers = _subscribers.Where(s => subscriberType.IsAssignableFrom(s.GetType()));
            if(subscribers.Any())
                Parallel.ForEach(subscribers, (subscriber) => subscriber.Publish(@event));
        }

        public void Dispose() => _subscribers.Clear();
    }
}
