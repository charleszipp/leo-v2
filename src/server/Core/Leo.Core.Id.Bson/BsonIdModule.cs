using Leo.Core.Ninject;
using Ninject;
using Ninject.Modules;

namespace Leo.Core.Id.Bson
{
    public static class KernelExtensions
    {
        public static IKernel WithBsonIds(this IKernel kernel)
        {
            return kernel.WithModule<BsonIdModule>();
        }
    }

    public class BsonIdModule : NinjectModule
    {
        public override void Load()
        {
            Rebind<IdProvider>().To<ObjectIdProvider>();
        }
    }
}