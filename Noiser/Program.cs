using NLog;
using System;

namespace Noiser
{
    internal class Program
    {
        private static Logger logger = LogManager.GetLogger("Noiser");

        static void Main(string[] args)
        {
            new NoiserPipeline();

            while (Console.ReadLine() != "exit") { }
        }

    }
}
