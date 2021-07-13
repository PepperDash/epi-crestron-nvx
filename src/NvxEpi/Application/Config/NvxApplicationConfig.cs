using System.Collections.Generic;
using Newtonsoft.Json;

namespace NvxEpi.Application.Config
{
    public class NvxApplicationConfig
    {
        [JsonProperty("transmitters")]
        public Dictionary<string, NvxApplicationDeviceVideoConfig> Transmitters { get; set; }

        [JsonProperty("receivers")]
        public Dictionary<string, NvxApplicationDeviceVideoConfig> Receivers { get; set; }

        [JsonProperty("audioTransmitters")]
        public Dictionary<string, NvxApplicationDeviceAudioConfig> AudioTransmitters { get; set; }

        [JsonProperty("audioReceivers")]
        public Dictionary<string, NvxApplicationDeviceAudioConfig> AudioReceivers { get; set; }
    }

    public class NvxApplicationDeviceVideoConfig
    {
        public string DeviceKey { get; set; }
        public string VideoName { get; set; }
        public string NvxRoutingPort { get; set; }
    }

    public class NvxApplicationDeviceAudioConfig
    {
        public string DeviceKey { get; set; }
        public string AudioName { get; set; }
        public string NvxRoutingPort { get; set; }
    }
}