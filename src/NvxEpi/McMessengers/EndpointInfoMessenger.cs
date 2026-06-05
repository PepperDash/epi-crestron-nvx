using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PepperDash.Essentials.AppServer.Messengers;
using PepperDash.Essentials.Core;

namespace NvxEpi.McMessengers
{
    public class EndpointInfoMessenger : MessengerBase
    {
        private readonly StringFeedback deviceNameFeedback;
        public EndpointInfoMessenger(string key, string path, StringFeedback deviceNameFeedback, INvxDevice device) : base(key, path, device)
        {
            this.deviceNameFeedback = deviceNameFeedback ?? throw new ArgumentNullException(nameof(deviceNameFeedback));
            this.deviceNameFeedback.OutputChange += SendUpdate;
        }

        protected override void RegisterActions()
        {
            base.RegisterActions();

            AddAction("/fullStatus", SendFullStatus);
            AddAction("/endpointInfo", SendFullStatus);
        }

        private void SendFullStatus(string id, JToken content) =>
            PostStatusMessage(new EndpointInfoStateMessage
            {
                DeviceName = deviceNameFeedback?.StringValue ?? string.Empty
            }, id);

        private void SendUpdate(object sender, FeedbackEventArgs args) =>
            PostStatusMessage(
                JToken.FromObject(new EndpointInfoUpdateMessage
                {
                    DeviceName = args.StringValue ?? string.Empty
                }));
    }

    public class EndpointInfoStateMessage : DeviceStateMessageBase
    {
        [JsonProperty("friendlyName")]
        public required string DeviceName { get; set; }
    }

    public class EndpointInfoUpdateMessage
    {
        [JsonProperty("friendlyName")]
        public required string DeviceName { get; set; }
    }
}
