using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Phase.ServiceFabric
{
    public static class ReliableStateManagerExtensions
    {
        public static Task<IReliableConcurrentQueue<IEnumerable<string>>> GetOrAddEventsQueueAsync(this IReliableStateManager states)
        {
            return states.GetOrAddAsync<IReliableConcurrentQueue<IEnumerable<string>>>(KnownStates.ReliableEventsQueue);
        }

        public static Task<IReliableDictionary<string, IEnumerable<string>>> GetOrAddEventsStateAsync(this IReliableStateManager states)
        {
            return states.GetOrAddAsync<IReliableDictionary<string, IEnumerable<string>>>(KnownStates.ReliableEvents);
        }

        public static async Task<bool> HasEventsStateAsync(this IReliableStateManager states)
        {
            var events = await states.TryGetAsync<IReliableDictionary<string, IEnumerable<string>>>(KnownStates.ReliableEvents).ConfigureAwait(false);
            return events.HasValue;
        }

        public static Task RemoveEventsQueueAsync(this IReliableStateManager states)
        {
            return states.RemoveAsync(KnownStates.ReliableEventsQueue);
        }

        public static Task RemoveEventsStateAsync(this IReliableStateManager states)
        {
            return states.RemoveAsync(KnownStates.ReliableEvents);
        }
    }
}