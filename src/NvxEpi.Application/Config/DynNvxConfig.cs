using System.Collections.Generic;
using Newtonsoft.Json;

namespace NvxEpi.Application.Config
{
    public class DynNvxConfig
    {
        [JsonProperty("transmitters")]
        public Dictionary<string, string> Transmitters { get; set; }

        [JsonProperty("receivers")]
        public Dictionary<string, string> Receivers { get; set; }
    }
}