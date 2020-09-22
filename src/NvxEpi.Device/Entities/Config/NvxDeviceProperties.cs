using Newtonsoft.Json;
using NvxEpi.Device.Services.Json;
using PepperDash.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Device.Entities.Config
{
    public class NvxDeviceProperties
    {
        public static NvxDeviceProperties FromDeviceConfig(DeviceConfig config)
        {
            return JsonConvert.DeserializeObject<NvxDeviceProperties>(config.Properties.ToString());
        }

        [JsonProperty("virtualDeviceId")]
        public int DeviceId { get; set; }

        [JsonProperty("friendlyName")]
        public string FriendlyName { get; set; }

        [JsonProperty("control")]
        public ControlPropertiesConfig Control { get; set; }

        [JsonProperty("usbMode")]
        public string UsbMode { get; set; }

        [JsonProperty("mode")]
        public string Mode { get; set; }

        [JsonProperty("mode")]
        [JsonConverter(typeof(DeviceModeConverter))]
        public bool IsTransmitter { get; set; }

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