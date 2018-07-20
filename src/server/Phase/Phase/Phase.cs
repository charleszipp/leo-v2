using System.Threading;
using System.Threading.Tasks;
using Phase.Interfaces;
using System;
using Phase.Providers;
using Phase.Mediators;
using Phase.Domains;
using Phase.Publishers;
using System.Collections.Generic;

namespace Phase
{
    public sealed class Phase
    {
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private readonly IEventsProvider _eventsProvider;
        private readonly Mediator _mediator;
        private readonly Session _session;
        private readonly EventPublisher _publisher;
        public DependencyResolver DependencyResolver { get; }

        internal Phase(DependencyResolver resolver, IEventsProvider eventsProvider, Func<string, IDictionary<string, string>> tenantKeysFactory)
        {
            DependencyResolver = resolver;
            _eventsProvider = eventsProvider;
            _mediator = new Mediator(resolver);
            _session = new Session(resolver, _eventsProvider);
            resolver._session = _session;
            _publisher = new EventPublisher(resolver);
        }

        public async Task ActivateAsync(string tenantInstanceName, CancellationToken cancellationToken)
        {
            try
            {
                await _lock.WaitAsync(cancellationToken).ConfigureAwait(false);
                await _eventsProvider.ActivateAsync(tenantInstanceName, cancellationToken).ConfigureAwait(false);
                var events = await _eventsProvider.GetEventsAsync(cancellationToken).ConfigureAwait(false);
                _publisher.Publish(events, cancellationToken);
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task DeactivateAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _lock.WaitAsync(cancellationToken).ConfigureAwait(false);
                await _eventsProvider.DeactivateAsync(cancellationToken);
                DependencyResolver.ReleaseVolatileStates();
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task<T> ExecuteAsync<T>(ICommand<T> command, CancellationToken cancellationToken)
        {
            try
            {
                await _lock.WaitAsync(cancellationToken).ConfigureAwait(false);
                var rvalue = await _mediator.ExecuteAsync(command, cancellationToken).ConfigureAwait(false);
                // before commit make sure we havent been asked to shut down
                cancellationToken.ThrowIfCancellationRequested();
                var events = _session.Flush();
                await _eventsProvider.CommitAsync(events, cancellationToken).ConfigureAwait(false);
                _publisher.Publish(events, cancellationToken);

                return rvalue;
            }
            catch (Exception)
            {
                // rollback any new events produced from the command
                _session.Flush();
                throw;
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task ExecuteAsync(ICommand command, CancellationToken cancellationToken)
        {
            try
            {
                await _lock.WaitAsync(cancellationToken).ConfigureAwait(false);
                await _mediator.ExecuteAsync(command, cancellationToken).ConfigureAwait(false);
                // before commit make sure we havent been asked to shut down
                cancellationToken.ThrowIfCancellationRequested();
                var events = _session.Flush();
                await _eventsProvider.CommitAsync(events, cancellationToken).ConfigureAwait(false);
                _publisher.Publish(events, cancellationToken);
            }
            catch(Exception)
            {
                // rollback any new events produced from the command
                _session.Flush();
                throw;
            }
            finally
            {
                _lock.Release();
            }
        }

        public Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken) => 
            _mediator.Query(query, cancellationToken);
    }
}