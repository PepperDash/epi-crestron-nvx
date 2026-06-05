using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using PepperDash.Core.Logging;
using PepperDash.Essentials.AppServer.Messengers;
using PepperDash.Essentials.Core;

namespace NvxEpi.McMessengers
{
    public class HdmiInputMessenger : MessengerBase
    {
        private readonly List<NvxHdmiInputPort> inputs;
        private readonly Timer debounceTimer;

        public HdmiInputMessenger(string key, string path, INvxDevice device) : base(key, path, device)
        {
            inputs = device.InputPorts.OfType<NvxHdmiInputPort>().ToList();
            debounceTimer = new Timer(_ => UpdateStatus(), null, Timeout.Infinite, Timeout.Infinite);
            EventHandler<FeedbackEventArgs> feedbackHandler = (o, a) => debounceTimer.Change(200, Timeout.Infinite);

            foreach (var inputPort in inputs)
            {
                inputPort.AudioChannelCount.OutputChange += feedbackHandler;
                inputPort.AudioFormat.OutputChange += feedbackHandler;
                inputPort.ColorspaceMode.OutputChange += feedbackHandler;
                inputPort.HdcpCapability.OutputChange += feedbackHandler;
                inputPort.HdrType.OutputChange += feedbackHandler;
                inputPort.VideoStatus.HdcpStateFeedback.OutputChange += feedbackHandler;
                inputPort.VideoStatus.VideoResolutionFeedback.OutputChange += feedbackHandler;
                inputPort.VideoStatus.VideoSyncFeedback.OutputChange += feedbackHandler;
                inputPort.VideoStatus.HdcpActiveFeedback.OutputChange += feedbackHandler;
            }
        }

        protected override void RegisterActions()
        {
            base.RegisterActions();

            AddAction("/fullStatus", (id, content) => SendFullStatus(id));

            AddAction("/inputStatus", (id, content) => SendFullStatus(id));
        }

        private void SendFullStatus(string? id = null)
        {
            var hdmiInputs = GetInputState();
            var message = new HdmiInputFullState
            {
                SyncDetected = inputs.Any(h => h.VideoStatus.VideoSyncFeedback.BoolValue),
                HdmiInputs = hdmiInputs,
            };

            try
            {
                PostStatusMessage(message, id);
            }
            catch (Exception e)
            {
                this.LogError(e, "Exception sending message {exception}");
            }
        }

        private void UpdateStatus()
        {
            var hdmiInputs = GetInputState();
            var message = new HdmiInputFullState
            {
                SyncDetected = inputs.Any(h => h.VideoStatus.VideoSyncFeedback.BoolValue),
                HdmiInputs = hdmiInputs,
            };

            try
            {
                PostStatusMessage(message);
            }
            catch (Exception e)
            {
                this.LogError(e, "Exception sending message {exception}");
            }
        }

        private Dictionary<string, HdmiInputState> GetInputState()
        {
            return inputs.ToDictionary(
                i => i.Key,
                i => new HdmiInputState(
                    key: i.Key,
                    hdcpCapability: i.HdcpCapability.StringValue,
                    hdcpSupport: i.VideoStatus.HdcpStateFeedback.StringValue,
                    syncDetected: i.VideoStatus.VideoSyncFeedback.BoolValue,
                    currentResolution: i.VideoStatus.VideoResolutionFeedback.StringValue,
                    audioChannelCount: i.AudioChannelCount.IntValue,
                    audioFormat: i.AudioFormat.StringValue,
                    colorspaceMode: i.ColorspaceMode.StringValue,
                    hdrType: i.HdrType.StringValue
                )
            );
        }
    }

    public class HdmiInputFullState : DeviceStateMessageBase
    {
        /// <summary>
        /// Whether or not sync is detected on any input
        /// </summary>
        [JsonProperty("syncDetected")]
        public bool SyncDetected { get; set; }

        [JsonProperty("hdmiInputs")]
        public required Dictionary<string, HdmiInputState> HdmiInputs { get; set; }
    }

    public class HdmiInputState(string key, string hdcpCapability, string hdcpSupport, bool syncDetected, string currentResolution, int audioChannelCount, string audioFormat, string colorspaceMode, string hdrType)
    {
        [JsonIgnore]
        public string Key { get; } = key;

        [JsonProperty("hdcpCapability")]
        public string HdcpCapability { get; } = hdcpCapability;

        [JsonProperty("hdcpSupport")]
        public string HdcpSupport { get; } = hdcpSupport;

        [JsonProperty("syncDetected")]
        public bool SyncDetected { get; } = syncDetected;

        [JsonProperty("currentResolution")]
        public string CurrentResolution { get; } = currentResolution;

        [JsonProperty("audioChannelCount")]
        public int AudioChannelCount { get; } = audioChannelCount;

        [JsonProperty("audioFormat")]
        public string AudioFormat { get; } = audioFormat;

        [JsonProperty("colorspaceMode")]
        public string ColorspaceMode { get; } = colorspaceMode;

        [JsonProperty("hdrType")]
        public string HdrType { get; } = hdrType;
    }
}
