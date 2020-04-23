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
    public class NvxRouterPropertiesConfig
    {
        public static NvxRouterPropertiesConfig FromDeviceConfig(DeviceConfig config)
        {
            return JsonConvert.DeserializeObject<NvxRouterPropertiesConfig>(config.Properties.ToString());
        }

        [JsonProperty("deviceUnusedText")]
        public string DeviceUnusedText { get; private set; }

        [JsonProperty("numberOfInputs")]
        public int NumberOfInputs { get; private set; }

        [JsonProperty("numberOfOutputs")]
        public int NumberOfOutputs { get; private set; }
    }
}