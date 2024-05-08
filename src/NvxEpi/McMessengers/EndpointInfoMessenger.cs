using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NvxEpi.Devices;
using NvxEpi.Services.Feedback;
using PepperDash.Essentials.AppServer.Messengers;
using PepperDash.Essentials.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NvxEpi.McMessengers
{
    public class EndpointInfoMessenger:MessengerBase
    {
        private readonly NvxBaseDevice device;

        private readonly StringFeedback deviceNameFeedback;
        public EndpointInfoMessenger(string key, string path, NvxBaseDevice device): base(key, path, device)
        {
            this.device = device;

            deviceNameFeedback = device.Feedbacks.FirstOrDefault(fb => fb.Key == DeviceNameFeedback.Key) as StringFeedback;

            if (deviceNameFeedback == null)
            {
                return;
            }

            deviceNameFeedback.OutputChange += SendUpdate;
        }

        protected override void RegisterActions()
        {
            base.RegisterActions();

            AddAction("/fullStatus", SendFullStatus);
            
        }

        private void SendFullStatus(string id, JToken content)
        {
            PostStatusMessage(new EndpointInfoStateMessage
            {
                DeviceName = deviceNameFeedback?.StringValue ?? string.Empty
            });
        }

        private void SendUpdate(object sender, FeedbackEventArgs args)
        {
            PostStatusMessage(JToken.FromObject(new EndpointInfoUpdateMessage
            {
                DeviceName = deviceNameFeedback?.StringValue ?? string.Empty
            }));
        }
    }

    public class EndpointInfoStateMessage : DeviceStateMessageBase
    {
        [JsonProperty("friendlyName")]
        public string DeviceName { get; set; }
    }

    public class EndpointInfoUpdateMessage 
    {
        [JsonProperty("friendlyName")]
        public string DeviceName { get; set; }
    }
}
