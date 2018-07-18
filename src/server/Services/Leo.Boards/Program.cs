using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Runtime;
using Ninject;
using Phase;
using Phase.Ninject;
using Phase.ServiceFabric;
using System.Collections.Generic;
using System.Fabric;

namespace Leo.Boards
{
    internal static class Program
    {
        private static void Main() => 
            ServiceRuntime.RegisterServiceAsync("boards", ServiceFactory)
            .GetAwaiter()
            .GetResult();

        private static BoardService ServiceFactory(StatefulServiceContext context)
        {
            var kernel = new StandardKernel();
            var stateManager = new ReliableStateManager(context);
            var phaseContainer = new PhaseDefaultBuilder(
                    new NinjectDependencyResolver(kernel),
                    (tenantInstanceName) => new Dictionary<string, string> { { "boardid", tenantInstanceName } }
                )
                .WithServiceFabric(stateManager)
                .Build();
            
            kernel.Bind<StatefulServiceContext>().ToConstant(context).InSingletonScope();
            kernel.Bind<ServiceContext>().ToConstant(context).InSingletonScope();
            kernel.Bind<IReliableStateManager>().ToConstant(stateManager).InSingletonScope();
            kernel.Bind<IReliableStateManagerReplica>().ToConstant(stateManager).InSingletonScope();
            kernel.Bind<IReliableStateManagerReplica2>().ToConstant(stateManager).InSingletonScope();
            kernel.Bind<PhaseContainer>().ToConstant(phaseContainer).InSingletonScope();
            kernel.Bind<IPhase>().ToConstant(phaseContainer.Phase).InSingletonScope();

            return new BoardService(context, phaseContainer.Phase);
        }
    }
}