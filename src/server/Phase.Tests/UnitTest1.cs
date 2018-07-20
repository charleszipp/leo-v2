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
using Phase.Tests.Queries;
using Phase.Builders;
using Phase.Tests.Models;
using Phase.Tests.Events;

namespace Phase.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private readonly Phase _phase;
        private CancellationTokenSource _cancellationTokenSource;

        public UnitTest1()
        {
            var dependencyResolver = new NinjectDependencyResolver();
            var eventsProvider = new InMemoryEventsProvider(new InMemoryEventCollection(), TenantKeyFactory);
            var builder = new PhaseBuilder(dependencyResolver, eventsProvider, TenantKeyFactory)
                .WithCommandHandler<CreateMockHandler, CreateMock>()
                .WithQueryHandler<GetMockHandler, GetMock, GetMockResult>()
                .WithAggregateRoot<MockAggregate>()
                .WithReadModel<MockReadModel>()
                .WithStatefulEventSubscriber<MockCreated, MockReadModel>();

            _phase = builder.Build();
        }

        private IDictionary<string, string> TenantKeyFactory(string tenantInstanceName) 
            => new Dictionary<string, string> { { "boardid", tenantInstanceName } };

        [TestMethod]
        public async Task TestMethod1()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var mockId = Guid.NewGuid();
            await _phase.OccupyAsync(mockId.ToString(), _cancellationTokenSource.Token);

            var command = new CreateMock(mockId, "Mock 1");
            await _phase.ExecuteAsync(command, _cancellationTokenSource.Token);

            var query = new GetMock();
            var result = await _phase.QueryAsync(query, _cancellationTokenSource.Token);
            
            Assert.AreEqual(command.MockName, result.MockName);

            await _phase.VacateAsync(_cancellationTokenSource.Token);

            var resultAfterDeactivate = await _phase.QueryAsync(query, _cancellationTokenSource.Token);
            Assert.IsTrue(string.IsNullOrEmpty(resultAfterDeactivate.MockName));
        }
    }
}
