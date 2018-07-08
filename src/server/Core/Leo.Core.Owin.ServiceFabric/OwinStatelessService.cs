using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Collections.Generic;
using System.Fabric;

namespace Leo.Core.Owin.ServiceFabric
{
    public sealed class OwinStatelessService : StatelessService
    {
        private readonly Leo.Core.Owin.AppBuilder _appBuilder;
        private readonly string _appRoot;
        private readonly string _endpointName;

        public OwinStatelessService(StatelessServiceContext context, Leo.Core.Owin.AppBuilder appBuilder, string endpointName = null, string appRoot = null)
            : base(context)
        {
            _appBuilder = appBuilder;
            _endpointName = endpointName;
            _appRoot = appRoot;
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[]
            {
                new ServiceInstanceListener(serviceContext =>
                    new OwinCommunicationListener(_appBuilder.Configure, Context, _endpointName, _appRoot)
                )
            };
        }
    }
}