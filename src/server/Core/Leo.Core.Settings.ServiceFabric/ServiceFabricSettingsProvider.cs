using System.Collections.Generic;
using System.Fabric;
using System.Linq;

namespace Leo.Core.Settings.ServiceFabric
{
    public static class ServiceFabricExtensions
    {
        public static IDictionary<string, string> ToSettingsDictionary(this ICodePackageActivationContext context)
        {
            IDictionary<string, string> rvalues = new Dictionary<string, string>();
            var packages = context.GetConfigurationPackageNames();
            foreach (var package in packages)
            {
                var sections = context.GetConfigurationPackageObject(package).Settings.Sections;
                if (sections?.Any() ?? false)
                {
                    foreach (var section in sections)
                    {
                        if (section.Parameters?.Any() ?? false)
                        {
                            foreach (var parameter in section.Parameters)
                            {
                                string name = $"{section.Name}.{parameter.Name}";
                                string value = parameter.IsEncrypted ? parameter.DecryptValue().ToString() : parameter.Value;
                                rvalues.Add(name, value);
                            }
                        }
                    }
                }
            }

            return rvalues;
        }
    }

    public class ServiceFabricSettingsProvider : SettingsProvider
    {
        public ServiceFabricSettingsProvider(ICodePackageActivationContext context)
            : base(context.ToSettingsDictionary())
        {
            //todo: add context and subscribe to config change events to get live config updates!
        }
    }
}