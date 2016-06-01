﻿using NLog;
using NLog.Config;
using NLog.Targets;

namespace Noiser.Infrastructure
{
    internal class ConsoleNLogTarget
    {
        public ConsoleNLogTarget()
        {
            string consoleLayout = "[${longdate}] ${level} ${logger} - ${message}";
            var cfg = new LoggingConfiguration();
            var consoleTarget = new ColoredConsoleTarget();
            cfg.AddTarget("console", consoleTarget);
            consoleTarget.Layout = consoleLayout;
            cfg.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, consoleTarget));
            LogManager.Configuration = cfg;
            LogManager.ThrowExceptions = true;
        }
    }
}
