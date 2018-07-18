using Phase.Domains;
using Phase.Interfaces;
using System;
using System.Collections.Generic;

namespace Phase
{
    public abstract class DependencyResolver
    {
        internal Session _session { get; set; }

        protected abstract object Single(Type interfaceType);
        protected T Single<T>() => (T)Single(typeof(T));

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

        internal IHandleQuery<TQuery, TResult> GetQueryHandler<TQuery, TResult>() where TQuery : IQuery<TResult> => 
            Single<IHandleQuery<TQuery, TResult>>();

        private void InitializeCommandHandler(object commandHandler)
        {
            if (commandHandler is CommandHandler)
            {
                ((CommandHandler)commandHandler)._session = _session;
            }
        }
    }
}