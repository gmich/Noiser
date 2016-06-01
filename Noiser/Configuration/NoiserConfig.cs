using Newtonsoft.Json;
using System;

namespace Noiser.Configuration
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class NoiserConfig
    {
        internal class Settings
        {
            internal class Time
            {
                [JsonProperty("from")]
                public DateTime From { get; set; }

                [JsonProperty("to")]
                public DateTime To { get;  set; }
            }

            [JsonProperty("time")]
            public Time TimeSpan { get; set; }
        }
    }
}
