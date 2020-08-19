using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;
using NvxEpi.DynRouting.Config;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.DynRouting.Builder
{
    public class DynNvxDeviceBuilder : IDynNvxBuilder
    {
        public string Key { get; private set; }
        public Dictionary<int, string> Transmitters { get; private set; }
        public Dictionary<int, string> Receivers { get; private set; }

        public EssentialsDevice Build()
        {
            return new DynNvx(this);
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