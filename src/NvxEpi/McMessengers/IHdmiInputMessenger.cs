using Crestron.SimplSharpPro.DM;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using NvxEpi.Abstractions.HdmiInput;
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

            foreach (var feedback in device.Feedbacks.Where((f) => f.Key.Contains("Hdmi1") || f.Key.Contains("Hdmi2")))
            {                
                feedback.OutputChange += (o, a) => UpdateStatus();
            }
        }

        private void SendFullStatus()
        {
            var hdmiInputs = GetInputState();


            var message = new HdmiInputFullState
            {
                HdmiInputs = hdmiInputs,
            };

            try
            {
                PostStatusMessage(message);
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
                    return new HdmiInputState(device, hdmiIn);
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
        private readonly IHdmiInput device;
        private readonly uint input;
        [JsonIgnore]
        public string Key => $"HdmiIn{input}";

        [JsonProperty("hdcpCapability")]        
        public string HdcpCapability => device.HdcpCapabilityString[input].StringValue;

        [JsonProperty("hdcpSupport")]
        public string HdcpSupport => device.HdcpSupport[input].StringValue;

        [JsonProperty("syncDetected")]
        public bool SyncDetected => device.SyncDetected[input].BoolValue;

        [JsonProperty("currentResolution")]
        public string CurrentResolution => device.CurrentResolution[input].StringValue;

        [JsonProperty("audioChannelCount")]
        public int AudioChannelCount => device.AudioChannels[input].IntValue;

        [JsonProperty("audioFormat")]
        public string AudioFormat => device.AudioFormat[input].StringValue;

        [JsonProperty("colorspaceMode")]
        public string ColorspaceMode => device.ColorSpace[input].StringValue;

        [JsonProperty("hdrType")]
        public string HdrType => device.HdrType[input].StringValue;

        public HdmiInputState(IHdmiInput device, uint input)
        {
            this.device = device;
            this.input = input;
        }
    }
}
