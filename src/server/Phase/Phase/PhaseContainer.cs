using Phase.Mediators;
using Phase.Domains;
using Phase.Providers;

namespace Phase
{
    public class PhaseContainer
    {
        public ISession Session { get; set; }
        public IMediator Mediator { get; set; }
        public IEventsProvider EventsProvider { get; set; }
        public IEventsPublisher Publisher { get; set; }
    }
}