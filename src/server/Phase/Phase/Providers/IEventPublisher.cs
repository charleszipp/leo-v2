using Phase.Interfaces;
using System.Collections.Generic;

namespace Phase.Providers
{
    public interface IEventsPublisher
    {
        void Publish(IEnumerable<IEvent> events);

        void Publish(IEvent @event);
    }
}