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
        public Dictionary<int, MockDisplay> VideoDestinations { get; private set; }
        public Dictionary<int, Amplifier> AudioDestinations { get; private set; }
        public Dictionary<int, DummyRoutingInputsDevice> Sources { get; private set; }

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

            VideoDestinations = new Dictionary<int, MockDisplay>();
            AudioDestinations = new Dictionary<int, Amplifier>();
            Sources = new Dictionary<int, DummyRoutingInputsDevice>();

            foreach (var rx in Receivers)
            {
                VideoDestinations.Add(rx.Key, new MockDisplay(rx.Value + "--Display", rx.Value + "--Display"));
                AudioDestinations.Add(rx.Key, new Amplifier(rx.Value + "--Audio", rx.Value + "--Audio"));
            }

            foreach (var tx in Transmitters)
            {
                Sources.Add(tx.Key, new DummyRoutingInputsDevice(tx.Value + "--Source"));
            }
        }
    }
}