using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Ninject;
using Phase.Ninject;
using System.Threading;
using System.Threading.Tasks;
using Phase.Tests.Commands;
using Phase.Interfaces;
using Phase.Providers.Memory;

namespace Phase.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private readonly Phase _phase;
        private CancellationTokenSource _cancellationTokenSource;

        public UnitTest1()
        {
            var kernel = new StandardKernel();
            kernel.Bind<IHandleCommand<CreateMock>>().To<CreateMockHandler>();
            var dependencyResolver = new NinjectDependencyResolver(kernel);
            var eventsProvider = new InMemoryEventsProvider(new InMemoryEventCollection(), TenantKeyFactory);
            _phase = new Phase(dependencyResolver, eventsProvider, TenantKeyFactory);
            
        }

        private IDictionary<string, string> TenantKeyFactory(string tenantInstanceName) 
            => new Dictionary<string, string> { { "boardid", tenantInstanceName } };

        [TestMethod]
        public async Task TestMethod1()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            await _phase.ActivateAsync(Guid.NewGuid().ToString(), _cancellationTokenSource.Token);

            var command = new CreateMock(Guid.NewGuid(), "Mock 1");
            await _phase.ExecuteAsync(command, _cancellationTokenSource.Token);


            await _phase.DeactivateAsync(_cancellationTokenSource.Token);
        }
    }
}
