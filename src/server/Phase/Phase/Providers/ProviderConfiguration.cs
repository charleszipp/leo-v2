using System.Collections.Generic;

namespace Phase.Providers
{
    public class ProviderConfiguration
    {
        public ProviderConfiguration(string serviceTypeName, string executingDirectory, string providerName, KeyValuePair<string, object> partitionKey, IDictionary<string, string> serviceInstanceKeys)
        {
            ServiceTypeName = serviceTypeName;
            ExecutingDirectory = executingDirectory;
            ProviderName = providerName;
            PartitionKey = partitionKey;
            ServiceInstanceKeys = serviceInstanceKeys;
            ServiceInstanceKeys[partitionKey.Key] = partitionKey.Value.ToString();
        }

        public string ExecutingDirectory { get; }

        public KeyValuePair<string, object> PartitionKey { get; }

        public string ProviderName { get; }

        public IDictionary<string, string> ServiceInstanceKeys { get; }

        public string ServiceTypeName { get; }
    }
}