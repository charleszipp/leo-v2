using System.Collections.Generic;

namespace Phase.Providers.Memory
{
    public class InMemoryEventsProviderConfiguration
    {
        public InMemoryEventsProviderConfiguration(IDictionary<string, string> serviceInstanceKeys)
        {
            ServiceInstanceKeys = serviceInstanceKeys;
        }

        public IDictionary<string, string> ServiceInstanceKeys { get; }
    }
}