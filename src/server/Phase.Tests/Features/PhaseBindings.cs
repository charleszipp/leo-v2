using Microsoft.VisualStudio.TestTools.UnitTesting;
using Phase.Builders;
using Phase.Ninject;
using Phase.Providers.Memory;
using Phase.Tests.Commands;
using Phase.Tests.Events;
using Phase.Tests.Models;
using Phase.Tests.Queries;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Bindings;

namespace Phase.Tests.Features
{
    [Binding]
    public class PhaseBindings
    {
        private readonly CancellationTokenSource _cancellation;
        private readonly Phase _phase;
        private Exception _exception;

        public PhaseBindings()
        {
            var dependencyResolver = new NinjectDependencyResolver();
            var eventsProvider = new InMemoryEventsProvider(new InMemoryEventCollection(), TenantKeyFactory);

            _phase = new PhaseBuilder(dependencyResolver, eventsProvider, TenantKeyFactory)
                .WithCommandHandler<CreateMockHandler, CreateMock>()
                .WithQueryHandler<GetMockHandler, GetMock, GetMockResult>()
                .WithAggregateRoot<MockAggregate>()
                .WithReadModel<MockReadModel>()
                .WithStatefulEventSubscriber<MockCreated, MockReadModel>()
                .Build();
            _cancellation = new CancellationTokenSource();
        }

        [Given(@"the phase client is vacant")]
        public async Task GivenThePhaseClientIsVacant()
        {
            if (_phase.IsOccupied)
                await _phase.VacateAsync(_cancellation.Token);
        }

        [Then(@"an exception should be thrown with message ""(.*)""")]
        public void ThenAnExceptionShouldBeThrownWithMessage(string exceptionMessage)
        {
            Assert.AreEqual(exceptionMessage, ScenarioContext.Current.TestError?.Message);
        }

        [When(@"executing a command without result")]
        public Task WhenExecutingACommandWithoutResult() => 
            _phase.ExecuteAsync(new CreateMock(Guid.NewGuid(), "Mock 1"), _cancellation.Token);

        [When(@"executing a command with result")]
        public void WhenExecutingACommandWithResult()
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"executing a query")]
        public Task WhenExecutingAQuery() => 
            _phase.QueryAsync(new GetMock(), _cancellation.Token);

        [When(@"executing vacate")]
        public Task WhenExecutingVacate() => 
            _phase.VacateAsync(_cancellation.Token);

        private IDictionary<string, string> TenantKeyFactory(string tenantInstanceName) => 
            new Dictionary<string, string> { { "boardid", tenantInstanceName } };

        [AfterStep("CatchException")]
        public void CatchException()
        {
            if (ScenarioContext.Current.StepContext.StepInfo.StepDefinitionType == StepDefinitionType.When)
            {
                PropertyInfo testStatusProperty = typeof(ScenarioContext).GetProperty(nameof(ScenarioContext.Current.ScenarioExecutionStatus), BindingFlags.Public | BindingFlags.Instance);
                testStatusProperty.SetValue(ScenarioContext.Current, ScenarioExecutionStatus.OK);
            }
        }
    }
}