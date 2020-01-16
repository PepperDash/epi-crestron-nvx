using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PepperDash.Core;

namespace NvxEpi
{
    public class NvxDevicePropertiesConfig
    {
        [JsonProperty("deviceName")]
        public string DeviceName { get; set; }

        [JsonProperty("friendlyName")]
        public string FriendlyName { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("control")]
        public ControlPropertiesConfig Control { get; set; }
    }
}