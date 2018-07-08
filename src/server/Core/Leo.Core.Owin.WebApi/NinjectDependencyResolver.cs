using Ninject;
using Ninject.Parameters;
using Ninject.Syntax;
using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;
using Ninject.Extensions.ChildKernel;

namespace Leo.Core.Owin.WebApi
{
    public class NinjectDependencyResolver : NinjectDependencyScope, IDependencyResolver, IDependencyScope, IDisposable
    {
        private readonly IKernel _kernel;

        public NinjectDependencyResolver(IKernel kernel)
          : base(kernel)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
        }

        public IDependencyScope BeginScope() => new NinjectDependencyScope(new ChildKernel(_kernel));
    }

    public class NinjectDependencyScope : IDependencyScope, IDisposable
    {
        private IResolutionRoot _resolver;

        public NinjectDependencyScope(IResolutionRoot resolver)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        public virtual void Dispose()
        {
            IDisposable resolver = _resolver as IDisposable;
            if (resolver != null)
                resolver.Dispose();
            _resolver = (IResolutionRoot)null;
        }

        public object GetService(Type serviceType)
        {
            if (_resolver == null)
                throw new ObjectDisposedException("this", "This scope has been disposed");

            try
            {
                return _resolver.Get(serviceType, new IParameter[0]);
            }
            catch (ActivationException e)
            {
                if (serviceType.FullName.StartsWith("System") ||
                    serviceType.FullName.StartsWith("Microsoft"))
                    return null;
                else                 
                    throw;
            }            
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            if (_resolver == null)
                throw new ObjectDisposedException("this", "This scope has been disposed");
            return _resolver.GetAll(serviceType, new IParameter[0]);
        }
    }
}