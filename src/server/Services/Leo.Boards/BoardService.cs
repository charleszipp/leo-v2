using System.Fabric;
using Microsoft.ServiceFabric.Services.Runtime;
using Phase;

namespace Leo.Boards
{
    internal sealed class BoardService : StatefulService
    {
        private readonly IPhase _phase;

        public BoardService(StatefulServiceContext context, IPhase phase)
            : base(context)
        {
            _phase = phase;
        }
    }
}
