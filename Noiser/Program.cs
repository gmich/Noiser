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
            new NoiserPipeline().Execute();
            //var source = new SourceLifetime<string>(() => { logger.Info("in"); return "start"; }, x => { logger.Info("out"); }, 0.5d, 0.1d);
            //source.Begin();

            while (Console.ReadLine() != "exit") { }
        }


    }
}
