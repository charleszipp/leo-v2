﻿using Phase.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Phase.Mediators
{
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