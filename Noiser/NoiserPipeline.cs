using Autofac;
using NLog;
using Noiser.Configuration;
using Noiser.Sources;
using System.Reflection;
using System.Linq;
using System;

namespace Noiser
{
    internal class NoiserPipeline
    {
        private static Logger logger = LogManager.GetLogger("Noiser");
        private readonly IContainer container;

        private TService Service<TService>() => container.Resolve<TService>();

        public NoiserPipeline()

        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyModules(Assembly.GetExecutingAssembly());
            container = builder.Build();
            //Parse config
            //Create sources
            //Validate sources
            //Configure time period
            //Add the source lifetime to the start of the time period
            //Dispose the source at the end
        }

        public void Execute()
        {
            ParseConfig()
            .OnSuccess(res =>
                res
                .Noise
                .Select(noise =>
                    Service<ISourceFactory>().GetSource(noise.Id, noise.Source))
                .FilterSuccessful()
                .Select(noise => noise.Validate())
                .FilterSuccessful()
                .ToList()
             )
            .OnFailure(res => logger.Error($"Noiser Pipeline Failure. {res.ErrorMessage}"));
        }

        public Result<NoiserConfig> ParseConfig()
        =>
            Service<IConfigurationLoader>()
           .Load()
           .OnSuccess(cfg => logger.Info($"Configuration Successfuly Loaded.{Environment.NewLine}{cfg.ToString()}"));

    }
}
