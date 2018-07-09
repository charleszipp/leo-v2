﻿using Phase.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Phase.Providers
{
    public interface IEventsProvider
    {
        Task AbortAsync(CancellationToken cancellationToken);

        Task CommitAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken);

        Task<IEnumerable<IEvent>> GetAsync(string aggregateId, CancellationToken cancellationToken, int fromVersion = -1);

        Task<IEnumerable<IEvent>> GetEventsAsync(CancellationToken cancellationToken);

        Task InitializeAsync(CancellationToken cancellationToken);
    }
}