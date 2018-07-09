using Phase.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Phase.Mediators
{
    public class LockingMediator : IMediator
    {
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private readonly IMediator _mediator;

        public LockingMediator(IMediator mediator) => _mediator = mediator;

        public async Task<T> ExecuteAsync<T>(ICommand<T> command, CancellationToken cancellationToken)
        {
            try
            {
                await _lock.WaitAsync().ConfigureAwait(false);
                return await _mediator.ExecuteAsync(command, cancellationToken).ConfigureAwait(false);
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
                await _lock.WaitAsync().ConfigureAwait(false);
                await _mediator.ExecuteAsync(command, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task<TResult> Query<TResult>(IQuery<TResult> query, CancellationToken token)
        {
            try
            {
                await _lock.WaitAsync().ConfigureAwait(false);
                return await _mediator.Query(query, token);
            }
            finally
            {
                _lock.Release();
            }
        }
    }
}