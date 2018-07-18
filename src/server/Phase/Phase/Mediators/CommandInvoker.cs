using Phase.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Phase.Mediators
{
    internal abstract class CommandInvoker<T>
    {
        internal abstract Task<T> InvokeAsync(ICommand<T> command, DependencyResolver resolver, CancellationToken cancellationToken);
    }

    internal class CommandInvoker<TCommand, TReturn> : CommandInvoker<TReturn>
        where TCommand : ICommand<TReturn>
    {
        internal override Task<TReturn> InvokeAsync(ICommand<TReturn> command, DependencyResolver resolver, CancellationToken cancellationToken) 
            => resolver.GetCommandHandler<TCommand, TReturn>().Execute((TCommand)command, cancellationToken);
    }

    internal abstract class VoidCommandInvoker
    {
        internal abstract Task InvokeAsync(ICommand command, DependencyResolver resolver, CancellationToken cancellationToken);
    }

    internal class VoidCommandInvoker<TCommand> : VoidCommandInvoker
        where TCommand : ICommand
    {
        internal override Task InvokeAsync(ICommand command, DependencyResolver resolver, CancellationToken cancellationToken) => 
            resolver.GetCommandHandler<TCommand>().Execute((TCommand)command, cancellationToken);
    }
}