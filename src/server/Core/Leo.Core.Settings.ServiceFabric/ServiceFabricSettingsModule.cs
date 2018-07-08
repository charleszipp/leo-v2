using Leo.Core.Ninject;
using Ninject;
using Ninject.Modules;
using System.Fabric;

namespace Leo.Core.Settings.ServiceFabric
{
    public static class KernelExtensions
    {
        public static IKernel WithServiceFabricSettings(this IKernel kernel, ICodePackageActivationContext context)
        {
            return kernel.WithModule(new ServiceFabricSettingsModule(context));
        }
    }

    public class ServiceFabricSettingsModule : NinjectModule
    {
        protected readonly ICodePackageActivationContext _context;

        public ServiceFabricSettingsModule(ICodePackageActivationContext context)
        {
            _context = context;
        }

        public override void Load()
        {
            Bind<ICodePackageActivationContext>().ToConstant(_context);
            Bind<ISettingsProvider>().To<ServiceFabricSettingsProvider>();
        }
    }
}