using Ninject;
using Ninject.Modules;
using System;

namespace Leo.Core.Ninject
{
    public static class KernelExtensions
    {
        /// <summary>
        /// Attempts to load the module instance to the kernel
        /// </summary>
        /// <param name="kernel"></param>
        /// <param name="module">Module to load</param>
        /// <returns>IKernel instance with module loaded</returns>
        /// <exception cref="Exception">Thrown if unable to load the module</exception>
        /// <example>
        /// kernel.WithModule(new LoggingModule());
        /// </example>
        public static IKernel WithModule(this IKernel kernel, INinjectModule module)
        {
            if (!kernel.HasModule(module.Name))
            {
                try
                {
                    kernel.Load(module);
                }
                catch (Exception ex)
                {
                    var error = string.Format("Unable to load module '{0}' see inner exception.", module.Name);
                    throw new Exception(error, ex);
                }
            }

            return kernel;
        }

        /// <summary>
        /// Instantiates the module then checks if it has already been loaded. If not, it will then load the module.
        /// </summary>
        /// <typeparam name="TModule">Module type to load</typeparam>
        /// <param name="kernel"></param>
        /// <returns>IKernel instance with the module loaded</returns>
        /// <exception cref="Exception">Thrown if unable to instantiate or load the module</exception>
        /// <example>
        /// kernel.WithModule<LoggingModule>();
        /// </example>
        public static IKernel WithModule<TModule>(this IKernel kernel)
            where TModule : INinjectModule, new()
        {
            INinjectModule module = null;
            try
            {
                module = (INinjectModule)System.Activator.CreateInstance(typeof(TModule));
                if (!kernel.HasModule(module.Name))
                {
                    kernel.Load<TModule>();
                }
            }
            catch (Exception ex)
            {
                var error = string.Format("Unable to load module '{0}' see inner exception.", module.Name);
                throw new Exception(error, ex);
            }

            return kernel;
        }
    }
}