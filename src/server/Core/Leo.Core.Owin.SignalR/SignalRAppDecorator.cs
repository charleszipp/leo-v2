using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin.Cors;
using Newtonsoft.Json;
using Ninject;
using Owin;
using System.Linq;

namespace Leo.Core.Owin.SignalR
{
    public class SignalRAppDecorator : AppDecorator
    {
        private readonly IDependencyResolver _resolver;
        private readonly IKernel _kernel;
        private readonly IHubPipelineModule[] _modules;

        public SignalRAppDecorator(AppBuilder build, IDependencyResolver resolver, IKernel kernel, params IHubPipelineModule[] modules)
            : base(build)
        {
            _resolver = resolver;
            _kernel = kernel;
            _modules = modules;
        }

        public override void Configure(IAppBuilder app)
        {
            _build.Configure(app);

            _kernel.Bind<JsonSerializer>()
                .ToMethod(ctx =>
                    JsonSerializer.Create(
                        new JsonSerializerSettings
                        {
                            ContractResolver = new SignalRContractResolver(),
                            NullValueHandling = NullValueHandling.Ignore
                        }
                    )
                );

            app.Map("/signalr", map =>
            {
                map.UseCors(CorsOptions.AllowAll);
                HubConfiguration config = new HubConfiguration();
                config.EnableDetailedErrors = true;
                config.EnableJavaScriptProxies = true;
                config.EnableJSONP = true;
                config.Resolver = _resolver;

                if (_modules != null && _modules.Any())
                {
                    var hubPipeline = config.Resolver.Resolve<IHubPipeline>();
                    foreach (var module in _modules)
                    {
                        hubPipeline.AddModule(module);
                    }
                }

                map.RunSignalR(config);
            });            
        }
    }
}