using System.Collections.Generic;
using Newtonsoft.Json;

namespace NvxEpi.Application.Config
{
    public class NvxApplicationConfig
    {
        [JsonProperty("transmitters")]
        public Dictionary<string, string> Transmitters { get; set; }

        [JsonProperty("receivers")]
        public Dictionary<string, string> Receivers { get; set; }

        [JsonProperty("audioTransmitters")]
        public Dictionary<string, string> AudioTransmitters { get; set; }

        [JsonProperty("audioReceivers")]
        public Dictionary<string, string> AudioReceivers { get; set; }
    }
}