using Leo.Core.Api;
using Leo.Core.Api.Interfaces;
using Newtonsoft.Json.Serialization;
using Ninject;
using Owin;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Filters;

namespace Leo.Core.Owin.WebApi
{
    public class WebApiAppDecorator : AppDecorator
    {
        private readonly IActionFilter[] _filters;
        private readonly IKernel _kernel;

        public WebApiAppDecorator(AppBuilder build, IKernel kernel, params IActionFilter[] filters)
            : base(build)
        {
            _kernel = kernel;
            _filters = filters;
        }

        public override void Configure(IAppBuilder app)
        {
            _build.Configure(app);

            HttpConfiguration config = new HttpConfiguration();

            //use route attributes by default
            config.MapHttpAttributeRoutes();

            //default json serialization settings
            var formatters = config.Formatters;
            var jsonFormatter = formatters.JsonFormatter;
            var settings = jsonFormatter.SerializerSettings;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            settings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            config.DependencyResolver = new NinjectDependencyResolver(_kernel);

            config.ParameterBindingRules.Insert(0, descriptor => typeof(IRequest).IsAssignableFrom(descriptor.ParameterType) && descriptor.ParameterType.IsClass ? new MultiSourceParameterBinding(descriptor) : null);

            if (_filters != null && _filters.Any())
                config.Filters.AddRange(_filters);

            app.UseWebApi(config);
        }
    }
}