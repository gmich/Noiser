namespace Noiser
{
    using NLog;
    using System;
    using TimeManagement;
    internal class Program
    {
        private static Logger logger = LogManager.GetLogger("Main");
        static void Main(string[] args)
        {
            new Infrastructure.ConsoleNLogTarget();

            var source= new SourceLifetime<string>(() => { logger.Info("in"); return "start"; }, x => { logger.Info("out"); }, 0.5d, 0.1d);

            source.Begin();

            while (Console.ReadLine() != "exit") { }
        }
    }
}
