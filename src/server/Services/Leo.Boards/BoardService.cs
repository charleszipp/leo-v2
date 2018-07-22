using System.Fabric;
using Microsoft.ServiceFabric.Services.Runtime;
using Phase;

namespace Leo.Boards
{
    internal sealed class BoardService : StatefulService
    {
        private readonly Phase _phase;

        public BoardService(StatefulServiceContext context, Phase phase)
            : base(context)
        {
            _phase = phase;
        }
    }
}
