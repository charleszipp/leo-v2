using Phase.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Phase.Mediators
{
    public interface IMediator
    {
        Task<T> ExecuteAsync<T>(ICommand<T> command, CancellationToken cancellationToken);

        Task ExecuteAsync(ICommand command, CancellationToken cancellationToken);

        Task<TResult> Query<TResult>(IQuery<TResult> query, CancellationToken cancellationToken);
    }
}