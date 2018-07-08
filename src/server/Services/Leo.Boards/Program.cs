using Microsoft.ServiceFabric.Services.Runtime;

namespace Leo.Boards
{
    internal static class Program
    {
        private static void Main()
        {
            ServiceRuntime.RegisterServiceAsync("boards",
                context => new BoardsService(context)).GetAwaiter().GetResult();
        }
    }
}