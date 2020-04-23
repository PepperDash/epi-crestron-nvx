using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PepperDash.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi
{
    public class NvxDevicePropertiesConfig
    {
        public static NvxDevicePropertiesConfig FromDeviceConfig(DeviceConfig config)
        {
            return JsonConvert.DeserializeObject<NvxDevicePropertiesConfig>(config.Properties.ToString());
        }

        [JsonProperty("deviceName")]
        public string DeviceName { get; set; }

        [JsonProperty("friendlyName")]
        public string FriendlyName { get; set; }

        [JsonProperty("parentDeviceKey")]
        public string ParentDeviceKey { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("control")]
        public ControlPropertiesConfig Control { get; set; }

        [JsonProperty("usbMode")]
        public string UsbMode { get; set; }

        [JsonProperty("virtualDevice")]
        public int VirtualDevice { get; set; }

        [JsonProperty("mode")]
        public string Mode { get; set; }

        [JsonProperty("enableAudioBreakaway")]
        public bool EnableAudioBreakaway { get; set; }

        [JsonProperty("multicastVideoAddress")]
        public string MulticastVideoAddress { get; set; }

        [JsonProperty("multicastAudioAddress")]
        public string MulticastAudioAddress { get; set; }
    }
}