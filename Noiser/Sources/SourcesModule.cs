using Autofac;

namespace Noiser.Sources
{
    internal class SourcesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
            .RegisterType<SourceFactory>()
            .As<ISourceFactory>();
        }
    }
}
