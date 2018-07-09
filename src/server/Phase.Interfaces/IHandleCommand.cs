using System.Threading;
using System.Threading.Tasks;

namespace Phase.Interfaces
{
    public interface IHandleCommand<in TCommand, TReturn>
        where TCommand : ICommand<TReturn>
    {
        Task<TReturn> Execute(TCommand command, CancellationToken cancellationToken);
    }

    public interface IHandleCommand<in TCommand>
        where TCommand : ICommand
    {
        Task Execute(TCommand command, CancellationToken cancellationToken);
    }
}