using Microsoft.AspNet.SignalR;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Leo.Core.Owin.SignalR
{
    public class NinjectDependencyResolver : DefaultDependencyResolver
    {
        private readonly IKernel _kernel;

        public NinjectDependencyResolver(IKernel kernel)
        {
            _kernel = kernel;
        }

        public override object GetService(Type serviceType)
        {
            return _kernel.TryGet(serviceType) ?? base.GetService(serviceType);
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            return _kernel.GetAll(serviceType).Concat(base.GetServices(serviceType));
        }
    }
}