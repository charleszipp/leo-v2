using Phase.Domains;
using Phase.Interfaces;
using System;
using System.Collections.Generic;

namespace Phase
{
    public abstract class DependencyResolver
    {
        internal Session _session { get; set; }

        protected abstract void RegisterTransient<TInterface, TImplementation>()
            where TImplementation : class, TInterface;

        protected abstract void RegisterSingleton<TInterface, TImplementation>() 
            where TImplementation : class, TInterface;

        protected abstract void CopySingleton<TOriginal, TDestination>()
            where TOriginal : TDestination;

        protected abstract T Single<T>();

        protected abstract void ReleaseAll<T>();

        internal TAggregate GetAggregateRoot<TAggregate>() where TAggregate : AggregateRoot => 
            AggregateProxy<TAggregate>.Create(Single<TAggregate>());

        internal IHandleCommand<TCommand> GetCommandHandler<TCommand>() where TCommand : ICommand
        {
            var rvalue = Single<IHandleCommand<TCommand>>();
            InitializeCommandHandler(rvalue);
            return rvalue;
        }

        internal IHandleCommand<TCommand, TResult> GetCommandHandler<TCommand, TResult>() where TCommand : ICommand<TResult>
        {
            var rvalue = Single<IHandleCommand<TCommand, TResult>>();
            InitializeCommandHandler(rvalue);
            return rvalue;
        }

        internal IHandleQuery<TQuery, TResult> GetQueryHandler<TQuery, TResult>() 
            where TQuery : IQuery<TResult> => 
            Single<IHandleQuery<TQuery, TResult>>();

        internal void RegisterCommandHandler<TCommandHandler, TCommand>()
            where TCommandHandler : class, IHandleCommand<TCommand>
            where TCommand : ICommand => 
            RegisterTransient<IHandleCommand<TCommand>, TCommandHandler>();

        internal void RegisterCommandHandler<TCommandHandler, TCommand, TResult>()
            where TCommandHandler : class, IHandleCommand<TCommand, TResult>
            where TCommand : ICommand<TResult> =>
            RegisterTransient<IHandleCommand<TCommand, TResult>, TCommandHandler>();

        internal void RegisterQueryHandler<TQueryHandler, TQuery, TResult>()
            where TQueryHandler : class, IHandleQuery<TQuery, TResult>
            where TQuery : IQuery<TResult> =>
            RegisterTransient<IHandleQuery<TQuery, TResult>, TQueryHandler>();

        internal void RegisterVolatileState<T>()
            where T : class, IVolatileState
        {
            RegisterSingleton<T, T>();
            CopySingleton<T, IVolatileState>();
        }

        internal void ReleaseVolatileStates() => 
            ReleaseAll<IVolatileState>();

        private void InitializeCommandHandler(object commandHandler)
        {
            if (commandHandler is CommandHandler)
            {
                ((CommandHandler)commandHandler)._session = _session;
            }
        }
    }
}