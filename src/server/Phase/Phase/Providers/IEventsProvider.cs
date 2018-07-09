using Phase.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Phase.Providers
{
    public interface IEventsProvider
    {
        Task AbortAsync();

        Task CommitAsync(IEnumerable<IEvent> events);

        Task<IEnumerable<IEvent>> GetAsync(string aggregateId, int fromVersion = -1);

        Task<IEnumerable<IEvent>> GetEventsAsync(CancellationToken cancellationToken);

        Task InitializeAsync(CancellationToken cancellationToken);
    }
}