using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NvxEpi.Devices;
using PepperDash.Essentials.AppServer.Messengers;
using PepperDash.Essentials.Core;

namespace NvxEpi.McMessengers
{
    public class PrimaryStreamStatusMessenger:MessengerBase
    {
        private readonly NvxBaseDevice device;
        public PrimaryStreamStatusMessenger(string key, string messagePath, NvxBaseDevice device) : base(key, messagePath, device)
        {
            this.device = device;
        }

        protected override void RegisterActions()
        {
            base.RegisterActions();

            AddAction("/fullStatus", SendFullStatus);

            device.IsStreamingVideo.OutputChange += HandleUpdate;
            device.VideoStreamStatus.OutputChange += HandleUpdate;
            device.StreamUrl.OutputChange += HandleUpdate;
            device.MulticastAddress.OutputChange += HandleUpdate;
        }

        private void SendFullStatus(string id, JToken content) {
            PostStatusMessage(new StreamStateMessage(device));
        }

        private void HandleUpdate(object sender, FeedbackEventArgs args)
        {
            PostStatusMessage(JToken.FromObject(new StreamUpdateMessage(device)));
        }
    }

    public class StreamStateMessage : DeviceStateMessageBase
    {
        [JsonIgnore]
        private readonly NvxBaseDevice device;

        [JsonProperty("isStreamingVideo")]
        public bool IsStreamingVideo => device.IsStreamingVideo.BoolValue;

        [JsonProperty("videoStreamStatus")]
        public string VideoStreamStatus => device.VideoStreamStatus.StringValue;

        [JsonProperty("streamUrl")]
        public string StreamUrl => device.StreamUrl.StringValue;

        [JsonProperty("multicastAddress")]
        public string MulticastAddress => device.MulticastAddress.StringValue;

        [JsonProperty("isTransmitter")]
        public bool IsTransmitter => device.IsTransmitter;

        public StreamStateMessage(NvxBaseDevice device)
        {
            this.device = device;
        }
    }

    public class StreamUpdateMessage
    {
        [JsonIgnore]
        private readonly NvxBaseDevice device;

        [JsonProperty("isStreamingVideo")]
        public bool IsStreamingVideo => device.IsStreamingVideo.BoolValue;

        [JsonProperty("videoStreamStatus")]
        public string VideoStreamStatus => device.VideoStreamStatus.StringValue;

        [JsonProperty("streamUrl")]
        public string StreamUrl => device.StreamUrl.StringValue;

        [JsonProperty("multicastAddress")]
        public string MulticastAddress => device.MulticastAddress.StringValue;

        [JsonProperty("isTransmitter")]
        public bool IsTransmitter => device.IsTransmitter;

        public StreamUpdateMessage(NvxBaseDevice device)
        {
            this.device = device;
        }
    }
}
