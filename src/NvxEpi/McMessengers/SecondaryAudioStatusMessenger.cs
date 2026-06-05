using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PepperDash.Essentials.AppServer.Messengers;
using PepperDash.Essentials.Core;
using System.Threading;

namespace NvxEpi.McMessengers
{
    public class SecondaryAudioStatusMessenger : MessengerBase
    {
        private readonly INvxDevice device;
        private readonly Timer debounceTimer;

        public SecondaryAudioStatusMessenger(string key, string path, INvxDevice device) : base(key, path, device)
        {
            this.device = device;
            debounceTimer = new Timer(_ => SendUpdate(), null, Timeout.Infinite, Timeout.Infinite);
        }

        protected override void RegisterActions()
        {
            base.RegisterActions();

            AddAction("/fullStatus", SendFullStatus);
            AddAction("/secondaryAudioStatus", SendFullStatus);

            device.IsTransmittingDmNaxFeedback.OutputChange += Debounce;
            device.IsReceivingDmNaxFeedback.OutputChange += Debounce;
            device.DmNaxTransmitStatusFeedback.OutputChange += Debounce;
            device.DmNaxReceiveStatusFeedback.OutputChange += Debounce;
            device.DmNaxTxAddressFeedback.OutputChange += Debounce;
            device.DmNaxRxAddressFeedback.OutputChange += Debounce;
        }

        private void Debounce(object sender, FeedbackEventArgs args) => debounceTimer.Change(200, Timeout.Infinite);

        private void SendFullStatus(string id, JToken content) => PostStatusMessage(BuildStateMessage(), id);

        private void SendUpdate() => PostStatusMessage(JToken.FromObject(BuildUpdateMessage()));

        private SecondaryAudioStateMessage BuildStateMessage()
        {
            var isTransmitter = device.IsTransmitter;
            var isStreaming = isTransmitter ? device.IsTransmittingDmNaxFeedback.BoolValue : device.IsReceivingDmNaxFeedback.BoolValue;
            var status = isTransmitter ? device.DmNaxTransmitStatusFeedback.StringValue : device.DmNaxReceiveStatusFeedback.StringValue;
            var streamUrl = isTransmitter ? device.DmNaxTxAddressFeedback.StringValue : device.DmNaxRxAddressFeedback.StringValue;

            return new SecondaryAudioStateMessage(
                isStreamingSecondaryAudio: isStreaming,
                secondaryAudioStreamStatus: status,
                secondaryAudioStreamUrl: streamUrl);
        }

        private SecondaryAudioUpdateMessage BuildUpdateMessage()
        {
            var isTransmitter = device.IsTransmitter;
            var isStreaming = isTransmitter ? device.IsTransmittingDmNaxFeedback.BoolValue : device.IsReceivingDmNaxFeedback.BoolValue;
            var status = isTransmitter ? device.DmNaxTransmitStatusFeedback.StringValue : device.DmNaxReceiveStatusFeedback.StringValue;
            var streamUrl = isTransmitter ? device.DmNaxTxAddressFeedback.StringValue : device.DmNaxRxAddressFeedback.StringValue;

            return new SecondaryAudioUpdateMessage(
                isStreamingSecondaryAudio: isStreaming,
                secondaryAudioStreamStatus: status,
                secondaryAudioStreamUrl: streamUrl);
        }
    }

    public class SecondaryAudioStateMessage(bool isStreamingSecondaryAudio, string secondaryAudioStreamStatus, string secondaryAudioStreamUrl) : DeviceStateMessageBase
    {
        [JsonProperty("isStreamingSecondaryAudio")]
        public bool IsStreamingSecondaryAudio { get; } = isStreamingSecondaryAudio;

        [JsonProperty("secondaryAudioStreamStatus")]
        public string SecondaryAudioStreamStatus { get; } = secondaryAudioStreamStatus;

        [JsonProperty("secondaryAudioStreamUrl")]
        public string SecondaryAudioStreamUrl { get; } = secondaryAudioStreamUrl;
    }

    public class SecondaryAudioUpdateMessage(bool isStreamingSecondaryAudio, string secondaryAudioStreamStatus, string secondaryAudioStreamUrl) : DeviceStateMessageBase
    {
        [JsonProperty("isStreamingSecondaryAudio")]
        public bool IsStreamingSecondaryAudio { get; } = isStreamingSecondaryAudio;

        [JsonProperty("secondaryAudioStreamStatus")]
        public string SecondaryAudioStreamStatus { get; } = secondaryAudioStreamStatus;

        [JsonProperty("secondaryAudioStreamUrl")]
        public string SecondaryAudioStreamUrl { get; } = secondaryAudioStreamUrl;
    }
}
