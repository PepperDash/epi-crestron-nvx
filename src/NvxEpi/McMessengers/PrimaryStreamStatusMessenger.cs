using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PepperDash.Essentials.AppServer.Messengers;
using PepperDash.Essentials.Core;
using System.Threading;

namespace NvxEpi.McMessengers
{
    public class PrimaryStreamStatusMessenger : MessengerBase
    {
        private readonly INvxDevice device;
        private readonly Timer debounceTimer;

        public PrimaryStreamStatusMessenger(string key, string path, INvxDevice device) : base(key, path, device)
        {
            this.device = device;
            debounceTimer = new Timer(_ => SendUpdate(), null, Timeout.Infinite, Timeout.Infinite);
        }

        protected override void RegisterActions()
        {
            base.RegisterActions();

            AddAction("/fullStatus", SendFullStatus);
            AddAction("/videoStreamStatus", SendFullStatus);

            device.IsStreamingFeedback.OutputChange += Debounce;
            device.StreamStatusFeedback.OutputChange += Debounce;
            device.StreamUrlFeedback.OutputChange += Debounce;
        }

        private void Debounce(object sender, FeedbackEventArgs args) => debounceTimer.Change(200, Timeout.Infinite);

        private void SendFullStatus(string id, JToken content) => PostStatusMessage(BuildStateMessage(), id);

        private void SendUpdate() => PostStatusMessage(JToken.FromObject(BuildUpdateMessage()));

        private StreamStateMessage BuildStateMessage()
        {
            var isTransmitter = device.IsTransmitter;

            return new StreamStateMessage(
                isStreamingVideo: device.IsStreamingFeedback.BoolValue,
                videoStreamStatus: device.StreamStatusFeedback.StringValue,
                streamUrl: device.StreamUrlFeedback.StringValue,
                multicastAddress: device.MulticastAddressFeedback.StringValue,
                isTransmitter: isTransmitter);
        }

        private StreamUpdateMessage BuildUpdateMessage()
        {
            var isTransmitter = device.IsTransmitter;

            return new StreamUpdateMessage(
                isStreamingVideo: device.IsStreamingFeedback.BoolValue,
                videoStreamStatus: device.StreamStatusFeedback.StringValue,
                streamUrl: device.StreamUrlFeedback.StringValue,
                multicastAddress: device.MulticastAddressFeedback.StringValue,
                isTransmitter: isTransmitter);
        }
    }

    public class StreamStateMessage(bool isStreamingVideo, string videoStreamStatus, string streamUrl, string multicastAddress, bool isTransmitter) : DeviceStateMessageBase
    {
        [JsonProperty("isStreamingVideo")]
        public bool IsStreamingVideo { get; } = isStreamingVideo;

        [JsonProperty("videoStreamStatus")]
        public string VideoStreamStatus { get; } = videoStreamStatus;

        [JsonProperty("streamUrl")]
        public string StreamUrl { get; } = streamUrl;

        [JsonProperty("multicastAddress")]
        public string MulticastAddress { get; } = multicastAddress;

        [JsonProperty("isTransmitter")]
        public bool IsTransmitter { get; } = isTransmitter;
    }

    public class StreamUpdateMessage(bool isStreamingVideo, string videoStreamStatus, string streamUrl, string multicastAddress, bool isTransmitter)
    {
        [JsonProperty("isStreamingVideo")]
        public bool IsStreamingVideo { get; } = isStreamingVideo;

        [JsonProperty("videoStreamStatus")]
        public string VideoStreamStatus { get; } = videoStreamStatus;

        [JsonProperty("streamUrl")]
        public string StreamUrl { get; } = streamUrl;

        [JsonProperty("multicastAddress")]
        public string MulticastAddress { get; } = multicastAddress;

        [JsonProperty("isTransmitter")]
        public bool IsTransmitter { get; } = isTransmitter;
    }
}
