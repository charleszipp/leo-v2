using Phase.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Phase.Interfaces;
using System.Threading;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Phase.ServiceFabric
{
    public class ReliableEventsProvider : IEventsProvider
    {
        private readonly IEventsProvider _provider;
        private readonly IReliableStateManager _states;
        // this is used primarily for the overloads that accept cancellation tokens. these overloads require both timeout timespan and cancellation
        private TimeSpan _defaultTimeout = TimeSpan.FromSeconds(4);

        public ReliableEventsProvider(IReliableStateManager states, IEventsProvider provider)
        {
            _provider = provider;
            _states = states;
        }

        public async Task DeactivateAsync(CancellationToken cancellationToken)
        {
            await _provider.DeactivateAsync(cancellationToken).ConfigureAwait(false);
            await _states.RemoveEventsStateAsync().ConfigureAwait(false);
        }

        public async Task CommitAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken)
        {
            await _provider.CommitAsync(events, cancellationToken).ConfigureAwait(false);
            await OnUpdateEventsState(events, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<IEvent>> GetAsync(string aggregateId, CancellationToken cancellationToken, int fromVersion = -1)
        {
            var rvalues = new List<IEvent>();
            var eventsState = await _states.GetOrAddEventsStateAsync().ConfigureAwait(false);

            using (var tx = _states.CreateTransaction())
            {
                var eventsJson = await eventsState.TryGetValueAsync(tx, aggregateId, _defaultTimeout, cancellationToken).ConfigureAwait(false);

                if (eventsJson.HasValue)
                {
                    var events = eventsJson.Value.Select(e =>
                    {
                        JObject doc = JObject.Parse(e);
                        return (IEvent)JsonConvert.DeserializeObject(e, Type.GetType(doc["_evtype"].ToString()));
                    });
                    rvalues = events.Where(e => e.Version >= fromVersion).ToList();
                }
            }

            return rvalues;
        }

        public async Task<IEnumerable<IEvent>> GetEventsAsync(CancellationToken cancellationToken)
        {
            var rvalues = new List<IEvent>();
            var eventsState = await _states.GetOrAddEventsStateAsync().ConfigureAwait(false);

            using (var tx = _states.CreateTransaction())
            {
                var eventsJsonEnumerable = await eventsState.CreateEnumerableAsync(tx, EnumerationMode.Unordered).ConfigureAwait(false);
                using (var eventsJson = eventsJsonEnumerable.GetAsyncEnumerator())
                {
                    do
                    {
                        if (eventsJson.Current.Key != null)
                        {
                            var events = eventsJson.Current.Value.Select(e =>
                            {
                                JObject doc = JObject.Parse(e);
                                return (IEvent)JsonConvert.DeserializeObject(e, Type.GetType(doc["_evtype"].ToString()));
                            });
                            rvalues.AddRange(events);
                        }
                    }
                    while (await eventsJson.MoveNextAsync(cancellationToken));
                }
            }

            return rvalues;
        }

        public async Task ActivateAsync(string tenantInstanceName, CancellationToken cancellationToken)
        {
            if (!await _states.HasEventsStateAsync().ConfigureAwait(false))
            {
                await _provider.ActivateAsync(tenantInstanceName, cancellationToken).ConfigureAwait(false);
                var events = await _provider.GetEventsAsync(cancellationToken);
                await OnUpdateEventsState(events, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task OnUpdateEventsState(IEnumerable<IEvent> events, CancellationToken cancellationToken)
        {
            var eventsByEntity = events.GroupBy(e => e.AggregateId).ToList();

            var eventsState = await _states.GetOrAddEventsStateAsync().ConfigureAwait(false);
            using (var tx = _states.CreateTransaction())
            {
                var upserts = new List<Task<IEnumerable<string>>>();
                foreach (var entityEvents in eventsByEntity)
                {
                    var entityEventsAsJson = entityEvents.Select(e => JsonConvert.SerializeObject(e)).ToList();

                    var upsert = eventsState.AddOrUpdateAsync(tx, entityEvents.Key, entityEventsAsJson, (id, existingEntityEvents) =>
                    {
                        var newEvents = new List<string>(existingEntityEvents);
                        newEvents.AddRange(entityEventsAsJson);
                        return newEvents;
                    }, TimeSpan.FromSeconds(4), cancellationToken);
                    upserts.Add(upsert);
                }
                await Task.WhenAll(upserts).ConfigureAwait(false);
                await tx.CommitAsync().ConfigureAwait(false);
            }
        }
    }
}
