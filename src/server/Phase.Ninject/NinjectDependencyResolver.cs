using Ninject;
using System;

namespace Phase.Ninject
{
    public class NinjectDependencyResolver : DependencyResolver
    {
        private readonly IKernel _kernel;

        public NinjectDependencyResolver(IKernel kernel) => _kernel = kernel;

        protected override object Single(Type interfaceType) => _kernel.Get(interfaceType);
    }
}