using Autofac;
using NLog;
using Noiser.Configuration;
using Noiser.Sources;
using System.Reflection;
using System.Linq;
using System;
using Noiser.TimeManagement;

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

            ParseConfig()
            .OnSuccess(cfg =>
            {
                GetNoiseFactory(cfg)
                .OnSuccess(factory =>
                {
                    var lifetime = CreateLifetime(cfg, factory);
                    if (IsHappeningNow(cfg.Settings.TimeSpan.From, cfg.Settings.TimeSpan.To))
                    {
                        logger.Info("Noiser beginning " + DateTime.Now);
                        lifetime.Begin();
                    }
                    else
                    {
                        InMemoryScheduler.ScheduleAction(() =>
                        {
                            logger.Info("Noiser beginning " + DateTime.Now);
                            lifetime.Begin();
                        }, cfg.Settings.TimeSpan.From, "Noise period start");
                    }
                    InMemoryScheduler.ScheduleAction(() =>
                    {
                        logger.Info("Noiser ending " + DateTime.Now);
                        lifetime.End();
                    }, cfg.Settings.TimeSpan.To, "Noise period disposal");
                });
            });
        }

        private bool IsHappeningNow(DateTime from, DateTime to)
        {
            var now = DateTime.Now;
            return (now > from && now < to);
        }

        public SourceLifetime<Result<IDisposable>> CreateLifetime(NoiserConfig cfg, ISourceSelector factory)
        =>
             SourceLifeTime.For(
                      () =>
                          factory.Next.Create(),
                      source => source.OnSuccess(src =>
                         src.Dispose()),
                      cfg.Settings.IntervalMinutes,
                      cfg.Settings.DurationMinutes);


        public Result<ISourceSelector> GetSourceSelector(NoiseOrder order, INoiseSource[] sources)
        {
            if (sources.Length == 0)
            {
                return Result.FailWith<ISourceSelector>(State.Error, "0 noise sources provided").Log(logger.Error);
            }
            switch (order)
            {
                case NoiseOrder.Random:
                    logger.Trace("Initializing RandomSourceSelector");
                    return Result.Ok(new RandomSourceSelector(sources) as ISourceSelector);
                case NoiseOrder.Sequenced:
                    logger.Trace("Initializing SequentialSourceSelector");
                    return Result.Ok(new SequentialSourceSelector(sources) as ISourceSelector);
                default:
                    logger.Trace("Initializing SequentialSourceSelector");
                    return Result.Ok(new SequentialSourceSelector(sources) as ISourceSelector);
            }
        }

        private Result<ISourceSelector> GetNoiseFactory(NoiserConfig config)
        =>
                GetSourceSelector(
                    config.Settings.Order,
                    config
                    .Noise
                    .Select(noise =>
                        Service<ISourceFactory>().GetSource(noise.Id, noise.Source))
                    .FilterSuccessful()
                    .Where(noise => noise.Validate().Success)
                    .ToArray()
                )
                .OnFailure(res => logger.Error($"Noiser Pipeline Failure. {res.ErrorMessage}"));


        public Result<NoiserConfig> ParseConfig()
        =>
            Service<IConfigurationLoader>()
           .Load()
           .OnSuccess(cfg => logger.Info($"Configuration Successfuly Loaded.{Environment.NewLine}{cfg.ToString()}"));

    }
}
