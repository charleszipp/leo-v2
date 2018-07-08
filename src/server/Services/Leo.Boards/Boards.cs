using System.Fabric;
using Microsoft.ServiceFabric.Services.Runtime;

namespace Leo.Boards
{
    internal sealed class BoardsService : StatefulService
    {
        public BoardsService(StatefulServiceContext context)
            : base(context)
        { }
    }
}
