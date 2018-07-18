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

        public Task<TResult> Query<TResult>(IQuery<TResult> query, CancellationToken cancellationToken)
        {
            var invoker = (QueryInvoker<TResult>)Activator.CreateInstance(typeof(QueryInvoker<,>).MakeGenericType(query.GetType(), typeof(TResult)));
            return invoker.InvokeAsync(query, _resolver, cancellationToken);
        }

        public Task<T> ExecuteAsync<T>(ICommand<T> command, CancellationToken cancellationToken)
        {
            var invoker = (CommandInvoker<T>)Activator.CreateInstance(typeof(CommandInvoker<,>).MakeGenericType(command.GetType(), typeof(T)));
            return invoker.InvokeAsync(command, _resolver, cancellationToken);
        }

        public Task ExecuteAsync(ICommand command, CancellationToken cancellationToken)
        {
            var invoker = (VoidCommandInvoker)Activator.CreateInstance(typeof(VoidCommandInvoker<>).MakeGenericType(command.GetType()));
            return invoker.InvokeAsync(command, _resolver, cancellationToken);
        }
    }
}
