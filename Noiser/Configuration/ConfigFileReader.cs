using Newtonsoft.Json;
using System;
using System.IO;

namespace Noiser.Configuration
{
    internal class ConfigFileReader : IConfigurationReader
    {
        private readonly string path;

        public ConfigFileReader(string path)
        {
            this.path = path;
        }
        public NoiserConfig Read()
        {
            try
            {
                var jsonText = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<NoiserConfig>(jsonText);
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}
