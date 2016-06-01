using NLog;
using System;
using System.Reactive.Linq;

namespace Noiser.TimeManagement
{
    public class SourceLifetime<TItem>
    {

        private static Logger logger = LogManager.GetLogger("SourceLifetime");

        private readonly Func<TItem> intervalStart;
        private readonly Action<TItem> intervalEnd;
        private readonly double cycleMinutes;
        private readonly double durationMinutes;
        private TItem item;
        private IDisposable start;
        private IDisposable cleanup;

        public SourceLifetime(Func<TItem> intervalStart, Action<TItem> intervalEnd, double cycleMinutes, double durationMinutes)
        {
            this.intervalStart = intervalStart;
            this.intervalEnd = intervalEnd;
            this.cycleMinutes = cycleMinutes;
            this.durationMinutes = durationMinutes;
        }

        public void Begin()
        {
            var observable = Observable.Interval(TimeSpan.FromMinutes(cycleMinutes));

            cleanup = observable.Delay(TimeSpan.FromMinutes(durationMinutes)).Subscribe(time =>
            {
                logger.Info("Firing with delay");
                intervalEnd(item);
            });

            start = observable.Subscribe(x =>
            {
                logger.Info("Subscribed");
                item = intervalStart();
            });
        }

        public void End()
        {
            start.Dispose();
            cleanup.Dispose();
            try
            {
                intervalEnd(item);
            }
            catch { }
        }
    }
}
