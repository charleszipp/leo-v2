using Phase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Phase.Mediators
{
    internal sealed class Mediator
    {
        private readonly DependencyResolver _resolver;

        internal Mediator(DependencyResolver resolver) => _resolver = resolver;

        public Task<TResult> Query<TResult>(ITenantContext tenant, IQuery<TResult> query, CancellationToken cancellationToken)
        {
            var invoker = (QueryInvoker<TResult>)Activator.CreateInstance(typeof(QueryInvoker<,>).MakeGenericType(query.GetType(), typeof(TResult)));
            return invoker.InvokeAsync(query, _resolver, cancellationToken);
        }

        public Task ExecuteAsync(ITenantContext tenant, ICommand command, CancellationToken cancellationToken)
        {
            var invoker = (VoidCommandInvoker)Activator.CreateInstance(typeof(VoidCommandInvoker<>).MakeGenericType(command.GetType()));
            return invoker.InvokeAsync(tenant, command, _resolver, cancellationToken);
        }
    }
}
