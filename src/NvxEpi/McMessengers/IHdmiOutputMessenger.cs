using Crestron.SimplSharpPro.DM;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using NvxEpi.Abstractions.HdmiOutput;
using NvxEpi.Devices;
using PepperDash.Core;
using PepperDash.Essentials.AppServer.Messengers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NvxEpi.McMessengers
{
    public class IHdmiOutputMessenger : MessengerBase
    {
        private readonly IHdmiOutput device;

        public IHdmiOutputMessenger(string key, string messagePath, IHdmiOutput device) : base(key, messagePath, device)
        {
            this.device = device;
        }

        protected override void RegisterActions()
        {
            base.RegisterActions();

            AddAction("/fullStatus",(id, content) => SendFullStatus());

            foreach (var feedback in device.Feedbacks.Where((f) => f.Key.Contains("HdmiOut")))
            {
                Debug.LogMessage(Serilog.Events.LogEventLevel.Verbose, "feedback key: {key}", this, feedback.Key);
                feedback.OutputChange += (o, a) => UpdateStatus();
            }
        }

        private void SendFullStatus()
        {        
            var message = new HdmiOutputFullState
            {
                HdmiOutputs = new Dictionary<string, HdmiOutputState>
                {
                    {"HdmiOut",new  HdmiOutputState(device) }
                }
            };            

            try
            {

                PostStatusMessage(message);
            } catch(Exception e)
            {
                Debug.LogMessage(e, "Exception sending message {exception}", this, e);
            }
        }

        private void UpdateStatus()
        {           
            PostStatusMessage(JToken.FromObject(new {
                hdmiOutput = new Dictionary<string, HdmiOutputState>
                {
                    {"HdmiOut",new  HdmiOutputState(device) }
                }
            }));
        }        
    }

    public class HdmiOutputFullState: DeviceStateMessageBase
    {
        [JsonProperty("hdmiOutputs")]
        public Dictionary<string, HdmiOutputState> HdmiOutputs { get; set; }
    }

    public class HdmiOutputState
    {
        private readonly IHdmiOutput device;

        [JsonProperty("disabledByHdcp")]
        public bool DisabledByHdcp => device.DisabledByHdcp.BoolValue;

        [JsonProperty("outputResolution")]
        public string OutputResolution => device.OutputResolution.StringValue;

        [JsonProperty("edidManufacturer")]
        public string EdidManufacturer => device.EdidManufacturer.StringValue;

        public HdmiOutputState(IHdmiOutput device)
        {
            this.device = device;
        }
    }
}
