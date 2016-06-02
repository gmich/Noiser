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
            logger.Trace("Starting cycle ");

            cleanup = observable.Delay(TimeSpan.FromMinutes(durationMinutes)).Subscribe(time =>
            {
                logger.Trace("Ending interval ");
                intervalEnd(item);
            });

            start = observable.Subscribe(x =>
            {
                logger.Trace("Starting Interval ");
                item = intervalStart();
            });
        }

        public void End()
        {
            logger.Trace("Ending cycle ");
            start.Dispose();
            cleanup.Dispose();
            try
            {
                intervalEnd(item);
            }
            catch { }
        }
    }

    public static class SourceLifeTime
    {
        public static SourceLifetime<T> For<T>(Func<T> intervalStart, Action<T> intervalEnd, double cycleMinutes, double durationMinutes)
        {
            return new SourceLifetime<T>(intervalStart, intervalEnd, cycleMinutes, durationMinutes);
        }
    }
}
