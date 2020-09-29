using Newtonsoft.Json;
using PepperDash.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Entities.Config
{
    public class NvxDeviceProperties
    {
        public static NvxDeviceProperties FromDeviceConfig(DeviceConfig config)
        {
            return JsonConvert.DeserializeObject<NvxDeviceProperties>(config.Properties.ToString());
        }

        [JsonProperty("deviceId")]
        public int DeviceId { get; set; }

        [JsonProperty("control")]
        public ControlPropertiesConfig Control { get; set; }

        [JsonProperty("usbMode")]
        public string UsbMode { get; set; }

        [JsonProperty("mode")]
        public string Mode { get; set; }

        [JsonProperty("multicastVideoAddress")]
        public string MulticastVideoAddress { get; set; }

        [JsonProperty("multicastAudioAddress")]
        public string MulticastAudioAddress { get; set; }

        [JsonProperty("defaultVideoSource")]
        public string DefaultVideoSource { get; set; }

        [JsonProperty("defaultAudioSource")]
        public string DefaultAudioSource { get; set; }   
    }
}