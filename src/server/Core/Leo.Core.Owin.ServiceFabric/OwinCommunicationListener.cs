using Microsoft.Owin.Hosting;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Owin;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Leo.Core.Owin.ServiceFabric
{
    public class OwinCommunicationListener : ICommunicationListener
    {
        private readonly Action<IAppBuilder> startup;
        private readonly ServiceContext serviceContext;
        private readonly string endpointName;
        private readonly string appRoot;

        private IDisposable webApp;
        private string publishAddress;
        private string listeningAddress;

        public OwinCommunicationListener(Action<IAppBuilder> startup, ServiceContext serviceContext, string endpointName = null, string appRoot = null)
        {
            if (startup == null)
            {
                throw new ArgumentNullException(nameof(startup));
            }

            if (serviceContext == null)
            {
                throw new ArgumentNullException(nameof(serviceContext));
            }

            this.startup = startup;
            this.serviceContext = serviceContext;
            this.endpointName = endpointName;
            this.appRoot = appRoot;
        }

        public bool ListenOnSecondary { get; set; }

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            var serviceEndpoint = 
                string.IsNullOrEmpty(this.endpointName) ? 
                    this.serviceContext.CodePackageActivationContext.GetEndpoints().SingleOrDefault(e => 
                        e.Protocol == System.Fabric.Description.EndpointProtocol.Http || e.Protocol == System.Fabric.Description.EndpointProtocol.Https) : 
                    this.serviceContext.CodePackageActivationContext.GetEndpoint(this.endpointName);
            
            int port = serviceEndpoint.Port;
            string suffix = (this.appRoot ?? serviceEndpoint.PathSuffix ?? string.Empty).Trim('/');

            if (this.serviceContext is StatefulServiceContext)
            {
                StatefulServiceContext statefulServiceContext = this.serviceContext as StatefulServiceContext;

                this.listeningAddress = string.Format(
                    CultureInfo.InvariantCulture,
                    "http://+:{0}/{1}{2}/{3}/{4}",
                    port,
                    suffix,
                    statefulServiceContext.PartitionId,
                    statefulServiceContext.ReplicaId,
                    Guid.NewGuid());
            }
            else if (this.serviceContext is StatelessServiceContext)
            {
                this.listeningAddress = string.Format(
                    CultureInfo.InvariantCulture,
                    "http://+:{0}/{1}",
                    port,
                    string.IsNullOrEmpty(suffix) ? string.Empty : suffix + '/'
                    );
            }
            else
            {
                throw new InvalidOperationException();
            }

            this.publishAddress = this.listeningAddress.Replace("+", FabricRuntime.GetNodeContext().IPAddressOrFQDN);

            try
            {

                this.webApp = WebApp.Start(this.listeningAddress, appBuilder => this.startup.Invoke(appBuilder));

                return Task.FromResult(this.publishAddress);
            }
            catch (Exception ex)
            {
                this.StopWebServer();

                throw;
            }
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            this.StopWebServer();

            return Task.FromResult(true);
        }

        public void Abort()
        {
            this.StopWebServer();
        }

        private void StopWebServer()
        {
            if (this.webApp != null)
            {
                try
                {
                    this.webApp.Dispose();
                }
                catch (ObjectDisposedException)
                {
                    // no-op
                }
            }
        }
    }
}
