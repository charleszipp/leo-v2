using Phase.Mediators;
using System.Threading;
using System.Threading.Tasks;

namespace Phase
{
    public interface IPhase : IMediator
    {
        Task ActivateAsync(CancellationToken cancellationToken);

        Task DeactivateAsync(CancellationToken cancellationToken);
    }
}