using NLog;
using System;
using Noiser.TimeManagement;
using Autofac;
using System.Reflection;
using Noiser.Configuration;

namespace Noiser
{


    internal class Program
    {
        private static Logger logger = LogManager.GetLogger("Main");
        static void Main(string[] args)
        {
            var container = RegisterModules();

            var res= container.Resolve<IConfigurationLoader>().Load();

            //var source = new SourceLifetime<string>(() => { logger.Info("in"); return "start"; }, x => { logger.Info("out"); }, 0.5d, 0.1d);
            //source.Begin();

            while (Console.ReadLine() != "exit") { }
        }

        private static IContainer RegisterModules()
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyModules(Assembly.GetExecutingAssembly());
            return builder.Build();
        }
    }
}
