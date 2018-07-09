using Phase.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Phase.Providers.Memory
{
    public class InMemoryEventsProvider : IEventsProvider
    {
        protected readonly InMemoryEventCollection Db;
        protected readonly InMemoryEventsProviderConfiguration ProviderConfiguration;

        public InMemoryEventsProvider(InMemoryEventCollection db, InMemoryEventsProviderConfiguration providerConfiguration)
        {
            Db = db;
            ProviderConfiguration = providerConfiguration;
        }

        public virtual Task AbortAsync() =>
            Task.CompletedTask;

        public virtual Task CommitAsync(IEnumerable<IEvent> events)
        {
            Db.AddOrUpdateEvents(events, ProviderConfiguration);
            return Task.CompletedTask;
        }

        public virtual Task<IEnumerable<IEvent>> GetAsync(string aggregateId, int fromVersion = -1) =>
            Task.FromResult(Db.Get(aggregateId, fromVersion));

        public Task<IEnumerable<IEvent>> GetEventsAsync(CancellationToken cancellationToken) =>
            Task.FromResult(Db.Get(ProviderConfiguration));

        public Task InitializeAsync(CancellationToken cancellationToken) =>
            Task.CompletedTask;
    }
}