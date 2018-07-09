using Phase.Providers;
using System.Threading.Tasks;

namespace Phase.Domains
{
    public class Repository : IRepository
    {
        private readonly IEventsProvider _eventsProvider;
        private readonly IEventsPublisher _publisher;
        private readonly DependencyResolver _resolver;

        public Repository(IEventsProvider eventsProvider, IEventsPublisher publisher, DependencyResolver resolver)
        {
            _eventsProvider = eventsProvider;
            _resolver = resolver;
            _publisher = publisher;
        }

        public async Task<T> Get<T>(string aggregateId)
            where T : AggregateRoot
        {
            var events = await _eventsProvider.GetAsync(aggregateId).ConfigureAwait(false);
            var rvalue = AggregateProxy<T>.Create((T)_resolver(typeof(T)));
            rvalue.Load(_publisher, events, aggregateId);
            return rvalue;
        }
    }
}