using System.Fabric;
using Microsoft.ServiceFabric.Services.Runtime;
using Phase;

namespace Leo.Boards
{
    internal sealed class BoardService : StatefulService
    {
        private readonly Phase.Phase _phase;

        public BoardService(StatefulServiceContext context, Phase.Phase phase)
            : base(context)
        {
            _phase = phase;
        }
    }
}
