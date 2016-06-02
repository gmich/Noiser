using NLog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Noiser.TimeManagement
{
    internal static class InMemoryScheduler
    {
        private static Logger logger = LogManager.GetLogger("InMemoryScheduler");

        public async static void ScheduleAction(Action action, DateTime ExecutionTime, string actionInfo = "")
        {
            if(ExecutionTime< DateTime.Now)
            {
                ExecutionTime = ExecutionTime.AddDays(1);
            }
            logger.Trace($"Scheduling action for {ExecutionTime} {actionInfo}");
            await Task.Delay((int)ExecutionTime.Subtract(DateTime.Now).TotalMilliseconds);
            action();
        }
    }
}
