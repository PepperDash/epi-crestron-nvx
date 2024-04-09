using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NvxEpi.Devices;
using PepperDash.Essentials.AppServer.Messengers;
using PepperDash.Essentials.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NvxEpi.McMessengers
{
    public class SecondaryAudioStatusMessenger:MessengerBase
    {
        private readonly NvxBaseDevice device;
        public SecondaryAudioStatusMessenger(string key, string path, NvxBaseDevice device):base(key, path, device)
        {
            this.device = device;
        }

        protected override void RegisterActions()
        {
            base.RegisterActions();

            AddAction("/fullStatus", SendFullStatus);
        }

        private void SendFullStatus(string id, JToken content)
        {
            PostStatusMessage(new SecondaryAudioStateMessage(device));
        }

        private void SendUpdate(object sender, FeedbackEventArgs args)
        {
            PostStatusMessage(JToken.FromObject(new SecondaryAudioUpdateMessage(device)));
        }
    }

    public class SecondaryAudioStateMessage : DeviceStateMessageBase
    {
        [JsonIgnore]
        private readonly NvxBaseDevice device;

        [JsonProperty("isStreamingSecondaryAudio")]
        public bool IsStreamingSecondaryAudio => device.IsStreamingSecondaryAudio.BoolValue;

        [JsonProperty("secondaryAudioStreamStatus")]
        public string SecondaryAudioStreamStatus => device.SecondaryAudioStreamStatus.StringValue;

        [JsonProperty("secondaryAudioStreamUrl")]
        public string SecondaryAudioStreamUrl => device.SecondaryAudioAddress.StringValue;

        public SecondaryAudioStateMessage(NvxBaseDevice device)
        {
            this.device = device;
        }
    }

    public class SecondaryAudioUpdateMessage : DeviceStateMessageBase
    {
        [JsonIgnore]
        private readonly NvxBaseDevice device;

        [JsonProperty("isStreamingSecondaryAudio")]
        public bool IsStreamingSecondaryAudio => device.IsStreamingSecondaryAudio.BoolValue;

        [JsonProperty("secondaryAudioStreamStatus")]
        public string SecondaryAudioStreamStatus => device.SecondaryAudioStreamStatus.StringValue;

        [JsonProperty("secondaryAudioStreamUrl")]
        public string SecondaryAudioStreamUrl => device.SecondaryAudioAddress.StringValue;

        public SecondaryAudioUpdateMessage(NvxBaseDevice device)
        {
            this.device = device;
        }
    }
}
