using System.Threading;
using System.Threading.Tasks;
using Phase.Interfaces;
using System;

namespace Phase
{
    public class Phase : IPhase
    {
        private readonly PhaseContainer _container;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        public Phase(PhaseContainer container)
        {
            _container = container;
        }

        public async Task ActivateAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _lock.WaitAsync(cancellationToken).ConfigureAwait(false);
                await _container.EventsProvider.InitializeAsync(cancellationToken).ConfigureAwait(false);
                var events = await _container.EventsProvider.GetEventsAsync(cancellationToken).ConfigureAwait(false);
                _container.Publisher.Publish(events);
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
                await _container.EventsProvider.AbortAsync(cancellationToken);
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
                var rvalue = await _container.Mediator.ExecuteAsync(command, cancellationToken).ConfigureAwait(false);
                // before commit make sure we havent been asked to shut down
                cancellationToken.ThrowIfCancellationRequested();
                var events = _container.Session.Flush();
                await _container.EventsProvider.CommitAsync(events, cancellationToken).ConfigureAwait(false);
                _container.Publisher.Publish(events);

                return rvalue;
            }
            catch (Exception)
            {
                // rollback any new events produced from the command
                _container.Session.Flush();
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
                await _container.Mediator.ExecuteAsync(command, cancellationToken).ConfigureAwait(false);
                // before commit make sure we havent been asked to shut down
                cancellationToken.ThrowIfCancellationRequested();
                var events = _container.Session.Flush();
                await _container.EventsProvider.CommitAsync(events, cancellationToken).ConfigureAwait(false);
                _container.Publisher.Publish(events);
            }
            catch(Exception)
            {
                // rollback any new events produced from the command
                _container.Session.Flush();
                throw;
            }
            finally
            {
                _lock.Release();
            }
        }

        public Task<TResult> Query<TResult>(IQuery<TResult> query, CancellationToken cancellationToken) => 
            _container.Mediator.Query(query, cancellationToken);
    }
}