using Phase.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Phase.Mediators
{
    public abstract class QueryInvoker<TResult>
    {
        public abstract Task<TResult> InvokeAsync(IQuery<TResult> query, DependencyResolver resolver, CancellationToken cancellationToken);

        protected TQuery GetQueryHandler<TQuery>(DependencyResolver resolver)
        {
            return (TQuery)resolver(typeof(TQuery));
        }
    }

    public class QueryInvoker<TQuery, TResult> : QueryInvoker<TResult>
        where TQuery : IQuery<TResult>
    {
        public override Task<TResult> InvokeAsync(IQuery<TResult> query, DependencyResolver resolver, CancellationToken cancellationToken)
        {
            return GetQueryHandler<IHandleQuery<TQuery, TResult>>(resolver)
                .Execute((TQuery)query, cancellationToken);
        }
    }
}