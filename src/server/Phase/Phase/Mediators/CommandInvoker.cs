using Phase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Phase.Mediators
{
    public abstract class VoidCommandInvoker
    {
        public abstract Task InvokeAsync(ICommand command, DependencyResolver resolver, CancellationToken cancellationToken);

        protected TCommandHandler GetCommandHandler<TCommandHandler>(DependencyResolver resolver)
        {
            return (TCommandHandler)resolver(typeof(TCommandHandler));
        }
    }

    public class VoidCommandInvoker<TCommand> : VoidCommandInvoker
        where TCommand : ICommand
    {
        public override Task InvokeAsync(ICommand command, DependencyResolver resolver, CancellationToken cancellationToken)
        {
            return GetCommandHandler<IHandleCommand<TCommand>>(resolver)
                .Execute((TCommand)command, cancellationToken);
        }
    }

    public abstract class CommandInvoker<T>
    {
        public abstract Task<T> InvokeAsync(ICommand<T> command, DependencyResolver resolver, CancellationToken cancellationToken);

        protected TCommandHandler GetCommandHandler<TCommandHandler>(DependencyResolver resolver)
        {
            return (TCommandHandler)resolver(typeof(TCommandHandler));
        }
    }

    public class CommandInvoker<TCommand, TReturn> : CommandInvoker<TReturn>
        where TCommand : ICommand<TReturn>
    {
        public override Task<TReturn> InvokeAsync(ICommand<TReturn> command, DependencyResolver resolver, CancellationToken cancellationToken)
        {
            return GetCommandHandler<IHandleCommand<TCommand, TReturn>>(resolver)
                .Execute((TCommand)command, cancellationToken);
        }
    }
}
