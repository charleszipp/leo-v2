using Phase.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Phase.Domains
{
    public interface ISession
    {
        Task AddAsync<T>(T aggregate) where T : AggregateRoot;

        IEnumerable<IEvent> Flush();

        Task<T> GetAsync<T>(Guid id) where T : AggregateRoot;
    }
}