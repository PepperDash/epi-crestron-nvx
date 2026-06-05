using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using PepperDash.Core;
using PepperDash.Essentials.AppServer.Messengers;
using PepperDash.Essentials.Core;

namespace NvxEpi.McMessengers
{
    public class HdmiOutputMessenger : MessengerBase
    {
        private readonly List<NvxHdmiOutputPort> outputs;
        private readonly INvxDevice device;

        private readonly Timer debounceTimer;

        public HdmiOutputMessenger(string key, string path, INvxDevice device) : base(key, path, device)
        {
            this.device = device;
            outputs = device.OutputPorts.OfType<NvxHdmiOutputPort>().ToList();
            debounceTimer = new Timer(_ => UpdateStatus(), null, Timeout.Infinite, Timeout.Infinite);
        }

        protected override void RegisterActions()
        {
            base.RegisterActions();

            AddAction("/fullStatus", (id, content) => SendFullStatus(id));
            AddAction("/outputStatus", (id, content) => SendFullStatus(id));

            EventHandler<FeedbackEventArgs> feedbackChangedHandler = (sender, args) => debounceTimer.Change(100, Timeout.Infinite);

            foreach (var outputPort in outputs)
            {
                outputPort.DisabledByHdcpFeedback.OutputChange += feedbackChangedHandler;
                outputPort.OutputResolutionFeedback.OutputChange += feedbackChangedHandler;
                outputPort.EdidManufacturerFeedback.OutputChange += feedbackChangedHandler;
            }
        }

        private void SendFullStatus(string? id = null)
        {
            var message = new HdmiOutputFullState
            {
                HdmiOutputs = outputs.ToDictionary(
                    o => o.Key,
                    o => new HdmiOutputState(
                        disabledByHdcp: o.DisabledByHdcpFeedback.BoolValue,
                        outputResolution: o.OutputResolutionFeedback.StringValue,
                        edidManufacturer: o.EdidManufacturerFeedback.StringValue
                    )
                )
            };

            try
            {
                PostStatusMessage(message, id);
            }
            catch (Exception e)
            {
                Debug.LogMessage(e, "Exception sending message {exception}", this, e);
            }
        }

        private void UpdateStatus()
        {
            var message = new HdmiOutputFullState
            {
                HdmiOutputs = outputs.ToDictionary(
                    o => o.Key,
                    o => new HdmiOutputState(
                        disabledByHdcp: o.DisabledByHdcpFeedback.BoolValue,
                        outputResolution: o.OutputResolutionFeedback.StringValue,
                        edidManufacturer: o.EdidManufacturerFeedback.StringValue
                    )
                )
            };

            try
            {
                PostStatusMessage(message);
            }
            catch (Exception e)
            {
                Debug.LogMessage(e, "Exception sending message {exception}", this, e);
            }
        }
    }

    public class HdmiOutputFullState : DeviceStateMessageBase
    {
        [JsonProperty("hdmiOutputs")]
        public required Dictionary<string, HdmiOutputState> HdmiOutputs { get; set; } = new Dictionary<string, HdmiOutputState>();
    }

    public class HdmiOutputState(bool disabledByHdcp, string outputResolution, string edidManufacturer)
    {
        [JsonProperty("disabledByHdcp")]
        public bool DisabledByHdcp { get; } = disabledByHdcp;

        [JsonProperty("outputResolution")]
        public string OutputResolution { get; } = outputResolution;

        [JsonProperty("edidManufacturer")]
        public string EdidManufacturer { get; } = edidManufacturer;
    }
}
