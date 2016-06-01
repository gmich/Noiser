using Autofac;

namespace Noiser.Configuration
{
    internal class ConfigurationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
            .Register(c => new ConfigFileReader("noiser.config"))
            .As<IConfigurationLoader>();
        }
    }
}
