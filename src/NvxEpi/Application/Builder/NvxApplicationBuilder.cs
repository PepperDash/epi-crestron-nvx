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
    public class NvxApplicationApplicationBuilder : INvxApplicationBuilder
    {
        public string Key { get; private set; }
        public Dictionary<int, NvxApplicationDeviceVideoConfig> Transmitters { get; private set; }
        public Dictionary<int, NvxApplicationDeviceVideoConfig> Receivers { get; private set; }
        public Dictionary<int, NvxApplicationDeviceAudioConfig> AudioTransmitters { get; private set; }
        public Dictionary<int, NvxApplicationDeviceAudioConfig> AudioReceivers { get; private set; }

        public EssentialsDevice Build()
        {
            return new NvxApplication(this);
        }

        public NvxApplicationApplicationBuilder(DeviceConfig config)
        {
            Transmitters = new Dictionary<int, NvxApplicationDeviceVideoConfig>();
            Receivers = new Dictionary<int, NvxApplicationDeviceVideoConfig>();
            AudioTransmitters = new Dictionary<int, NvxApplicationDeviceAudioConfig>();
            AudioReceivers = new Dictionary<int, NvxApplicationDeviceAudioConfig>();

            Key = config.Key;
            var props = JsonConvert.DeserializeObject<NvxApplicationConfig>(config.Properties.ToString());

            if (props.Transmitters != null)
                Transmitters = props.Transmitters.ToDictionary(x => Convert.ToInt32(x.Key), x => x.Value);
            
            if (props.Receivers != null)
                Receivers = props.Receivers.ToDictionary(x => Convert.ToInt32(x.Key), x => x.Value);

            if (props.AudioTransmitters != null)
                AudioTransmitters = props.AudioTransmitters.ToDictionary(x => Convert.ToInt32(x.Key), x => x.Value);

            if (props.AudioReceivers != null)
                AudioReceivers = props.AudioReceivers.ToDictionary(x => Convert.ToInt32(x.Key), x => x.Value);
        }
    }
}