using System;

namespace Phase
{
    public delegate object DependencyResolver(Type interfaceType);

    public interface IDependencyResolver
    {
        object Get(Type interfaceType);
        bool TryGet(Type interfaceType, out object instance);
        bool TryGet<T>(out object instance);
    }
}