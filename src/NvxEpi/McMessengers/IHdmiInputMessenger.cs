using Crestron.SimplSharpPro.DM;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.HdmiInput;
using Org.BouncyCastle.Crypto.Prng;
using PepperDash.Core;
using PepperDash.Essentials.AppServer.Messengers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NvxEpi.McMessengers
{
    public class IHdmiInputMessenger : MessengerBase
    {
        private readonly IHdmiInput device;

        public IHdmiInputMessenger(string key, string messagePath, IHdmiInput device) : base(key, messagePath, device)
        {
            this.device = device;
        }

        protected override void RegisterActions()
        {
            base.RegisterActions();

            AddAction("/fullStatus",(id, content) => SendFullStatus());

            foreach (var feedback in device.Feedbacks.Where(fb => fb.Key.Contains("Hdmi")))
            {
                feedback.OutputChange += (o, a) => UpdateStatus();
            }
        }

        private void SendFullStatus()
        {
            Debug.Console(1, this, "Sending Full Status");

            var hdmiInputs = GetInputState();

            Debug.Console(2, this, "Current input state: {@inputState}", hdmiInputs);

            var message = new HdmiInputFullState
            {
                HdmiInputs = hdmiInputs,
            };

            Debug.Console(2, this, "Sending message {@hdmiInputs}", message);

            try
            {

                PostStatusMessage(new HdmiInputFullState
                {
                    HdmiInputs = hdmiInputs,
                });
            } catch(Exception e)
            {
                Debug.Console(5, this, "Exception sending message {exception}", e);
            }
        }

        private void UpdateStatus()
        {           
            PostStatusMessage(JToken.FromObject(new {
                hdmiInputs = GetInputState()
            }));
        }

        private Dictionary<string, HdmiInputState> GetInputState()
        {            
            var hdmiInputs = device.Hardware.HdmiIn == null
                ? new Dictionary<string, HdmiInputState>()
                : device.Hardware.HdmiIn.Keys.Select(hdmiIn =>
                {                  
                    return new HdmiInputState
                    {
                        Key = string.Format("HdmiIn{0}", hdmiIn),
                        HdcpCapability = (eHdcpCapabilityType)device.HdcpCapability[hdmiIn]?.IntValue,
                        SyncDetected = device.SyncDetected[hdmiIn]?.BoolValue,
                        CurrentResolution = device.CurrentResolution[hdmiIn]?.StringValue,
                        AudioChannelCount = device.AudioChannels[hdmiIn]?.IntValue,
                        AudioFormat = device.AudioFormat[hdmiIn]?.StringValue,
                        ColorspaceMode = device.ColorSpace[hdmiIn]?.StringValue,
                        HdrType = device.HdrType[hdmiIn]?.StringValue,
                    };
                }).ToDictionary(hdmiInState => hdmiInState.Key, hdmiInputState => hdmiInputState);

            return hdmiInputs;
        }
    }

    public class HdmiInputFullState: DeviceStateMessageBase
    {
        [JsonProperty("hdmiInputs")]
        public Dictionary<string, HdmiInputState> HdmiInputs { get; set; }
    }

    public class HdmiInputState
    {
        [JsonIgnore]
        public string Key { get; set; }
        [JsonProperty("hdcpCapability", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public eHdcpCapabilityType? HdcpCapability { get; set; }

        [JsonProperty("syncDetected", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SyncDetected { get; set; }

        [JsonProperty("currentResolution", NullValueHandling = NullValueHandling.Ignore)]
        public string CurrentResolution { get; set; }

        [JsonProperty("audioChannelCount", NullValueHandling = NullValueHandling.Ignore)]
        public int? AudioChannelCount { get; set; }

        [JsonProperty("audioFormat", NullValueHandling = NullValueHandling.Ignore)]
        public string AudioFormat { get; set; }

        [JsonProperty("colorspaceMode", NullValueHandling = NullValueHandling.Ignore)]
        public string ColorspaceMode { get; set; }

        [JsonProperty("hdrType", NullValueHandling = NullValueHandling.Ignore)]
        public string HdrType { get; set; }
    }
}
