using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phase.Domains
{
    public interface IRepository
    {
        Task<T> Get<T>(string aggregateId) where T : AggregateRoot;
    }
}
