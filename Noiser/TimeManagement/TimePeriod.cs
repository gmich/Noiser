using System;
using System.Threading;

namespace Noiser.TimeManagement
{
    internal class TimePeriod
    {
        public void TimerCallback(object state) { }
        public TimePeriod()
        {
            var t = new Timer(TimerCallback);

            DateTime now = DateTime.Now;
            DateTime fourOClock = DateTime.Today.AddHours(16.0);

            if (now > fourOClock)
            {
                fourOClock = fourOClock.AddDays(1.0);
            }

            int msUntilFour = (int)((fourOClock - now).TotalMilliseconds);

            t.Change(msUntilFour, Timeout.Infinite);
        }
    }
}
