using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Noiser.Configuration
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class NoiserConfig
    {
        [JsonProperty("settings")]
        public NoiserSettings Settings { get; set; }

        [JsonProperty("noise")]
        public IEnumerable<NoiseItem> Noise { get; set; }

        public class NoiseItem
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("source")]
            public string Source { get; set; }
        }
    }

    internal class NoiserSettings
    {
        internal class Time
        {
            [JsonProperty("from")]
            public DateTime From { get; set; }

            [JsonProperty("to")]
            public DateTime To { get; set; }
        }

        [JsonProperty("time")]
        public Time TimeSpan { get; set; }

        [JsonProperty("order")]
        public NoiseOrder Order { get; set; }

        [JsonProperty("repeat")]
        public bool Repeat { get; set; }

        [JsonProperty("intervalMinutes")]
        public double IntervalMinutes { get; set; }

        [JsonProperty("durationMinutes")]
        public double DurationMinutes { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum NoiseOrder
    {
        [EnumMember(Value = "random")]
        Random,

        [EnumMember(Value = "sequenced")]
        Sequenced,
    }
}
