using Autofac;

namespace Noiser.Infrastructure
{
    internal class InfrastructureModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            new ConsoleNLogTarget();
        }
    }
}
