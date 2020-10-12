using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NvxEpi.Application.Config;
using PepperDash.Essentials;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;
using PepperDash.Essentials.Core.Routing;

namespace NvxEpi.Application.Builder
{
    public class DynNvxDeviceBuilder : IDynNvxBuilder
    {
        public string Key { get; private set; }
        public Dictionary<int, string> Transmitters { get; private set; }
        public Dictionary<int, string> Receivers { get; private set; }

        public EssentialsDevice Build()
        {
            return new NvxApplication(this);
        }

        public DynNvxDeviceBuilder(DeviceConfig config)
        {
            Key = config.Key;
            var props = JsonConvert.DeserializeObject<DynNvxConfig>(config.Properties.ToString());

            Transmitters = props.Transmitters.ToDictionary(x => Convert.ToInt32(x.Key), x => x.Value);
            Receivers = props.Receivers.ToDictionary(x => Convert.ToInt32(x.Key), x => x.Value);
        }
    }
}