using Newtonsoft.Json;
using NLog;
using System;
using System.IO;

namespace Noiser.Configuration
{
    internal class ConfigFileReader : IConfigurationLoader
    {
        private readonly string path;
        private static Logger logger = LogManager.GetLogger("ConfigurationLoader");

        public ConfigFileReader(string path)
        {
            DebugArgument.Require.NotNull(() => path);
            this.path = path;
        }

        public Result<NoiserConfig> Load()
        {
            try
            {
                var jsonText = File.ReadAllText(path);
                logger.Trace(jsonText);
                return Result.Ok(JsonConvert.DeserializeObject<NoiserConfig>(jsonText));
            }
            catch (Exception ex)
            {
                return Result.FailWith<NoiserConfig>(State.Error, $"Unable to parse the configuration. {ex.Message}");
            }
        }
    }
}
